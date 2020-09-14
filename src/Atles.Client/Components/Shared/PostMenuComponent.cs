using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Atles.Client.Components.Shared
{
    public abstract class PostMenuComponent : SharedComponentBase
    {
        [Parameter] public Guid PostId { get; set; }

        [Parameter] public bool CanAcceptAnswer { get; set; }
        [Parameter] public bool CanRemoveAnswer { get; set; }
        [Parameter] public bool CanEdit { get; set; }
        [Parameter] public bool CanDelete { get; set; }

        [Parameter] public EventCallback<MouseEventArgs> AcceptAnswerCallback { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> RemoveAnswerCallback { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> EditCallback { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> DeleteCallback { get; set; }

        protected bool Display { get; set; }

        protected override void OnInitialized()
        {
            Display = CanAcceptAnswer || CanRemoveAnswer || CanEdit || CanDelete;
        }
    }
}