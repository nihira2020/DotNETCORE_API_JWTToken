using System;
using System.Collections.Generic;

namespace CustomerAPI.Models
{
    public partial class TblRefreshtoken
    {
        public string UserId { get; set; }
        public string TokenId { get; set; }
        public string RefreshToken { get; set; }
        public bool? IsActive { get; set; }
    }
}
