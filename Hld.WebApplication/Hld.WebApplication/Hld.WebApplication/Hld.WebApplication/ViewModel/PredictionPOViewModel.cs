using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class PredictionPOViewModel
    {
        public int idPurchaseOrdersItems { get; set; }
        public int VendorId { get; set; }
        public string Vendor { get; set; }
        public decimal LowStock90 { get; set; }
    
        public int CoverPhy { get; set; }
        public string Currency { get; set; }
        public string Notes { get; set; }
        public int POQty { get; set; }
        public string SKU { get; set; }
        public int OnOrder { get; set; }
        public int CasePackQty { get; set; }
        public string LargImage { get; set; }
        public string SmallImage { get; set; }
        public decimal ApprovedUnitPrice { get; set; }
        public int InternalPOID { get; set; }
        public int QtySold60 { get; set; }
        public int QtySold90 { get; set; }
        public int CoverDays { get; set; }
        public decimal Velocity { get; set; }
        public int PhysicalQty { get; set; }
        public int POstatus { get; set; }
        public int ReservedQty { get; set; }
        public DateTime OrderedOn { get; set; }
        public string VendorAlias { get; set; }
        public List<PredictionSKUs> list { get; set; }
    }
    public class PredictionSKUs
    {
        public int idPurchaseOrdersItems { get; set; }
        public int VendorId { get; set; }
        public int ReservedQty { get; set; }
        public decimal LowStock90 { get; set; }

        public int CoverPhy { get; set; }
        public int POQty { get; set; }
        public int CasePackQty { get; set; }
        public int OnOrder { get; set; }
        public string SKU { get; set; }
        public string VendorAlias { get; set; }
        public string LargImage { get; set; }
        public string SmallImage { get; set; }
        public decimal ApprovedUnitPrice { get; set; }

        public string Currency { get; set; }
        public int QtySold60 { get; set; }
        public int QtySold90 { get; set; }
        public int CoverDays { get; set; }
        public decimal Velocity { get; set; }
        public int PhysicalQty { get; set; }

    }
    public class PredictionInternalPOList
    {
        public string Vendor { get; set; }
        public int InternalPOID { get; set; }
    }


    public class Product
    {
        public string ProductID { get; set; }
        public int QtyUnitsOrdered { get; set; }
        public double UnitPrice { get; set; }
    }

    public class SCShippingAddress
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string ContactName { get; set; }
        public string Business { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Fax { get; set; }
        public string Region { get; set; }
        public string State { get; set; }
        public string Phone { get; set; }
    }

    public class SCPOViewModel
    {
        public int CompanyID { get; set; }
        public int VendorID { get; set; }
        public string POType { get; set; }
        public bool CaseQtyMode { get; set; }
        public int DefaultWarehouseID { get; set; }
        public string Description { get; set; }
        public string VendorNote { get; set; }
        public int PaymentTermID { get; set; }
        public List<Product> Products { get; set; }
        public SCShippingAddress ShippingAddress { get; set; }
    }

    public class PredictionInternalSKUList
    {
        public int InternalPOID { get; set; }
        public int VendorId { get; set; }
        public string SKU { get; set; }
    }
}
