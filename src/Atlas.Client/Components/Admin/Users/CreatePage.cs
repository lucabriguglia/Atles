using System;
using System.Text.Json;
using System.Threading.Tasks;
using Atlas.Models.Admin.Users;

namespace Atlas.Client.Components.Admin.Members
{
    public abstract class CreatePage : AdminPageBase
    {
        protected CreatePageModel Model { get; set; }
        protected bool Error { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Model = await ApiService.GetFromJsonAsync<CreatePageModel>("api/admin/members/create");
        }

        protected async Task SaveAsync()
        {
            var response = await ApiService.PostAsJsonAsync("api/admin/members/save", Model.User);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var memberId = JsonSerializer.Deserialize<Guid>(content);
                NavigationManager.NavigateTo($"/admin/members/edit/{memberId}");
            }
            else
            {
                Error = true;
            }
        }

        protected void Cancel()
        {
            NavigationManager.NavigateTo("/admin/members");
        }
    }
}