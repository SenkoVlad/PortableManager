using PortableManager.Web.Server.Models;
using PortableManager.Web.Server.Models.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace PortableManager.Web.Server.Data.Repositories.Interface
{
    public interface ITaskTypeRepository
    {
        Task<List<TaskType>> GetAllTaskTypeAsync();
        Task<TaskType> GetTaskTypeByIdAsync(int id);
        Task<TaskType> GetTaskTypeByNameAsync(string name);
        Task<TaskType> AddTaskTypeAsync(TaskType taskType);
        Task DeleteTaskTypeAsync(TaskType taskType);
        Task UpdateTaskTypeAsync(TaskType taskType);
    }
}
