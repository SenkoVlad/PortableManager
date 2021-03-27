using Microsoft.EntityFrameworkCore;
using PortableManager.Web.Server.Data.Repositories.Interface;
using PortableManager.Web.Server.Models;
using PortableManager.Web.Server.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace PortableManager.Web.Server.Data.Repositories.Implementation
{
    public class TaskTypeRepository : ITaskTypeRepository
    {
        private DbContextFactory dbContext;
        public TaskTypeRepository(DbContextFactory context)
        {
            dbContext = context;
        }
        public async Task<List<TaskType>> GetAllTaskTypeAsync()
        {
            var Context = dbContext.Create(typeof(TaskTypeRepository));
            
            var taskType = await Context.TaskTypes.ToListAsync();
            return taskType.Select(TaskType.Map).ToList();
        }
        public async Task<TaskType> GetTaskTypeByIdAsync(int id)
        {
            var Context = dbContext.Create(typeof(TaskTypeRepository));

            TaskTypeDto taskTypeDto = await Context.TaskTypes.FirstOrDefaultAsync(item => item.Id == id);
            
            if (taskTypeDto != null)
                return TaskType.Map(taskTypeDto);

            return await Task.FromResult(TaskType.Map(new TaskTypeDto()
            {
                Id = 0,
                Name = ""
            }));
        }
        public async Task<TaskType> GetTaskTypeByNameAsync(string name)
        {
            var Context = dbContext.Create(typeof(TaskTypeRepository));

            TaskTypeDto taskTypeDto = await Context.TaskTypes.FirstOrDefaultAsync(item => item.Name == name.Trim());
            if (taskTypeDto != null)
                return TaskType.Map(taskTypeDto);
            
            return TaskType.Map(new TaskTypeDto());
        }
        public async Task<TaskType> AddTaskTypeAsync(TaskType taskType)
        {
            if(taskType != null && !string.IsNullOrWhiteSpace(taskType.Name))
            {
                var taskTypeName = await GetTaskTypeByNameAsync(taskType.Name);
                
                if(string.IsNullOrWhiteSpace(taskTypeName.Name))
                {
                    var Context = dbContext.Create(typeof(TaskTypeRepository));
                    var taskTypeDto = new TaskTypeDto() { Name = taskType.Name };

                    await Context.TaskTypes.AddAsync(taskTypeDto);
                    await Context.SaveChangesAsync();

                    return await Task.FromResult(TaskType.Map(taskTypeDto));
                }
            }

            return await Task.FromResult(new TaskType() {});
        }
        public async Task DeleteTaskTypeAsync(TaskType taskType)
        {
            if(taskType != null && !string.IsNullOrWhiteSpace(taskType.Name))
            {
                var Context = dbContext.Create(typeof(TaskTypeRepository));

                var taskTypeForDelete = await Context.TaskTypes.FirstOrDefaultAsync(item => item.Name == taskType.Name);
                if(taskTypeForDelete != null)
                    Context.TaskTypes.Remove(taskTypeForDelete);

                await Context.SaveChangesAsync();
            }
        }

        public async Task<int> UpdateTaskTypeAsync(TaskType taskType)
        {
            if(taskType != null && !string.IsNullOrWhiteSpace(taskType.Name))
            {
                var Context = dbContext.Create(typeof(TaskTypeRepository));

                var taskTypeForUpdate = await Context.TaskTypes.FirstOrDefaultAsync(item => item.Id == taskType.Id);
                if(taskTypeForUpdate != null)
                {
                    taskTypeForUpdate.Name = taskType.Name;
                    Context.Entry(taskTypeForUpdate).State = EntityState.Modified;
                    return await Context.SaveChangesAsync();
                }
            }

            return await Task.FromResult(0);
        }
    }
}
