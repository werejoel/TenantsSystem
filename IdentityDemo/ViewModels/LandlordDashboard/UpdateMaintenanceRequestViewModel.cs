using System.ComponentModel.DataAnnotations;

namespace TenantsManagementApp.ViewModels.LandlordDashboard
{
    public class UpdateMaintenanceRequestViewModel
    {
    [Required]
    public int RequestId { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        public string AssignedTo { get; set; } = string.Empty;
        public decimal? EstimatedCost { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
