using Atles.Client.Components.Admin;
using Atles.Models.Admin.Categories;
using Microsoft.AspNetCore.Components;

namespace Atles.Client.Pages.Admin.Categories;

public abstract class FormComponent : AdminFormBase
{
    [Parameter] public CategoryFormModel Model { get; set; }
    [Parameter] public string SubmitButtonText { get; set; }
}