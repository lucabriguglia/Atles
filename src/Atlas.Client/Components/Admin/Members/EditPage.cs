using System;
using System.Threading.Tasks;
using Atlas.Models.Admin.Members;
using Microsoft.AspNetCore.Components;

namespace Atlas.Client.Components.Admin.Members
{
    public abstract class EditPage : AdminPageBase
    {
        [Parameter] public Guid Id { get; set; }

        protected EditPageModel Model { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            Model = await ApiService.GetFromJsonAsync<EditPageModel>($"api/admin/members/edit/{Id}");
        }

        protected async Task UpdateAsync()
        {
            await ApiService.PostAsJsonAsync("api/admin/members/update", Model);
            NavigationManager.NavigateTo("/admin/members");
        }
    }
}