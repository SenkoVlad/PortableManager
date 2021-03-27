using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortableManager.Web.Client.Models
{
    public class Task
    {
        public int Id { get; set; }
        public int TaskTypeId { get; set; }
        public string Text { get; set; }
        public bool Status { get; set; }
    }
}
