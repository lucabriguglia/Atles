using Atles.Client.Pages.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace Atles.Client.Components.Admin
{
    [Authorize(Policy = "Admin")]
    [Layout(typeof(AdminLayout))]
    public abstract class AdminPageBase : AdminComponentBase
    {
        protected string ApiUrl(string action) => $"api/admin/{action}";
    }
}