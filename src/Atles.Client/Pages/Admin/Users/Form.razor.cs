using Atles.Client.Components.Admin;
using Atles.Models.Admin.Users;
using Microsoft.AspNetCore.Components;

namespace Atles.Client.Pages.Admin.Users;

public abstract class FormComponent : AdminFormBase
{
    [Parameter] public EditUserPageModel.UserModel Model { get; set; }
    [Parameter] public string SubmitButtonText { get; set; }
}
