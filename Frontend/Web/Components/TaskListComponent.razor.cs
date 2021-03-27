using Microsoft.AspNetCore.Components;
using PortableManager.Web.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace PortableManager.Web.Client.Components
{
    public class TaskListViewModel : ComponentBase
    {
        [Inject] HttpClient Http { get; set; }
        [Parameter] public TaskType TaskType { get; set; }
        [Parameter] public bool RerenderTaskTypesList { get; set; }
        [Parameter] public EventCallback<bool> RerenderTaskTypesListChanged { get; set; }
        

        public string InputTask { get; set; }

        private int GetTaskIndexFromList(Models.Task task)
        {
            return ListTaskKindsViewModel.Tasks.FindIndex(item => item.Id == task.Id);
        }

        public async void UpdateTaskStatusAsync(Models.Task task, ChangeEventArgs eventArgs) 
        {
            task.Status = (bool)eventArgs.Value;
            ListTaskKindsViewModel.Tasks[GetTaskIndexFromList(task)].Status = (bool)eventArgs.Value;
            
            RerenderTaskTypesList = true;
            await RerenderTaskTypesListChanged.InvokeAsync(RerenderTaskTypesList);
            await Http.PostAsJsonAsync( "task/update/task", task);
        }

        public async Task UpdateTaskTextAsync(Models.Task task, ChangeEventArgs eventArgs)
        {
            task.Text = (string)eventArgs.Value;
            await Http.PostAsJsonAsync( "task/update/task", task);
        }

        public async Task MoveTaskAsync(TaskType taskType, Models.Task task)
        {
            task.TaskTypeId = taskType.Id;
            ListTaskKindsViewModel.Tasks[GetTaskIndexFromList(task)].Id = taskType.Id;
            
            RerenderTaskTypesList = true;
            await RerenderTaskTypesListChanged.InvokeAsync(RerenderTaskTypesList);
            await Http.PostAsJsonAsync("task/update/task", task);
        }

        public async Task RemoveTaskAsync(Models.Task task)
        {
            ListTaskKindsViewModel.Tasks.RemoveAt(GetTaskIndexFromList(task));
            
            RerenderTaskTypesList = true;
            await RerenderTaskTypesListChanged.InvokeAsync(RerenderTaskTypesList);
            await Http.PostAsJsonAsync("task/delete/task", task);
        }

        public async Task AddTaskAsync()
        {
            if(TaskType != null)
            {
                var task = new Models.Task()
                {
                    Status = false,
                    Text = InputTask,
                    TaskTypeId = TaskType.Id
                };
                var resultPostTask = await Http.PostAsJsonAsync( "task/add/task", task);

                if ((await GetTaskFromHttpResponse(resultPostTask)).Id != 0) 
                { 
                    ListTaskKindsViewModel.Tasks.Add(task);
                }

                RerenderTaskTypesList = true;
                await RerenderTaskTypesListChanged.InvokeAsync(RerenderTaskTypesList);
                InputTask = "";
            }
        }

        private async Task<Models.Task> GetTaskFromHttpResponse(HttpResponseMessage responseMessage)
        {
            byte[] responseByte = await responseMessage.Content.ReadAsByteArrayAsync();
            string responseString = Encoding.UTF8.GetString(responseByte);

            var a = new JsonSerializerOptions();
            a.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            return JsonSerializer.Deserialize<Models.Task>(responseString, a);
        }
    }
}
