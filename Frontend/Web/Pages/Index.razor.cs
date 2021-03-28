using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using PortableManager.Web.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace PortableManager.Web.Client.Pages
{
    public class IndexModel : ComponentBase
    {
        [Parameter] public TaskType CurrentTaskType { get; set; }
        [Parameter] public bool RerenderTaskTypes { get; set; }
        [Parameter] public List<Models.Task> Tasks { get; set; }
        [Parameter] public List<TaskType> TaskTypes { get; set; }
    }
}
