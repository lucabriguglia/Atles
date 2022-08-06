using Microsoft.AspNetCore.Components;

namespace Atles.Client.Components.Admin;

public abstract class AdminFormBase : AdminComponentBase
{
    [Parameter] public EventCallback OnSubmit { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }
}