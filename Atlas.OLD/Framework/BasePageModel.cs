using System.Linq;
using System.Security.Claims;
using Atlas.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Atlas.Framework
{
    public abstract class BasePageModel : PageModel
    {
        protected readonly AtlasDbContext DbContext;

        private AtlasContext _atlasContext;

        public AtlasContext AtlasContext => _atlasContext ?? (_atlasContext = new AtlasContext());

        protected BasePageModel(AtlasDbContext dbContext)
        {
            DbContext = dbContext;
        }

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
