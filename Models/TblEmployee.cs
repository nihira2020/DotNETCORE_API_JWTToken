using System;
using System.Collections.Generic;

namespace CustomerAPI.Models
{
    public partial class TblEmployee
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Designation { get; set; }
    }
}
