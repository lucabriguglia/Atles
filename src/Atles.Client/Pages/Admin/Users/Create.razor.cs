using System.Text.Json;
using Atles.Client.Components.Admin;
using Atles.Models.Admin.Users;

namespace Atles.Client.Pages.Admin.Users;

public abstract class CreatePage : AdminPageBase
{
    protected CreatePageModel Model { get; set; }
    protected bool Error { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Model = await ApiService.GetFromJsonAsync<CreatePageModel>("api/admin/users/create");
    }

    protected async Task SaveAsync()
    {
        var response = await ApiService.PostAsJsonAsync("api/admin/users/save", Model.User);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var userId = JsonSerializer.Deserialize<Guid>(content);
            NavigationManager.NavigateTo($"/admin/users/edit/{userId}");
        }
        else
        {
            Error = true;
        }
    }

    protected void Cancel()
    {
        NavigationManager.NavigateTo("/admin/users");
    }
}
