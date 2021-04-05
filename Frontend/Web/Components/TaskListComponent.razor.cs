using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
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
        [Inject] IJSRuntime jsRuntime { get; set; }

        [Parameter] public TaskType TaskType { get; set; }
        [Parameter] public bool RerenderTaskTypesList { get; set; }
        [Parameter] public EventCallback<bool> RerenderTaskTypesListChanged { get; set; }


        [Parameter] public List<Models.TaskType> TaskTypes { get; set; }
        [Parameter] public List<Models.Task> Tasks { get; set; }
        [Parameter] public EventCallback<List<Models.Task>> TasksChanged { get; set; }


        public string InputTask { get; set; }

        protected async override Task OnInitializedAsync()
        {
            Tasks = (await Http.GetFromJsonAsync<List<Models.Task>>("task/get/tasks")).ToList();
            await TasksChanged.InvokeAsync(Tasks);
        }

        public async void UpdateTaskStatusAsync(Models.Task task, ChangeEventArgs eventArgs) 
        {
            task.Status = (bool)eventArgs.Value;
            Tasks[GetTaskIndexFromList(task)].Status = (bool)eventArgs.Value;
            await TasksChanged.InvokeAsync(Tasks);

            RerenderTaskTypesList = true;
            await RerenderTaskTypesListChanged.InvokeAsync(RerenderTaskTypesList);
            await Http.PostAsJsonAsync( "task/update/task", task);

            await jsRuntime.InvokeVoidAsync("showSuccessToast", $"update task status");
        }

        public async Task UpdateTaskTextAsync(Models.Task task, ChangeEventArgs eventArgs)
        {
            task.Text = (string)eventArgs.Value;
            await Http.PostAsJsonAsync( "task/update/task", task);
            await jsRuntime.InvokeVoidAsync("showSuccessToast", $"update task text");
        }

        public async Task MoveTaskAsync(TaskType taskType, Models.Task task)
        {
            task.TaskTypeId = taskType.Id;
            Tasks[GetTaskIndexFromList(task)].Id = taskType.Id;
            await TasksChanged.InvokeAsync(Tasks);

            RerenderTaskTypesList = true;
            await RerenderTaskTypesListChanged.InvokeAsync(RerenderTaskTypesList);
            await Http.PostAsJsonAsync("task/update/task", task);
            await jsRuntime.InvokeVoidAsync("showSuccessToast", $"moved task");
        }

        public async Task RemoveTaskAsync(Models.Task task)
        {
            Tasks.RemoveAt(GetTaskIndexFromList(task));
            await TasksChanged.InvokeAsync(Tasks);

            RerenderTaskTypesList = true;
            await RerenderTaskTypesListChanged.InvokeAsync(RerenderTaskTypesList);
            await Http.PostAsJsonAsync("task/delete/task", task);
            await jsRuntime.InvokeVoidAsync("showSuccessToast", $"removed task");
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
                    Tasks.Add(task);
                    await TasksChanged.InvokeAsync(Tasks);
                    await jsRuntime.InvokeVoidAsync("showSuccessToast", $"added task");
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
        private int GetTaskIndexFromList(Models.Task task)
        {
            return Tasks.FindIndex(item => item.Id == task.Id);
        }

    }
}
