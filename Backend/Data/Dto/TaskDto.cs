using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PortableManager.Web.Server.Models.Dto
{
    public class TaskDto
    {
        public int Id { get; set; }
        public int TaskTypeId { get; set; }
        
        [ForeignKey("TaskTypeId")]
        public TaskTypeDto TaskType { get; set; }
        public string Text { get; set; }
        public bool Status { get; set; }
    
        public static void Map(ref TaskDto taskDto, Models.Task task)
        {
            taskDto.Id = task.Id;
            taskDto.TaskTypeId = task.TaskTypeId;
            taskDto.Text = task.Text;
            taskDto.Status = task.Status;
        }
    }
}
