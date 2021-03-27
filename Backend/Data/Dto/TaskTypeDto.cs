using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortableManager.Web.Server.Models.Dto
{
    public class TaskTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public static TaskTypeDto Map(TaskType taskType)
        {
            return new TaskTypeDto()
            {
                Id = taskType.Id,
                Name = taskType.Name
            };
        }
    }
}
