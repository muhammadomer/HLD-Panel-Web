using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class CatageoriesViewModel
    {
        [Display(Name = "Category 1")]
        public string CatageoryMainLabel { get; set; }
        public IEnumerable<SelectListItem> CatageoryMain { get; set; }

        [Display(Name = "Category 2")]
        public string CatageorySub1Label { get; set; }
        public IEnumerable<SelectListItem> CatageorySub1 { get; set; }

        [Display(Name = "Category 3")]
        public string CatageorySub2Label { get; set; }
        public IEnumerable<SelectListItem> CatageorySub2 { get; set; }

        [Display(Name = "Category 4")]
        public string CatageorySub3Label { get; set; }
        public IEnumerable<SelectListItem> CatageorySub3 { get; set; }

        [Display(Name = "Category 5")]
        public string CatageorySub4Label { get; set; }
        public IEnumerable<SelectListItem> CatageorySub4 { get; set; }
    }
}
