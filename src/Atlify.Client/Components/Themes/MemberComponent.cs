using System;
using System.Threading.Tasks;
using Atlify.Models.Public.Members;
using Microsoft.AspNetCore.Components;

namespace Atlify.Client.Components.Themes
{
    public abstract class MemberComponent : ThemeComponentBase
    {
        [Parameter] public Guid? Id { get; set; }

        protected MemberPageModel Model { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var requestUri = Id == null ? "api/public/members" : $"api/public/members/{Id}";
            Model = await ApiService.GetFromJsonAsync<MemberPageModel>(requestUri);
        }
    }
}