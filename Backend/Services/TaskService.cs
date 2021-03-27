using PortableManager.Web.Server.Data.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortableManager.Web.Server.Servicies
{
    public class TaskService
    {
        private ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<Models.Task> AddTaskAsync(Models.Task task)
        {
            return await _taskRepository.AddTaskAsync(task);
        }
        public async Task<Models.Task[]> GetAllTaskByTaskTypeIdAsync(int? id)
        {
            return await _taskRepository.GetAllTaskByTaskTypeIdAsync(id);
        }
        public async Task<Models.Task[]> GetAllTasksAsync()
        {
            return await _taskRepository.GetAllTasksAsync();
        }

        public async Task UpdateTaskAsync(Models.Task task)
        {
            await _taskRepository.UpdateTaskAsync(task);
        }
        public async Task DeleteTaskAsync(Models.Task task)
        {
            await _taskRepository.DeleteTaskAsync(task);
        }
    }       
}
