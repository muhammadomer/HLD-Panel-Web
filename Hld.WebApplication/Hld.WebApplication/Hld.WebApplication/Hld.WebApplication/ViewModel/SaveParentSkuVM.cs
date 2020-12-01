﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class SaveParentSkuVM
    {
       
        public int Parentproduct_id { get; set; }
        public int ManufactureId { get; set; }
        public int Childproduct_id { get; set; }
        public int productstatus { get; set; }
        public string Sku { get; set; }
        public string Upc { get; set; }
        public string ProductTitle { get; set; }
        public int ConditionId { get; set; }
        public IEnumerable<SelectListItem> Condition { get; set; }
        public string CatagoryName { get; set; }
        public decimal ShipWt { get; set; }
        public decimal ShipLt { get; set; }
        public decimal ShipHt { get; set; }
        public decimal ShipmentWeight { get; set; }
        public string ManufactureName { get; set; }
        public int ManufactureModel { get; set; }
        //public string Menufacture { get; set; }
        //public string MenufactureModel { get; set; }
        public int DeviceModel { get; set; }
        public int Style { get; set; }
        public string StyleName { get; set; }
        public bool IsCreatedOnSC { get; set; }
        public string Feature { get; set; }
        public string Description { get; set; }
        public int ColorId { get; set; }
        public string Color { get; set; }
        [Display(Name = "Brand")]
        public int BrandId { get; set; }
        public string Brand { get; set; }
        public string ColorAlias { get; set; }
        public string ImageName { get; set; }
        public string CompressedImage { get; set; }
        public DateTime SkuCreationDate { get; set; }
    }
}
