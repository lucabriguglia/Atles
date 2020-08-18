using System;
using System.Threading.Tasks;
using Atlify.Models.Admin.Events;
using Microsoft.AspNetCore.Components;

namespace Atlify.Client.Components.Shared
{
    public abstract class TargetEventsComponent : SharedComponentBase
    {
        [Parameter] public Guid Id { get; set; }

        protected TargetEventsComponentModel Model { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            Model = await ApiService.GetFromJsonAsync<TargetEventsComponentModel>($"api/admin/events/target-model/{Id}");
        }
    }
}