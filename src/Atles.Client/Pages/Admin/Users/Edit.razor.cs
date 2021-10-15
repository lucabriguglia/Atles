using System;
using System.Threading.Tasks;
using Atles.Client.Components.Admin;
using Atles.Models.Admin.Users;
using Microsoft.AspNetCore.Components;

namespace Atles.Client.Pages.Admin.Users
{
    public abstract class EditPage : AdminPageBase
    {
        [Parameter] public Guid Id { get; set; }
        [Parameter] public string IdentityUserId { get; set; }

        protected EditPageModel Model { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            var requestUri = string.IsNullOrWhiteSpace(IdentityUserId)
                ? $"api/admin/users/edit/{Id}"
                : $"api/admin/users/edit-by-identity-user-id/{IdentityUserId}";

            Model = await ApiService.GetFromJsonAsync<EditPageModel>(requestUri);
        }

        protected async Task UpdateAsync()
        {
            await ApiService.PostAsJsonAsync("api/admin/users/update", Model);
            NavigationManager.NavigateTo("/admin/users");
        }
    }
}