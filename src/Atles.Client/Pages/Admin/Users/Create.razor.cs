using System.Text.Json;
using Atles.Client.Components.Admin;
using Atles.Client.Models;
using Atles.Models;
using Atles.Models.Admin.Users;

namespace Atles.Client.Pages.Admin.Users;

public abstract class CreatePage : AdminPageBase
{
    protected CreateUserPageModel Model { get; set; }
    protected bool HasError { get; set; }
    protected string Error { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Model = await ApiService.GetFromJsonAsync<CreateUserPageModel>("api/admin/users/create");
    }

    protected async Task SaveAsync()
    {
        var response = await ApiService.PostAsJsonAsync("api/admin/users/save", Model.User);
        var content = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            var userId = JsonSerializer.Deserialize<Guid>(content);
            NavigationManager.NavigateTo($"/admin/users/edit/{userId}");
        }
        else
        {
            HasError = true;
            var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(content);
            Error = problemDetails?.Detail;
        }
    }

    protected void Cancel()
    {
        NavigationManager.NavigateTo("/admin/users");
    }
}
