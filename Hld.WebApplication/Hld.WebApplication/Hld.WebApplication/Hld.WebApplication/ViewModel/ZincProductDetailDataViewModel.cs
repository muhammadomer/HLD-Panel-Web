using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ZincProductDetailDataViewModel
    {

        public class Width
        {
            public double amount { get; set; }
            public string unit { get; set; }
        }

        public class Depth
        {
            public double amount { get; set; }
            public string unit { get; set; }
        }

        public class Length
        {
            public double amount { get; set; }
            public string unit { get; set; }
        }

        public class Size
        {
            public Width width { get; set; }
            public Depth depth { get; set; }
            public Length length { get; set; }
        }

        public class Weight
        {
            public double amount { get; set; }
            public string unit { get; set; }
        }

        public class PackageDimensions
        {
            public Size size { get; set; }
            public Weight weight { get; set; }
        }

        public class VariantSpecific
        {
            public string dimension { get; set; }
            public string value { get; set; }
        }

        public class AllVariant
        {
            public string product_id { get; set; }
            public List<VariantSpecific> variant_specifics { get; set; }
        }

        public class VariantSpecific2
        {
            public string dimension { get; set; }
            public string value { get; set; }
        }

        public class Epid
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class EpidsMap
        {
            public string MPN { get; set; }
        }

        public class RootObject
        {
            public int timestamp { get; set; }
            public string status { get; set; }
            public List<string> feature_bullets { get; set; }
            public string title { get; set; }
            public string html_product_description { get; set; }
            public string product_description { get; set; }
            public List<string> images { get; set; }
            public string main_image { get; set; }
            public List<string> product_details { get; set; }
            public string brand { get; set; }
            public List<string> categories { get; set; }
            public PackageDimensions package_dimensions { get; set; }
            public List<AllVariant> all_variants { get; set; }
            public string tag_title { get; set; }
            public List<VariantSpecific2> variant_specifics { get; set; }
            public string asin { get; set; }
            public string product_id { get; set; }
            public string retailer { get; set; }
            public double stars { get; set; }
            public int review_count { get; set; }
            public int question_count { get; set; }
            public bool fresh { get; set; }
            public bool pantry { get; set; }
            public int price { get; set; }
            public int ship_price { get; set; }
            public List<Epid> epids { get; set; }
            public EpidsMap epids_map { get; set; }
        }
    }
}
