using System;
using System.Threading.Tasks;
using Atlas.Models.Public;
using Microsoft.AspNetCore.Components;

namespace Atlas.Client.Components.Pages
{
    public abstract class ForumPage : PageBase
    {
        [Parameter]
        public Guid Id { get; set; }

        protected ForumPageModel Model { get; set; }

        protected bool DisplayPage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Model = await ApiService.GetFromJsonAsync<ForumPageModel>($"api/public/forums/{Id}?page=1");
                DisplayPage = true;
            }
            catch (Exception)
            {
                Model = new ForumPageModel();
                DisplayPage = false;
            }
        }
    }
}