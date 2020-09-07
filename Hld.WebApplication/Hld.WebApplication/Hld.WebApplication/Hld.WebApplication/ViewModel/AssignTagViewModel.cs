using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class AssignTagViewModel
    {
        public string SKu { get; set; }
        public List<TagsList> tags { get; set; }
    }
    public class TagsList
    {
        public int TagId { get; set; }
    }
}
