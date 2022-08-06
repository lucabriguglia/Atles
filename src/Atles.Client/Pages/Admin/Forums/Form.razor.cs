using Atles.Client.Components.Admin;
using Atles.Models.Admin.Forums;
using Microsoft.AspNetCore.Components;

namespace Atles.Client.Pages.Admin.Forums;

public abstract class FormComponent : AdminFormBase
{
    [Parameter] public ForumFormModel Model { get; set; }
    [Parameter] public string SubmitButtonText { get; set; }
}
