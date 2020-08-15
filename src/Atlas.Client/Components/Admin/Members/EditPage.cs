using System;
using System.Threading.Tasks;
using Atlas.Models.Admin.Members;
using Microsoft.AspNetCore.Components;

namespace Atlas.Client.Components.Admin.Members
{
    public abstract class EditPage : AdminPageBase
    {
        [Parameter] public Guid Id { get; set; }
        [Parameter] public string UserId { get; set; }

        protected EditPageModel Model { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            var requestUri = string.IsNullOrWhiteSpace(UserId)
                ? $"api/admin/members/edit/{Id}"
                : $"api/admin/members/edit-by-user-Id/{UserId}";

            Model = await ApiService.GetFromJsonAsync<EditPageModel>(requestUri);
        }

        protected async Task UpdateAsync()
        {
            await ApiService.PostAsJsonAsync("api/admin/members/update", Model);
            NavigationManager.NavigateTo("/admin/members");
        }
    }
}