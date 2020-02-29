using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Atlas.Framework
{
    public class BasePageModel : PageModel
    {
        private AtlasContext _atlasContext;

        public AtlasContext AtlasContext => _atlasContext ?? (_atlasContext = new AtlasContext());

        public string UserId
        {
            get
            {
                return User.Identity.IsAuthenticated 
                    ? User.Identities.First().Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value
                    : null;
            }
        }
    }
}
