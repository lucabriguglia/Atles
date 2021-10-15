using Microsoft.AspNetCore.Components;
using Atles.Client.Components.Shared;

namespace Atles.Client.Shared
{
    public abstract class AvatarComponent : SharedComponentBase
    {
        [Parameter] public string GravatarHash { get; set; }
        [Parameter] public int GravatarSize { get; set; }
        [Parameter] public string Class { get; set; }
        [Parameter] public bool CurrentUser { get; set; }

        protected override void OnInitialized()
        {
            if (CurrentUser)
            {
                GravatarHash = User.GravatarHash;
            }
        }
    }
}