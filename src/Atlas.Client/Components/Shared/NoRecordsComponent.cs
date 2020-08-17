using Microsoft.AspNetCore.Components;

namespace Atlas.Client.Components.Shared
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