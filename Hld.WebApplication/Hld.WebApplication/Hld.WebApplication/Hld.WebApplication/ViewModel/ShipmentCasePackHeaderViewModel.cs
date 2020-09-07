using DataAccess.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ShipmentCasePackHeaderViewModel
    {
        public string ShipmentId { get; set; }
        public string ShipmentName { get; set; }
        public string Notes { get; set; }
        public string BoxId { get; set; }
        public int VendorId { get; set; }
        public int ShipedQty { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Length { get; set; }
        public decimal Weight { get; set; }
        public int NoOfBoxes { get; set; }
        public int Status { get; set; }
        public int SKUs { get; set; }
        public int POs { get; set; }
        public List<ShipmentCasePackProductViewModel> list { get; set; }
    }
}
