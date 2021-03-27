using PortableManager.Web.Server.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortableManager.Web.Server.Models
{
    public class TaskType
    {
        public int  Id { get; set; }
        public string Name { get; set; }

        public static TaskType Map(TaskTypeDto taskTypeDto)
        {
            return new TaskType()
            {
                Id = taskTypeDto.Id,
                Name = taskTypeDto.Name
            };
        }
    }
}
