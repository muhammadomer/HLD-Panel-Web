using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class UpdatePOAcceptedViewModel
    {
        public int IsAccepted { get; set; }
        public int POId { get; set; }

        [DataType(DataType.Date)]
        public DateTime POLastUpdate { get; set; }
        [DataType(DataType.Date)]
        public DateTime POArrivalDate { get; set; }

    }

    public class UpdatePOExchangeViewModel
    {

        public int POId { get; set; }
        public decimal ExchangeRate { get; set; }
    }
}
