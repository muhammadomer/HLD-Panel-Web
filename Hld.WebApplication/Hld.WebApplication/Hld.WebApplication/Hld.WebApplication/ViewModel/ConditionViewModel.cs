using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ConditionViewModel
    {
        public int ConditionId { get; set; }

        [Required(ErrorMessage = "Condition is Required")]
        [Display(Name = "Name")]
        public string ConditionName { get; set; }
    }
    public class ConditionViewModelDemo
    {
        public int ConditionId { get; set; }

        public string ConditionName { get; set; }
    }
}
