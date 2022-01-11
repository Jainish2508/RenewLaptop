using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Renew_Laptop.Models
{
    public class OrderViewModel
    {
        [Required(ErrorMessage = "This field is required")]
        [DataType(DataType.Text)]
        [Display(Name = "First Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Invalid Phone Number")]
        [Display(Name = "Contact Number")]
        public string Phone { get; set; }

        public string OrderId { get; set; }

        public string RazorPayKey { get; set; }

        public int Amount { get; set; }

        public string Currency { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        public string Description { get; set; }
    }
}