using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Atles.Client.Components.Shared;

namespace Atles.Client.Shared
{
    public abstract class ConfirmModalComponent : SharedComponentBase
    {
        [Parameter] public string Id { get; set; } = "confirm-modal";
        [Parameter] public string Title { get; set; }
        [Parameter] public string Body { get; set; }
        [Parameter] public string Button { get; set; }
        [Parameter] public string CssClass { get; set; } = "danger";
        [Parameter] public EventCallback<MouseEventArgs> OnClickCallback { get; set; }
    }
}