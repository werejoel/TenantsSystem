using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenantsManagementApp.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        [Required]
        public string FirstName { get; set; } = null!;

        public string? LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime? LastLogin { get; set; }

        public bool IsActive { get; set; } = true;

        // Audit Columns
        public DateTime? CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedOn { get; set; } = DateTime.UtcNow;

        // Manual email confirmation code
        public string? EmailConfirmationCode { get; set; }

        // Email confirmation code expiration and resend tracking
        public DateTime? EmailConfirmationCodeExpiresOn { get; set; }

        public DateTime? LastConfirmationCodeResendOn { get; set; }

        // TMS specific properties
        [StringLength(50)]
        public string? NationalId { get; set; }

        [StringLength(20)]
        public string? EmergencyContact { get; set; }

        // Navigation Properties
        public virtual List<Address> Addresses { get; set; } = new List<Address>();

        // TMS Navigation Properties
        public virtual Tenant? Tenant { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        // Identity Navigation Properties
        public virtual ICollection<IdentityUserRole<Guid>> UserRoles { get; set; } = new List<IdentityUserRole<Guid>>();

        // Computed Properties
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}".Trim();

        [NotMapped]
        public bool IsTenant => Tenant != null;


        [NotMapped]
        public bool IsManager => false;
        public bool IsAdmin => false;
    }
}