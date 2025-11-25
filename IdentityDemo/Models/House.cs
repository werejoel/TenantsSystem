using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace TenantsManagementApp.Models
{
    public class House : BaseEntity
    {
    [Required(ErrorMessage = "Please select a landlord.")]
    [ForeignKey("Landlord")]
    public int LandlordId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [StringLength(500)]
        public string? Location { get; set; }

        [StringLength(100)]
        public string? Model { get; set; } // Type of house

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Vacant"; // "Vacant", "Occupied", "Under Maintenance"

        public bool IsActive { get; set; } = true;

    // Navigation Properties
    public virtual Landlord? Landlord { get; set; }
        public virtual ICollection<Tenant> Tenants { get; set; } = new List<Tenant>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<Charge> Charges { get; set; } = new List<Charge>();
        public virtual ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}
