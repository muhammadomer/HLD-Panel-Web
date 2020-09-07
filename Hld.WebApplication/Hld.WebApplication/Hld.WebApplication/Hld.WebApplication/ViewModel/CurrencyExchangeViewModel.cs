using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class CurrencyExchangeViewModel
    {
        public DateTime dateTime { get; set; }  
        [Required(ErrorMessage ="USD TO CAD rate is required")]
        public decimal USD_To_CAD { get; set; }
        public decimal USD_To_CNY { get; set; }
        public bool IsActive { get; set; }
        public int CurrencyExchangeID { get; set; }
        public string Formated => dateTime.ToString("dd/MM/yyyy");
    }
}
