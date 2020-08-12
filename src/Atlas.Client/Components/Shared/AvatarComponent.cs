using Microsoft.AspNetCore.Components;

namespace Atlas.Client.Components.Shared
{
    public abstract class AvatarComponent : SharedComponentBase
    {
        [Parameter] public string GravatarHash { get; set; }
        [Parameter] public int GravatarSize { get; set; }
        [Parameter] public string Class { get; set; }
        [Parameter] public bool CurrentMember { get; set; }

        protected override void OnInitialized()
        {
            if (CurrentMember)
            {
                GravatarHash = Member.GravatarHash;
            }
        }
    }
}