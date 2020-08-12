using Microsoft.AspNetCore.Components;

namespace Atlas.Client.Components.Shared
{
    public abstract class NoRecordsComponent : SharedComponentBase
    {
        [Parameter] public string Text { get; set; }
    }
}