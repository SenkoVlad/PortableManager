using System;
using System.Collections.Generic;
using System.Text;

namespace PortableManager.Shared.Models
{
    public class ConfirmEmailModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
