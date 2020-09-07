using DataAccess.ViewModels;
using Hld.WebApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ShipementProductSearchViewModel
    {
        public string BoxId { get; set; }
        public ShipmentBoxDetailViewModel model { get; set; }

    }
}
