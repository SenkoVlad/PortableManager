using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortableManager.Web.Server.Data.Repositories.Interface
{
    public interface ITaskRepository
    {
        Task<Models.Task[]> GetAllTaskByTaskTypeIdAsync(int? taskTypeId);
        Task<Models.Task[]> GetAllTasksAsync();
        Task<Models.Task> GetTaskByIdAsync(int id);
        Task<Models.Task> GetTaskByNameAsync(string taskName);
        Task<Models.Task> AddTaskAsync(Models.Task task);
        Task DeleteTaskAsync(Models.Task task);
        Task UpdateTaskAsync(Models.Task task);
    }
}
