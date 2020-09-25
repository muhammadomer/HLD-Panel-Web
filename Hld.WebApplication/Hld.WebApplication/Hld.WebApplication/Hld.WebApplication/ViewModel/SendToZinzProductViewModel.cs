using DataAccess.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class SendToZincProductViewModel
    {
        public int OrderId { get; set; }
        public string Asin { get; set; }
        public string Sku { get; set; }
        public string Shipdays { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public int Qty { get; set; }
        public string ReqId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string getaddressname { get; set; }
        public int CreditCardId { get; set; }
        public int ZincAccountId { get; set; }
        public List<GetAddressViewModel> getaddress { get; set; }
        public List<CreditCardDetailViewModel> CreditCardDetail { get; set; }
        public List<ZincAccountsViewModel> ZincAccounts { get; set; }
    }
}
