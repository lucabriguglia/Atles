using System;
using System.Threading.Tasks;
using Atlify.Models.Admin.Forums;
using Microsoft.AspNetCore.Components;

namespace Atlify.Client.Components.Admin.Forums
{
    public abstract class EditPage : AdminPageBase
    {
        [Parameter] public Guid Id { get; set; }

        protected FormComponentModel Model { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            Model = await ApiService.GetFromJsonAsync<FormComponentModel>($"api/admin/forums/edit/{Id}");
        }

        protected async Task UpdateAsync()
        {
            await ApiService.PostAsJsonAsync($"api/admin/forums/update", Model.Forum);
            NavigationManager.NavigateTo($"/admin/forums/{Model.Forum.CategoryId}");
        }
    }
}