using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class GetNotesOrderViewModel
    {

        public int EntityID { get; set; }
        public string Note { get; set; }
        public string AuditDate { get; set; }
        public string CreatedByName { get; set; }
    }

    public class CreateOrderNotesViewModel
    {
        public int EntityID { get; set; }
        public int Category { get; set; }
        public int NoteID { get; set; }
        public string Note { get; set; }
        public DateTime? AuditDate { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public string CreatedByEmail { get; set; }
    }
}
