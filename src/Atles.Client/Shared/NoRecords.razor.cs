using Microsoft.AspNetCore.Components;
using Atles.Client.Components.Shared;

namespace Atles.Client.Shared
{
    public abstract class NoRecordsComponent : SharedComponentBase
    {
        [Parameter] public string Text { get; set; }

        protected override void OnInitialized()
        {
            if (string.IsNullOrWhiteSpace(Text))
            {
                Text = Loc["No records found."];
            }
        }
    }
}