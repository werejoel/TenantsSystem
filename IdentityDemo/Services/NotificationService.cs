using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantsManagementApp.Data;
using TenantsManagementApp.Models;

namespace TenantsManagementApp.Services;

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEmailService _emailService;
    private readonly ILogger<NotificationService> _logger;
    private readonly IConfiguration _configuration;

    public NotificationService(
        ApplicationDbContext dbContext,
        IEmailService emailService,
        ILogger<NotificationService> logger,
        IConfiguration configuration)
    {
        _dbContext = dbContext;
        _emailService = emailService;
        _logger = logger;
        _configuration = configuration;
    }

    // Send a notification (used by PaymentsController)
    public async Task SendNotificationAsync(Guid userId, string message, string subject)
    {
        try
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for notification", userId);
                return;
            }

            // Send email
            await _emailService.SendEmailAsync(user.Email, subject, message);
            _logger.LogInformation("Email sent to {Email} with subject: {Subject}", user.Email, subject);

            // Save notification to database
            var notification = new Notification
            {
                UserId = userId,
                Subject = subject,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _dbContext.Notifications.Add(notification);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Notification saved for user {UserId}, ID: {NotificationId}", userId, notification.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification to user {UserId}", userId);
            throw;
        }
    }

    // Get all notifications for a user
    public async Task<List<Notification>> GetUserNotificationsAsync(Guid userId)
    {
        try
        {
            var notifications = await _dbContext.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            _logger.LogInformation("Retrieved {Count} notifications for user {UserId}", notifications.Count, userId);
            return notifications;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve notifications for user {UserId}", userId);
            throw;
        }
    }

    // Create a new notification
    public async Task<Notification> CreateNotificationAsync(Notification notification)
    {
        if (notification == null || notification.UserId == Guid.Empty)
        {
            _logger.LogWarning("Invalid notification data provided");
            throw new ArgumentException("Invalid notification data");
        }

        try
        {
            var user = await _dbContext.Users.FindAsync(notification.UserId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for notification", notification.UserId);
                throw new ArgumentException("User not found");
            }

            notification.CreatedAt = DateTime.UtcNow;
            notification.UpdatedAt = DateTime.UtcNow;
            notification.IsRead = false;

            _dbContext.Notifications.Add(notification);
            await _dbContext.SaveChangesAsync();

            // Send email
            await _emailService.SendEmailAsync(user.Email, notification.Subject, notification.Message);
            _logger.LogInformation("Notification created and email sent for user {UserId}, ID: {NotificationId}", notification.UserId, notification.Id);

            return notification;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create notification for user {UserId}", notification.UserId);
            throw;
        }
    }

    // Mark a notification as read
    public async Task<bool> MarkAsReadAsync(int notificationId)
    {
        try
        {
            var notification = await _dbContext.Notifications.FindAsync(notificationId);
            if (notification == null)
            {
                _logger.LogWarning("Notification with ID {NotificationId} not found", notificationId);
                return false;
            }

            notification.IsRead = true;
            notification.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Notification {NotificationId} marked as read", notificationId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to mark notification {NotificationId} as read", notificationId);
            throw;
        }
    }

    // Send payment reminders for overdue charges
    public async Task SendPaymentRemindersAsync()
    {
        try
        {
            var overdueCharges = await _dbContext.Charges
                .Include(c => c.Tenant)
                .ThenInclude(t => t.User)
                .Where(c => c.Status != "Paid" && c.DueDate < DateTime.UtcNow)
                .ToListAsync();

            foreach (var charge in overdueCharges)
            {
                if (charge.Tenant?.User == null) continue;

                var message = $"Reminder: Your {charge.ChargeType} payment of {charge.Amount:C} for house ID {charge.HouseId} is overdue. Please pay by {charge.DueDate:MMMM dd, yyyy}.";
                var subject = $"Overdue {charge.ChargeType} Payment Reminder";

                await SendNotificationAsync(charge.Tenant.UserId, message, subject);
                _logger.LogInformation("Payment reminder sent for charge {ChargeId} to user {UserId}", charge.Id, charge.Tenant.UserId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send payment reminders");
            throw;
        }
    }

    // Send lease expiry reminders
    public async Task SendLeaseExpiryRemindersAsync()
    {
        try
        {
            var thresholdDate = DateTime.UtcNow.AddDays(30); // Notify for leases expiring within 30 days
            var expiringLeases = await _dbContext.Tenants
                .Include(t => t.User)
                .Where(t => t.IsActive && t.LeaseEndDate.HasValue && t.LeaseEndDate <= thresholdDate && t.LeaseEndDate >= DateTime.UtcNow)
                .ToListAsync();

            foreach (var tenant in expiringLeases)
            {
                if (tenant.User == null) continue;

                var message = $"Reminder: Your lease for house ID {tenant.HouseId} is set to expire on {tenant.LeaseEndDate:MMMM dd, yyyy}. Please contact the landlord to renew.";
                var subject = "Lease Expiry Reminder";

                await SendNotificationAsync(tenant.UserId, message, subject);
                _logger.LogInformation("Lease expiry reminder sent to user {UserId} for tenant {TenantId}", tenant.UserId, tenant.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send lease expiry reminders");
            throw;
        }
    }
}