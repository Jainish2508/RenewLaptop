//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Renew_Laptop.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Customer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Customer()
        {
            this.Orders = new HashSet<Order>();
        }
        [Key]
        public int CustomerID { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Enter a valid first name.")]
        [StringLength(50, ErrorMessage = "Max Char. 50")]
        [MinLength(2, ErrorMessage = "Min Char. 2")]
        [Display(Name = "Firstname")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Enter a valid last name.")]
        [StringLength(50, ErrorMessage = "Max Char. 50")]
        [MinLength(2, ErrorMessage = "Min Char. 2")]
        [Display(Name = "Lastname")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [EmailAddress(ErrorMessage = "Enter Valid Email Address.")]
        [StringLength(50, ErrorMessage = "Max Char. 50")]
        [MinLength(2, ErrorMessage = "Min Char. 2")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^(\d{10})$", ErrorMessage = "Not a valid contact number.")]
        [StringLength(10, ErrorMessage = "Max Length 10")]
        [Display(Name = "Contact Number")]
        public string Phone { get; set; }

        [StringLength(50, ErrorMessage = "Max Char. 50")]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Display(Name = "GST Number")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Not a valid GST number.")]
        [MinLength(15, ErrorMessage = "Min Length 15.")]
        [StringLength(15, ErrorMessage = "Max Length 15.")]
        public string GSTIN { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "Address Line 1")]
        [StringLength(100, ErrorMessage = "Max Length 100.")]
        public string AddressLine1 { get; set; }

        [Display(Name = "Address Line 2")]
        [StringLength(100, ErrorMessage = "Max Length 100.")]
        public string AddressLine2 { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "City")]
        [StringLength(50, ErrorMessage = "Max Char. 50")]
        public string City { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "State")]
        [StringLength(50, ErrorMessage = "Max Char. 50")]
        public string State { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^(\d{6})$", ErrorMessage = "Not a valid Pin Code.")]
        [StringLength(6, ErrorMessage = "Max Length 6")]
        [Display(Name = "PinCode")]
        public string PostalCode { get; set; }

        public System.DateTime CreatedOn { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
