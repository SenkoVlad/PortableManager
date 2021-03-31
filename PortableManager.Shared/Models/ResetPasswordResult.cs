using System;
using System.Collections.Generic;
using System.Text;

namespace PortableManager.Shared.Models
{
    public class ResetPasswordResult
    {
        public bool ShowMessages { get; set; }
        public bool  ShowEmail { get; set; }
        public IEnumerable<string> Messages { get; set; }
        public string Email { get; set; }
    }
}
