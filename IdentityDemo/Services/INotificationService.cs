using TenantsManagementApp.Models;

namespace TenantsManagementApp.Services
{
    public interface INotificationService
    {
        Task SendNotificationAsync(Guid userId, string message, string subject);
        Task<List<Notification>> GetUserNotificationsAsync(Guid userId);
        Task<Notification> CreateNotificationAsync(Notification notification);
        Task<bool> MarkAsReadAsync(int notificationId);
        Task SendPaymentRemindersAsync();
        Task SendLeaseExpiryRemindersAsync();
    }
}
