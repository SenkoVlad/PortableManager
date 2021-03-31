using System;
using System.Collections.Generic;
using System.Text;

namespace PortableManager.Shared.Models
{
    public class ConfirmEmailResult
    {
        public bool Status { get; set; }
        public IEnumerable<string> Messages { get; set; }
    }
}
