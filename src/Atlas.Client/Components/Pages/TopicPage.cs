using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Atlas.Models.Public;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Atlas.Client.Components.Pages
{
    public abstract class TopicPage : PageBase
    {
        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; set; }

        [Parameter] public Guid ForumId { get; set; }
        [Parameter] public Guid TopicId { get; set; }

        protected TopicPageModel Model { get; set; }

        protected bool DisplayPage { get; set; }
        protected ClaimsPrincipal CurrentUser { get; set; }
        protected string CurrentUserId { get; set; } = Guid.Empty.ToString();

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateTask;
            CurrentUser = authState.User;

            try
            {
                Model = await ApiService.GetFromJsonAsync<TopicPageModel>($"api/public/topics/{ForumId}/{TopicId}?page=1");

                DisplayPage = true;

                if (CurrentUser.Identity.IsAuthenticated)
                {
                    CurrentUserId = CurrentUser.Identities.First().Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
                }
            }
            catch (Exception)
            {
                Model = new TopicPageModel();
                DisplayPage = false;
            }
        }
    }
}