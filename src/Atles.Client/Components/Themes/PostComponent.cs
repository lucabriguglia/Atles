using System;
using System.Threading.Tasks;
using Atles.Client.Services;
using Atles.Models.Public;
using Microsoft.AspNetCore.Components;

namespace Atles.Client.Components.Themes
{
    public abstract class PostComponent : ThemeComponentBase
    {
        [Parameter] public Guid ForumId { get; set; }
        [Parameter] public Guid? TopicId { get; set; }

        protected PostPageModel Model { get; set; }

        protected string TitleText => TopicId != null ? Loc["Edit Topic"] : Loc["Create New Topic"];
        protected string ButtonText => TopicId != null ? Loc["Update"] : Loc["Save"];

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

        protected async Task SaveAsync()
        {
            var requestUri = TopicId != null
                ? "api/public/topics/update-topic"
                : "api/public/topics/create-topic";

            var response = await SaveDataAsync(() => ApiService.PostAsJsonAsync(requestUri, Model));
            var topicSlug = await response.Content.ReadAsStringAsync();

            Navigation.NavigateTo(Url.Topic(Model.Forum.Slug, topicSlug));
        }

        protected void Cancel()
        {
            var cancelUri = TopicId != null
                ? Url.Topic(Model.Forum.Slug, Model.Topic.Slug)
                : Url.Forum(Model.Forum.Slug);

            Navigation.NavigateTo(cancelUri);
        }
    }
}