using Microsoft.EntityFrameworkCore;
using PortableManager.Web.Server.Data.Repositories.Interface;
using PortableManager.Web.Server.Models.Dto;
using System.Linq;
using System.Threading.Tasks;

namespace PortableManager.Web.Server.Data.Repositories.Implementation
{
    public class TaskRepository : ITaskRepository
    {
        private DbContextFactory dbContext;
        public TaskRepository(DbContextFactory context)
        {
            dbContext = context;
        }

        public async Task<Models.Task> AddTaskAsync(Models.Task task)
        {
            if (task != null && !string.IsNullOrWhiteSpace(task.Text))
            {
                if (!(await TaskExists(task)))
                {
                    var Context = dbContext.Create(typeof(TaskRepository));
                    var taskDto = new TaskDto();

                    TaskDto.Map(ref taskDto, task);
                    await Context.Tasks.AddAsync(taskDto);
                    await Context.SaveChangesAsync();

                    return await Task.FromResult(Models.Task.Map(taskDto));
                }
            }
            return await Task.FromResult(Models.Task.Map(new TaskDto()));

        }

        public async Task<bool> TaskExists(Models.Task task)
        {
            var Context = dbContext.Create(typeof(TaskRepository));

            var taskEntity = await Context.Tasks.FirstOrDefaultAsync(item => item.TaskTypeId == task.TaskTypeId && item.Text == task.Text);
            if(taskEntity != null)
                return true;

            return false;
        } 
        public async Task<Models.Task> GetTaskByIdAsync(int id)
        {
            var Context = dbContext.Create(typeof(TaskRepository));

            TaskDto taskDto = await Context.Tasks.FirstOrDefaultAsync(item => item.Id == id);
            if (taskDto != null)
                return Models.Task.Map(taskDto);

            return Models.Task.Map(new TaskDto());
        }


        public async System.Threading.Tasks.Task DeleteTaskAsync(Models.Task task)
        {
            if (task != null && !string.IsNullOrWhiteSpace(task.Text))
            {
                var Context = dbContext.Create(typeof(TaskRepository));

                var taskForDelete = await Context.Tasks.FirstOrDefaultAsync(item => item.Id == task.Id);
                if (taskForDelete != null)
                    Context.Tasks.Remove(taskForDelete);

                await Context.SaveChangesAsync();
            }
        }

        public async Task<Models.Task[]> GetAllTaskByTaskTypeIdAsync(int? taskTypeId)
        {
            var Context = dbContext.Create(typeof(TaskRepository));
            TaskDto[] tasks = await Context.Tasks.Where(item => item.TaskTypeId == taskTypeId).ToArrayAsync();

            return tasks.Select(Models.Task.Map)
                        .ToArray();
        }

        public async Task<Models.Task> GetTaskByNameAsync(string taskText)
        {
            var Context = dbContext.Create(typeof(TaskRepository));

            TaskDto taskDto = await Context.Tasks.FirstOrDefaultAsync(item => item.Text == taskText.Trim());
            if (taskDto != null)
                return Models.Task.Map(taskDto);

            return Models.Task.Map(new TaskDto());
        }

        public async System.Threading.Tasks.Task UpdateTaskAsync(Models.Task task)
        {
            if (task != null && !string.IsNullOrWhiteSpace(task.Text))
            {
                var Context = dbContext.Create(typeof(TaskRepository));

                var taskForUpdate = await Context.Tasks.FirstOrDefaultAsync(item => item.Id == task.Id);
                if (taskForUpdate != null)
                {
                    TaskDto.Map(ref taskForUpdate, task);
                    //Context.Tasks.Update(taskForUpdate);
                    Context.Entry(taskForUpdate).State = EntityState.Modified;
                    await Context.SaveChangesAsync();
                }
            }
        }

        public async Task<Models.Task[]> GetAllTasksAsync()
        {
            var Context = dbContext.Create(typeof(TaskRepository));
            TaskDto[] tasks = await Context.Tasks.ToArrayAsync();

            return tasks.Select(Models.Task.Map)
                        .ToArray();
        }
    }
}
