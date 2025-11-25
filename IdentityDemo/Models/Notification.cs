using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenantsManagementApp.Models
{
    public class Notification : BaseEntity
    {
        [ForeignKey("User")]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Type { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string Title { get; set; } = null!;

        [StringLength(255)]
        public string? Subject { get; set; }

        [Required]
        [StringLength(2000)]
        public string Message { get; set; } = null!;

        [StringLength(50)]
        public string? Channel { get; set; }

        public bool IsRead { get; set; } = false;

        public bool IsSent { get; set; } = false;

        public DateTime? ScheduledAt { get; set; }

        public DateTime? SentAt { get; set; }

        // Navigation Properties
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
