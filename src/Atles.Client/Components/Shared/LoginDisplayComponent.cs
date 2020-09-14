using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Atles.Client.Components.Shared
{
    public abstract class LoginDisplayComponent : SharedComponentBase
    {
        [Inject] public SignOutSessionStateManager SignOutManager { get; set; }

        protected async Task BeginSignOut(MouseEventArgs args)
        {
            await SignOutManager.SetSignOutState();
            Navigation.NavigateTo("authentication/logout");
        }
    }
}