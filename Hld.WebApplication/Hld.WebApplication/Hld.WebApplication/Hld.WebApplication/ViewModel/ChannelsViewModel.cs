using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ChannelsViewModel
    {
        [Display(Name = "User Name:")]
        [Required(ErrorMessage = "please fill this field")]
        public string UserName { get; set; }
        [Display(Name = "Password:")]
        [Required(ErrorMessage = "please fill this field")]
        public string Password { get; set; }
    }

    public class AmazonZincViewModel
    {
        [Display(Name = "User Name:")]
        [Required(ErrorMessage = "please fill this field")]
        public string UserName { get; set; }
        [Display(Name = "Password:")]
        [Required(ErrorMessage = "please fill this field")]
        public string Password { get; set; }
        [Display(Name = "2FA:")]
        [Required(ErrorMessage = "please fill this field")]
        public string FAKey { get; set; }
    }
}
