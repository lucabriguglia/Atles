using System;
using System.Threading.Tasks;
using Atlas.Models.Public.Users;
using Microsoft.AspNetCore.Components;

namespace Atlas.Client.Components.Themes
{
    public abstract class UserComponent : ThemeComponentBase
    {
        [Parameter] public Guid? Id { get; set; }

        protected UserPageModel Model { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var requestUri = Id == null ? "api/public/users" : $"api/public/users/{Id}";
            Model = await ApiService.GetFromJsonAsync<UserPageModel>(requestUri);
        }
    }
}