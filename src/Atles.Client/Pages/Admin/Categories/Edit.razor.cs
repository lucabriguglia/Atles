using System;
using System.Threading.Tasks;
using Atles.Client.Components.Admin;
using Atles.Models.Admin.Categories;
using Microsoft.AspNetCore.Components;

namespace Atles.Client.Pages.Admin.Categories
{
    public abstract class EditPage : AdminPageBase
    {
        [Parameter] public Guid Id { get; set; }

        protected FormComponentModel Model { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            Model = await ApiService.GetFromJsonAsync<FormComponentModel>($"api/admin/categories/edit/{Id}");
        }

        protected async Task UpdateAsync()
        {
            await ApiService.PostAsJsonAsync("api/admin/categories/update", Model.Category);
            NavigationManager.NavigateTo("/admin/categories");
        }
    }
}