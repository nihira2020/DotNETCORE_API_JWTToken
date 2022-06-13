using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerAPI.Entities
{
    public class Product
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public decimal? Amount { get; set; }
        public string ProductImage { get; set; }

    }
}
