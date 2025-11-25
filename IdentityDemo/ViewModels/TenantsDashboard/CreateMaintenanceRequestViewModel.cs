using System.ComponentModel.DataAnnotations;

namespace TenantsManagementApp.ViewModels.TenantsDashboard
{
    public class CreateMaintenanceRequestViewModel
    {
        [Required]
        public string Category { get; set; }

        [Required]
        public string Priority { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; }
    }
}
