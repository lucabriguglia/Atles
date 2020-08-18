using Atlify.Client.Pages.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace Atlify.Client.Components.Admin
{
    [Authorize(Policy = "Admin")]
    [Layout(typeof(AdminLayout))]
    public abstract class AdminPageBase : AdminComponentBase
    {
    }
}