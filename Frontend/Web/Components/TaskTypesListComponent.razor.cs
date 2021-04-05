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
    public class ListTaskKindsViewModel : ComponentBase
    {
        [Inject] HttpClient Http { get; set; }
        [Inject] IJSRuntime jsRuntime { get; set; }
        [Parameter] public List<TaskType> TaskTypes { get; set; }
        [Parameter] public EventCallback<List<TaskType>> TaskTypesChanged { get; set; }

        [Parameter] public List<Models.Task> Tasks { get; set; }
        [Parameter] public bool Rerender
        { 
            get
            {
                return false;
            }
            set 
            { 
                if (value == true) 
                    StateHasChanged();
            }
        } 
        [Parameter] public TaskType CurrentTaskType { get; set; }
        [Parameter] public EventCallback<TaskType> CurrentTaskTypeChanged { get; set; }

        public string inputTaskType { get; set; }

        public void setTaskTypeActive(TaskType taskType)
        {
            CurrentTaskType = taskType;
            CurrentTaskTypeChanged.InvokeAsync(CurrentTaskType);
        }
        protected override async Task OnInitializedAsync()
        {
            TaskTypes = (await Http.GetFromJsonAsync<List<TaskType>>( "task/get/tasktypes")).ToList();
            await TaskTypesChanged.InvokeAsync(TaskTypes);

            if(TaskTypes.Count > 0)
            {
                CurrentTaskType = TaskTypes.First();
                await CurrentTaskTypeChanged.InvokeAsync(CurrentTaskType);
            }
    
        }

        public async Task<HttpResponseMessage> AddNewTaskTypeAsync()
        {
            if (!string.IsNullOrWhiteSpace(inputTaskType))
            {
                var newTaskType = new TaskType() { Name = inputTaskType };
                var response = await Http.PostAsJsonAsync("task/add/tasktype", newTaskType);

                newTaskType = await GetTaskTypeFromHttpResponse(response);
                
                TaskTypes.Add(newTaskType);
                await TaskTypesChanged.InvokeAsync(TaskTypes);

                CurrentTaskType = newTaskType;
                await CurrentTaskTypeChanged.InvokeAsync(CurrentTaskType);
                inputTaskType = "";

                await jsRuntime.InvokeVoidAsync("showSuccessToast", $"added new taskType {newTaskType.Name}");
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
        }
        public async Task DeleteTaskTypeAsync(TaskType taskType)
        {
            await Http.PostAsJsonAsync( "task/delete/tasktype", taskType );
            TaskTypes.Remove(taskType);
            await TaskTypesChanged.InvokeAsync(TaskTypes);

            CurrentTaskType = TaskTypes.First();
            await CurrentTaskTypeChanged.InvokeAsync(CurrentTaskType);

            await jsRuntime.InvokeVoidAsync("showSuccessToast", $"deleted taskType {taskType.Name}");
        }

        public async Task UpdateTaskTypeAsync(ChangeEventArgs eventArgs,TaskType taskType)
        {
            var oldTaskTypeIndex = TaskTypes.IndexOf(taskType);

            taskType.Name = (string)eventArgs.Value;
            var responseMessage = await Http.PostAsJsonAsync( "task/update/tasktype", taskType);

            var responseBytes = await responseMessage.Content.ReadAsByteArrayAsync();
            var response = Encoding.UTF8.GetString(responseBytes);

            if (response == "1")
            {
                TaskTypes[oldTaskTypeIndex] = taskType;
                await TaskTypesChanged.InvokeAsync(TaskTypes);

                await jsRuntime.InvokeVoidAsync("showSuccessToast", $"updated taskType {taskType.Name}");
            }

        }

        private async  Task<TaskType> GetTaskTypeFromHttpResponse(HttpResponseMessage responseMessage)
        {
            byte[] responseByte = await responseMessage.Content.ReadAsByteArrayAsync();
            string responseString = Encoding.UTF8.GetString(responseByte);

            var a = new JsonSerializerOptions();
            a.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            return  JsonSerializer.Deserialize<TaskType>(responseString, a);
        }
    }
}
