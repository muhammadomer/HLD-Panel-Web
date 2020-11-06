using Microsoft.AspNetCore.Mvc.Rendering;
using Renci.SshNet.Security.Cryptography.Ciphers.Modes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ProductManufacturedViewModel
    {
        public int ManufactureId { get;set;}
        public int ManufactureModelId { get; set; }
        public int ParentID { get;set;}
        [Required(ErrorMessage = "ManufactureName is required")]
        [MaxLength(20, ErrorMessage = "Maximum 20 characters allowed")]
        public  string ManufactureName { get;set;}     
        public string ManufactureModel { get;set;}
        [Required(ErrorMessage = "DeviceModel is required")]
        public string DeviceModel { get;set;}     
    }
}
