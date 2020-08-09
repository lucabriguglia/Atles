using System;
using System.Threading.Tasks;
using Atlas.Models.Public;
using Microsoft.AspNetCore.Components;

namespace Atlas.Client.Components.Pages
{
    public abstract class PostPage : PageBase
    {
        [Parameter]
        public Guid ForumId { get; set; }

        [Parameter]
        public Guid? TopicId { get; set; }

        protected PostPageModel Model { get; set; }

        protected bool DisplayPage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            var requestUri = TopicId != null
                ? $"api/public/topics/{ForumId}/edit-topic/{TopicId.Value}"
                : $"api/public/topics/{ForumId}/new-topic";

            try
            {
                Model = await ApiService.GetFromJsonAsync<PostPageModel>(requestUri);
                DisplayPage = true;
            }
            catch (Exception)
            {
                Model = new PostPageModel();
                DisplayPage = false;
            }
        }
    }
}