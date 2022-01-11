using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Renew_Laptop.Models
{
    public class DiagnosticViewModel
    {

        [Required(ErrorMessage = "This field is required.")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[\w]$", ErrorMessage ="Enter a valid name.")]
        [Display(Name = "Firstname")]
        public string FirstName { get; set; }
        
        [Required(ErrorMessage = "This field is required.")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[\w]$", ErrorMessage = "Enter a valid name.")]
        [Display(Name = "Lastname")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [EmailAddress(ErrorMessage = "Enter Valid Email Address.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^(\d{10})$", ErrorMessage = "Not a valid contact number.")]
        [Display(Name = "Contact Number")]
        public string Phone { get; set; }

        [Display(Name = "GST Number (Optional)")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Not a valid GST number.")]
        public string GST { get; set; }

        [Display(Name = "Company Name")]
        public string Company { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        public string OrderId { get; set; }

        public string Description { get; set; }

        public static string TransactionId { get; set; }

    }

    public class Orders
    {
        public int OrderID { get; set; }

        public int CustomerID { get; set; }

        public string PaymentID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime OrderDate { get; set; }

        public float GST { get; set; }

        public int TransacStatus { get; set; }

        public string ErrLoc { get; set; }

        public string ErrMsg { get; set; }

        public int Deleted { get; set; }

        public int Paid { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PaymentDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BillDate { get; set; }

        public string IP {get; set;}
    }

    public class Customers
    {
        public int CustomerID { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[\w]$", ErrorMessage = "Enter a valid name.")]
        [Display(Name = "Firstname")]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[\w]$", ErrorMessage = "Enter a valid name.")]
        [Display(Name = "Lastname")]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [EmailAddress(ErrorMessage = "Enter Valid Email Address.")]
        [Display(Name = "Email")]
        [StringLength(50)]
        public string Email { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^(\d{10})$", ErrorMessage = "Not a valid contact number.")]
        [Display(Name = "Contact Number")]
        [StringLength(10)]
        public string Phone { get; set; }

        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Display(Name = "GST Number (Optional)")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Not a valid GST number.")]
        [DisplayFormat(DataFormatString = "{0:n15}", ApplyFormatInEditMode = true)]
        public decimal GSTIN { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "Address Line 1")]
        [StringLength(100)]
        public string AddressLine1 { get; set; }

        [Display(Name = "Address Line 2")]
        [StringLength(100)]
        public string AddressLine2 { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "State")]
        public string State { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "Postal Code")]
        [RegularExpression(@"^(\d{6})$", ErrorMessage = "Not a valid postal code.")]
        [DisplayFormat(DataFormatString = "{0:n6}", ApplyFormatInEditMode = true)]
        public decimal PostalCode { get; set; }

    }

    
}