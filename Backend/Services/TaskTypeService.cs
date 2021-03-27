using PortableManager.Web.Server.Data.Repositories.Implementation;
using PortableManager.Web.Server.Data.Repositories.Interface;
using PortableManager.Web.Server.Models;
using PortableManager.Web.Server.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortableManager.Web.Server.Servicies
{
    public class TaskTypeService
    {
        private ITaskTypeRepository _taskTypeRepository;
        public TaskTypeService(ITaskTypeRepository taskTypeRepository)
        {
            _taskTypeRepository = taskTypeRepository;
        }

        public async Task<List<TaskType>> GetAllTaskTypeAsync()
        {
            return await _taskTypeRepository.GetAllTaskTypeAsync();
        }

        public async Task<TaskType> GetTaskTypeByIdAsync(int id)
        {
            return await _taskTypeRepository.GetTaskTypeByIdAsync(id);
        }

        public async System.Threading.Tasks.Task<TaskType> AddNewTaskTypeAsync(TaskType taskType)
        {
             return await _taskTypeRepository.AddTaskTypeAsync(taskType);
        }

        public async System.Threading.Tasks.Task DeleteTaskTypeAsync(TaskType taskType)
        {
            await _taskTypeRepository.DeleteTaskTypeAsync(taskType);
        }

        public async System.Threading.Tasks.Task UpdateTaskTypeAsync(TaskType taskType)
        {
            await _taskTypeRepository.UpdateTaskTypeAsync(taskType);
        }
    }
}
