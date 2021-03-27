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
        public static List<TaskType> TaskTypes { get; set; } = new List<TaskType>();
        public static List<Models.Task> Tasks { get; set; } = new List<Models.Task>();

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
            Console.WriteLine("ListTaskKindsViewModel OnInitializedAsync");

            TaskTypes = (await Http.GetFromJsonAsync<List<TaskType>>( "task/get/tasktypes")).ToList();
            Tasks = (await Http.GetFromJsonAsync<Models.Task[]>("task/get/tasks")).ToList();

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
                
                CurrentTaskType = newTaskType;
                await CurrentTaskTypeChanged.InvokeAsync(CurrentTaskType);
                inputTaskType = "";
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
        }
        public async Task DeleteTaskTypeAsync(TaskType taskType)
        {
            await Http.PostAsJsonAsync( "task/delete/tasktype", taskType );
            TaskTypes.Remove(taskType);

            CurrentTaskType = TaskTypes.First();
            await CurrentTaskTypeChanged.InvokeAsync(CurrentTaskType);
        }

        public async Task UpdateTaskTypeAsync(ChangeEventArgs eventArgs,TaskType taskType)
        {
            taskType.Name = (string)eventArgs.Value;
            await Http.PostAsJsonAsync( "task/update/tasktype", taskType);
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
