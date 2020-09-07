using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModels
{
    public class CasePackViewModel
    {
        public int CasePackId { get; set; }
        [Required(ErrorMessage = "Please enter SKU!")]
        public string SKU { get; set; }
        public int VendorId { get; set; }
        public string UserAlias { get; set; }
        [Required(ErrorMessage = "Please enter Qty Per Box!")]
        public int QtyPerBox { get; set; }
        [Required(ErrorMessage = "Please enter Width!")]
        public decimal Width { get; set; }
        [Required(ErrorMessage = "Please enter Width!")]
        public decimal Height { get; set; }
        [Required(ErrorMessage = "Please enter Width!")]
        public decimal Length { get; set; }
        [Required(ErrorMessage = "Please enter Width!")]
        public decimal Weight { get; set; }

        public string Title { get; set; }
        public string CompressedImage { get; set; }
        public string ImageName { get; set; }
        public int Counter { get; set; }
    }
}
