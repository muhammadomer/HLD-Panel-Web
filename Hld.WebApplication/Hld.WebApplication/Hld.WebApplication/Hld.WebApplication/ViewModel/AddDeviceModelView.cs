﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class AddDeviceModelView
    {
        public int ManufactureId { get; set; }
        public string Manufacture { get; set; }
        public string ManufactureModel { get; set; }
        public int ManufactureModelId { get; set; }
        public string DeviceModel { get; set; }
    }
}
