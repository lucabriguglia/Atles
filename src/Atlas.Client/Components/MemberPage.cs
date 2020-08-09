using System;
using System.Threading.Tasks;
using Atlas.Models.Public;
using Microsoft.AspNetCore.Components;

namespace Atlas.Client.Components
{
    public abstract class MemberPage : PageBase
    {
        [Parameter]
        public Guid Id { get; set; }

        protected MemberPageModel Model { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Model = await ApiService.GetFromJsonAsync<MemberPageModel>($"api/public/members/{Id}");
        }
    }
}