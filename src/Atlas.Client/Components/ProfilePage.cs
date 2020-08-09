using System.Threading.Tasks;
using Atlas.Models.Public;

namespace Atlas.Client.Components
{
    public abstract class ProfilePage : PageBase
    {
        protected MemberPageModel Model { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Model = await ApiService.GetFromJsonAsync<MemberPageModel>("api/public/members");
        }
    }
}