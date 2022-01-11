using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Renew_Laptop.Models
{
    public class ContactViewModel
    {
        [DataType(DataType.Text)]
        public string name { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [DataType(DataType.PhoneNumber)]
        public string phone { get; set; }

        [DataType(DataType.Text)]
        public string subject { get; set; }

        [DataType(DataType.MultilineText)]
        public string message { get; set; }

        public class CaptchaResponse
        {
            [JsonProperty("success")]
            [Required(ErrorMessage = "This field is required")]
            public bool Success { get; set; }

            [JsonProperty("error-codes")]
            public List<string> ErrorMessage { get; set; }
        }
    }
}