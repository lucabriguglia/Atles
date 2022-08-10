using Microsoft.AspNetCore.Components;
using Atles.Client.Components.Shared;

namespace Atles.Client.Shared;

public abstract class SomethingWrongComponent : SharedComponentBase
{
    [Parameter] public string Error { get; set; }

    public string Message { get; set; }

    protected override void OnInitialized()
    {
        Message = !string.IsNullOrEmpty(Error) ? Error : Loc["Ops something went wrong."];
    }
}
