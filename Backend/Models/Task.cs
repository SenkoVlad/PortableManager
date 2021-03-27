using PortableManager.Web.Server.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortableManager.Web.Server.Models
{
    public class Task
    {
        public int Id { get; set; }
        public int TaskTypeId { get; set; }
        public string Text { get; set; }
        public bool Status { get; set; }


        public static Task Map(TaskDto taskDto)
        {
            return new Task()
            {
                Id = taskDto.Id,
                TaskTypeId = taskDto.TaskTypeId,
                Status = taskDto.Status,
                Text = taskDto.Text
            };
        }
    }
}
