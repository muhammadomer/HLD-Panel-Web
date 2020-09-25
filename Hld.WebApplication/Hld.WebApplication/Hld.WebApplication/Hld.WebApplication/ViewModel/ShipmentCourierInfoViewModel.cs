using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ShipmentCourierInfoViewModel
    {
        public string ShipmentId { get; set; }
        public string CourierCode { get; set; }
        public string TrakingNumber { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
