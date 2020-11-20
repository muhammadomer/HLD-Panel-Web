﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class EditParentViewModel
    {
        public int Parentproduct_id { get; set; }
        public int productstatus { get; set; }
        public string Sku { get; set; }
        public string ProductTitle { get; set; }
        public int ConditionId { get; set; }
        public string ConditionName { get; set; }
        //public IEnumerable<SelectListItem> Condition { get; set; }

        public string CatagoryName { get; set; }
        public decimal ShipmentWeight { get; set; }
        public decimal ShipWt { get; set; }
        public decimal ShipLt { get; set; }
        public decimal ShipHt { get; set; }
        public int ManufactureId { get; set; }
        public string ManufactureName { get; set; }
        public int ManufactureModelId { get; set; }
        public string ManufactureModelName { get; set; }
        public int DeviceModelId { get; set; }
        public string DeviceModelName { get; set; }
        public int StyleId { get; set; }
        public string StyleName { get; set; }
        public bool IsCreatedOnSC { get; set; }
        public string Description { get; set; }
        public String Color { get; set; }
        public string ColorAlias { get; set; }
        public int ColorId { get; set; }
        public string Upc { get; set; }
        public DateTime SkuCreationDate { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
    }
}
