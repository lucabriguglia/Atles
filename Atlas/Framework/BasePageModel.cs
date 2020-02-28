using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Atlas.Framework
{
    public class BasePageModel : PageModel
    {
        private AtlasContext _atlasContext;

        public AtlasContext AtlasContext => _atlasContext ?? (_atlasContext = new AtlasContext());
    }
}
