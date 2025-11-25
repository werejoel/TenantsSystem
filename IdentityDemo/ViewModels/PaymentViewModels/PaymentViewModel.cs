using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TenantsManagementApp.Models.ViewModels;

public class PaymentViewModel
{
    [Required(ErrorMessage = "Please select a tenant.")]
    public int TenantId { get; set; }
    public List<SelectListItem> Tenants { get; set; } = new List<SelectListItem>();

    [Required(ErrorMessage = "Please select a house.")]
    public int HouseId { get; set; }
    public List<SelectListItem> Houses { get; set; } = new List<SelectListItem>();

    [Required(ErrorMessage = "Amount is required.")]
    [Range(100, double.MaxValue, ErrorMessage = "Amount must be at least 100 UGX.")]
    public decimal AmountPaid { get; set; }

    [Required(ErrorMessage = "Purpose is required.")]
    [StringLength(100, ErrorMessage = "Purpose cannot exceed 100 characters.")]
    public string Purpose { get; set; } = "Rent";

    [Required(ErrorMessage = "Payment date is required.")]
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    public DateTime? PeriodStart { get; set; }
    public DateTime? PeriodEnd { get; set; }

    [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
    public string? Notes { get; set; }

    public string RedirectUrl { get; set; } = "https://yourapp.com/payment-callback";

    [Required(ErrorMessage = "Please select at least one charge.")]
    public List<int> ChargeIds { get; set; } = new List<int>();
    public List<SelectListItem> Charges { get; set; } = new List<SelectListItem>();
}