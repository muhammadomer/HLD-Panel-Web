using DataAccess.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ZincProxyViewModel
    {

        [Required(ErrorMessage = "Please enter proxy Ip!")]
        public string ProxyIP { get; set; }
        [Required(ErrorMessage = "Please enter your password !")]
        public string ProxyPassword { get; set; }
        public string ProxyPasswordShort { get; set; }
        [Required(ErrorMessage = "Please enter  proxy port !")]
        public string ProxyPort { get; set; }
        [Required(ErrorMessage = "Please enter  proxy name !")]
        public string ProxyUser { get; set; }
        [Required(ErrorMessage = "Please enter  proxy email !")]
        public string ProxyEmail { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public List<SaveZincProxyEmailViewModel> List { get; set; }
    }

}
