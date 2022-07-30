using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Atles.Client.Components.Shared;
using Atles.Models.Admin.Events;

namespace Atles.Client.Shared
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