using System;
using System.Threading.Tasks;
using Atlas.Models.Admin.Forums;
using Microsoft.AspNetCore.Components;

namespace Atlas.Client.Components.Admin.Forums
{
    public abstract class CreatePage : AdminPageBase
    {
        [Parameter] public Guid? CategoryId { get; set; }

        protected FormComponentModel Model { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var requestUri = CategoryId == null
                ? "api/admin/forums/create"
                : $"api/admin/forums/create/{CategoryId}";

            Model = await ApiService.GetFromJsonAsync<FormComponentModel>(requestUri);
        }

        protected async Task SaveAsync()
        {
            await ApiService.PostAsJsonAsync("api/admin/forums/save", Model.Forum);
            NavigationManager.NavigateTo($"/admin/forums/{Model.Forum.CategoryId}");
        }
    }
}