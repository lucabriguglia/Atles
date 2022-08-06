using System;
using System.Threading.Tasks;
using Atles.Client.Components.Admin;
using Atles.Models.Admin.Forums;
using Microsoft.AspNetCore.Components;

namespace Atles.Client.Pages.Admin.Forums
{
    public abstract class CreatePage : AdminPageBase
    {
        [Parameter] public Guid? CategoryId { get; set; }

        protected CreateForumFormModel Model { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var requestUri = CategoryId == null
                ? "api/admin/forums/create"
                : $"api/admin/forums/create/{CategoryId}";

            Model = await ApiService.GetFromJsonAsync<CreateForumFormModel>(requestUri);
        }

        protected async Task SaveAsync()
        {
            await ApiService.PostAsJsonAsync("api/admin/forums/save", Model.Forum);
            NavigationManager.NavigateTo($"/admin/forums/{Model.Forum.CategoryId}");
        }
    }
}