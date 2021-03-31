using System;
using System.Collections.Generic;
using System.Text;

namespace PortableManager.Shared.Models
{
    public class ForgotPasswordResult
    {
        public bool ShowForm { get; set; }
        public bool ShowMessages { get; set; }
        public IEnumerable<string> Messages { get; set; }
    }
}
