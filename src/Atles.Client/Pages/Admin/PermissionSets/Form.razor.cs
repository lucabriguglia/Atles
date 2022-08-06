using Atles.Client.Components.Admin;
using Atles.Models.Admin.PermissionSets;
using Microsoft.AspNetCore.Components;

namespace Atles.Client.Pages.Admin.PermissionSets;

public abstract class FormComponent : AdminFormBase
{
    [Parameter] public PermissionSetFormModel Model { get; set; }
    [Parameter] public string SubmitButtonText { get; set; }
}
