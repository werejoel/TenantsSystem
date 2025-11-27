using System.ComponentModel.DataAnnotations;

namespace TenantsManagementApp.Models
{
    public class PaymentMethod : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [StringLength(50)]
        public string? Code { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
