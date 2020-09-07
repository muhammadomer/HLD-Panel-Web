using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class PostDataViewModel
    {
        public int? idPostEditor { get; set; }
        [Required(ErrorMessage = "Please enter your title first!")]
        public string posttitle { get; set; }
        [Required(ErrorMessage = "Please enter your data!")]
        public string postdata { get; set; }
        [DataType(DataType.Date)]
        public DateTime? Post_date { get; set; }
        [Required(ErrorMessage = "Please select your catagory first!")]
        public int catagoryid { get; set; }
        public List<PostData> listdatacatagory { get; set; }

    }
    public class PostData
    {
        public string catagory { get; set; }
        public int catagoryid { get; set; }
    }
}
