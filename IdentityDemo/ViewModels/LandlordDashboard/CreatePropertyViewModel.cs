using System.ComponentModel.DataAnnotations;

namespace TenantsManagementApp.ViewModels.LandlordDashboard
{
    public class CreatePropertyViewModel
    {
        [Required]
        [StringLength(200)]
        public string PropertyName { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string PropertyType { get; set; } = string.Empty;

        [Required]
        [Range(1, 1000)]
        public int TotalUnits { get; set; }
    }
}
