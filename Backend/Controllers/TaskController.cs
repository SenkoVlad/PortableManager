using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PortableManager.Web.Server.Models;
using PortableManager.Web.Server.Servicies;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortableManager.Web.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly TaskTypeService _taskTypeService;
        private readonly TaskService _taskService;

        private readonly ILogger<TaskController> _logger;
        public TaskController(ILogger<TaskController> logger, TaskTypeService taskTypeService, TaskService taskService)
        {
            _logger = logger;
            _taskTypeService = taskTypeService;
            _taskService = taskService;
        }

        [HttpGet("get/tasks/{id}")]
        public async Task<IEnumerable<Models.Task>> GetTask(int id)
        {
            return await _taskService.GetAllTaskByTaskTypeIdAsync(id);
        }
        [HttpGet("get/tasks")]
        public async Task<IEnumerable<Models.Task>> GetTask()
        {
            return await _taskService.GetAllTasksAsync();
        }

        [HttpPost("update/task")]
        public async Task<ActionResult<Models.Task>> UpdateTask(Models.Task task)
        {
            await _taskService.UpdateTaskAsync(task);
            return Ok();
        }
        
        [HttpPost("add/task")]
        public async Task<Models.Task> PostTask(Models.Task task)
        {
            return await _taskService.AddTaskAsync(task);
        }
        [HttpPost("delete/task")]
        public async Task<ActionResult<Models.Task>> DeleteTask(Models.Task task)
        {
            await _taskService.DeleteTaskAsync(task);
            return Ok();
        }


        [HttpGet("get/tasktypes")]
        public async Task<List<TaskType>> GetTaskType()
        {
            return await _taskTypeService.GetAllTaskTypeAsync();
        }

        [HttpGet("get/tasktype/{id}")]
        public async Task<ActionResult<TaskType>> GetTaskType(int id)
        {
            var taskType = await _taskTypeService.GetTaskTypeByIdAsync(id);
            return Ok(taskType);
        }

        [HttpPost("add/tasktype")]
        public async Task<TaskType> PostTaskType(TaskType taskType)
        {
            return await _taskTypeService.AddNewTaskTypeAsync(taskType);
        }

        [HttpPost("delete/tasktype")]
        public async Task<ActionResult<TaskType>> DeleteTaskType(TaskType taskType)
        {
            await _taskTypeService.DeleteTaskTypeAsync(taskType);
            return Ok();
        }

        [HttpPost("update/tasktype")]
        public async Task<int> UpdateTaskType(TaskType taskType)
        {
            return await _taskTypeService.UpdateTaskTypeAsync(taskType);
        }
    }
}
