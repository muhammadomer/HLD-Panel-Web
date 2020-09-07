using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModels
{
    public class CreditCardDetailViewModel
    {

        public int CreditCardDetailId { get; set; }
        [Required(ErrorMessage = "Please inter your card name first")]
        public string name_on_card { get; set; }
        [Required(ErrorMessage = "Please inter your card number first")]
        public string number { get; set; }
        [Required(ErrorMessage = "Please inter your secuirty code first")]
        public string security_code { get; set; }

        [Required(ErrorMessage = "Please select your expiry month first")]
        public string expiration_month { get; set; }
        [Required(ErrorMessage = "Please select your expiry year first")]
        public string expiration_year { get; set; }
        [Required(ErrorMessage = "Please inter your first name!")]
        public string first_name { get; set; }
        [Required(ErrorMessage = "Please inter your last name!")]
        public string last_name { get; set; }
        [Required(ErrorMessage = "Please inter your address line first!")]

        public string address_line1 { get; set; }

        public string address_line2 { get; set; }
        [Required(ErrorMessage = "Please inter your zip code first!")]
        public string zip_code { get; set; }
        [Required(ErrorMessage = "Please inter your city first!")]
        public string city { get; set; }
        [Required(ErrorMessage = "Please inter your state first!")]
        public string state { get; set; }
        [Required(ErrorMessage = "Please select your country first!")]
        public string country { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        [Required(ErrorMessage = "Please insert your number first!")]
        //[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
        //           ErrorMessage = "Entered phone format is not valid.")]

        public string PhoneNo { get; set; }



        public string name_on_cardShort { get; set; }

        public string numberShort { get; set; }

        public string security_codeShort { get; set; }
    }
}
