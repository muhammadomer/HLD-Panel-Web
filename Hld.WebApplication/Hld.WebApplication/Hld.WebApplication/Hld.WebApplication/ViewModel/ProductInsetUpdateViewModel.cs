using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ProductInsetUpdateViewModel
    {
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Sku is required")]
        [MaxLength(20, ErrorMessage = "Maximum 20 characters allowed")]
        [Display(Name = "SKU")]
        public string ProductSKU { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200, ErrorMessage = "Maximum 200 characters allowed")]
        [Display(Name = "Product Title")]
        public string ProductTitle { get; set; }

        [Required(ErrorMessage = "Condition is required")]
        [Display(Name = "Condition")]
        public int ConditionId { get; set; }
        public IEnumerable<SelectListItem> Condition { get; set; }

        [Display(Name = "Color")]
        public int ColorId { get; set; }
        //[Required(ErrorMessage = "Color is required")]
        public String Color { get; set; }

        [Display(Name = "Brand")]
        public int BrandId { get; set; }
        public String Brand { get; set; }

        [Display(Name = "UPC")]
        //[MinLength(12,ErrorMessage = "Min. 12 characters allowed")]
        //[MaxLength(12, ErrorMessage = "Max. 12 characters allowed")]
        public string Upc { get; set; }

        [Display(Name = "Category")]
        //[Required(ErrorMessage ="Category is required")]
        public string Category { get; set; }

        [MaxLength(1000, ErrorMessage = "Max. 1000 character Required")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Avg Cost (US$)")]
        public decimal AvgCost { get; set; }

        [Display(Name = "Ship Wt (oz)")]
        public decimal ShipmentWeight { get; set; }

        public string CategoryIds { get; set; }

        public int CategoryMain { get; set; }
        public int CategorySub1 { get; set; }
        public int CategorySub2 { get; set; }
        public int CategorySub3 { get; set; }
        public int CategorySub4 { get; set; }

        public List<IFormFile> UploadImages { get; set; }

        public decimal shipmentLenght { get; set; }
        public decimal shipmentWidth { get; set; }
        public decimal shipmentHeight { get; set; }

        public double CurrencyRate { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public double CADPrice { get; set; } // canadian price
        public string StatusName { get; set; }

        public List<ProductWarehouseQtyViewModel> ListProductWarehouseQty { get; set; }
    }

    public class ProductInsertUpdateViewModel
    {
        public int ProductId { get; set; }
        public string ProductSKU { get; set; }
        public string ProductTitle { get; set; }
        public int ConditionId { get; set; }
        public IEnumerable<SelectListItem> Condition { get; set; }
        public String Color { get; set; }
        public int ColorId { get; set; }
        public int BrandId { get; set; }
        public String Brand { get; set; }
        public string Upc { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public decimal AvgCost { get; set; }
        public decimal ShipmentWeight { get; set; }
        public string CategoryIds { get; set; }
        public int CategoryMain { get; set; }
        public int CategorySub1 { get; set; }
        public int CategorySub2 { get; set; }
        public int CategorySub3 { get; set; }
        public int CategorySub4 { get; set; }
        public decimal shipmentLenght { get; set; }
        public decimal shipmentWidth { get; set; }
        public decimal shipmentHeight { get; set; }
        public string StatusName { get; set; }

    }
}
