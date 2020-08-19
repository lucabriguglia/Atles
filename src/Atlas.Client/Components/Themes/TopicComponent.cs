using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Atlas.Client.Services;
using Atlas.Models;
using Atlas.Models.Public.Topics;
using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Atlas.Client.Components.Themes
{
    public abstract class TopicComponent : ThemeComponentBase
    {
        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; set; }

        [Parameter] public string ForumSlug { get; set; }
        [Parameter] public string TopicSlug { get; set; }

        protected TopicPageModel Model { get; set; }

        protected string PinButtonText => Model.Topic.Pinned
            ? Loc["Unpin"]
            : Loc["Pin"];

        protected string LockButtonText => Model.Topic.Locked
            ? Loc["Unlock"]
            : Loc["Lock"];

        protected string ReplyTitleText => Model.Post.Id != null
            ? Loc["Edit Reply"]
            : Loc["New Reply"];

        protected string ReplyButtonText => Model.Post.Id != null
            ? Loc["Update"]
            : Loc["Save"];

        protected string NoRecordsText => string.IsNullOrWhiteSpace(Search)
            ? Loc["No replies found for this topic."]
            : Loc["No replies found for current search."];

        protected Guid DeleteReplyId { get; set; }

        protected ClaimsPrincipal CurrentUser { get; set; }
        protected string CurrentUserId { get; set; } = Guid.Empty.ToString();

        protected string Search { get; set; }
        protected int CurrentPage { get; set; } = 1;
        protected bool DisplayPage { get; set; }
        protected bool EditingAnswer { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateTask;
            CurrentUser = authState.User;

            try
            {
                Model = await ApiService.GetFromJsonAsync<TopicPageModel>($"api/public/topics/{ForumSlug}/{TopicSlug}?page=1");

                DisplayPage = true;

                if (CurrentUser.Identity.IsAuthenticated)
                {
                    CurrentUserId = CurrentUser.Identities.First().Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
                }
            }
            catch (Exception)
            {
                Model = new TopicPageModel();
                DisplayPage = false;
            }
        }

        private async Task LoadDataAsync()
        {
            Model.Replies = await ApiService.GetFromJsonAsync<PaginatedData<TopicPageModel.ReplyModel>>($"api/public/topics/{Model.Forum.Id}/{Model.Topic.Id}/replies?page={CurrentPage}&search={Search}");
        }

        protected async Task SearchAsync()
        {
            CurrentPage = 1;
            Model.Replies = null;
            await LoadDataAsync();
        }

        protected async Task ClearSearchAsync()
        {
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = string.Empty;
                CurrentPage = 1;
                Model.Replies = null;
                await LoadDataAsync();
            }
        }

        protected async Task MyKeyUpAsync(KeyboardEventArgs key)
        {
            if (key.Code == "Enter")
            {
                await SearchAsync();
            }
        }

        protected async Task ChangePageAsync(int page)
        {
            await JsRuntime.InvokeVoidAsync("scrollToTarget", "replies");
            CurrentPage = page;
            Model.Replies = null;
            await LoadDataAsync();
        }

        protected void SetDeleteReplyId(Guid id)
        {
            DeleteReplyId = id;
        }

        protected async Task NewReplyAsync()
        {
            await JsRuntime.InvokeVoidAsync("scrollToTarget", "reply");
        }

        protected async Task PinAsync()
        {
            await ApiService.PostAsJsonAsync($"api/public/topics/pin-topic/{Model.Forum.Id}/{Model.Topic.Id}", !Model.Topic.Pinned);
            Model.Topic.Pinned = !Model.Topic.Pinned;
        }

        protected async Task LockAsync()
        {
            await ApiService.PostAsJsonAsync($"api/public/topics/lock-topic/{Model.Forum.Id}/{Model.Topic.Id}", !Model.Topic.Locked);
            Model.Topic.Locked = !Model.Topic.Locked;
        }

        protected async Task EditReplyAsync(Guid id, string content, Guid memberId, bool isAnswer = false)
        {
            EditingAnswer = isAnswer;
            Model.Post.Id = id;
            Model.Post.Content = content;
            Model.Post.MemberId = memberId;
            await JsRuntime.InvokeVoidAsync("scrollToTarget", "reply");
        }

        protected async Task SaveReplyAsync()
        {
            var requestUri = Model.Post.Id != null
                ? "api/public/replies/update-reply"
                : "api/public/replies/create-reply";

            await SaveDataAsync(() => ApiService.PostAsJsonAsync(requestUri, Model));

            if (Model.Post.Id != null)
            {
                await JsRuntime.InvokeVoidAsync("scrollToTarget", Model.Post.Id.Value);
            }

            if (Model.Post.Id == null)
            {
                CurrentPage = Model.Replies.TotalPages;
            }

            if (!EditingAnswer)
            {
                Model.Replies = null;
                await LoadDataAsync();
            }
            else
            {
                Model.Answer.Content = Markdown.ToHtml(Model.Post.Content);
                Model.Answer.OriginalContent = Model.Post.Content;
            }

            Model.Post = new TopicPageModel.PostModel();
        }

        protected async Task DeleteTopicAsync(MouseEventArgs e)
        {
            await ApiService.DeleteAsync($"api/public/topics/delete-topic/{Model.Forum.Id}/{Model.Topic.Id}");
            Navigation.NavigateTo(Url.Forum(Model.Forum.Slug));
        }

        protected async Task DeleteReplyAsync(MouseEventArgs e)
        {
            await ApiService.DeleteAsync($"api/public/replies/delete-reply/{Model.Forum.Id}/{Model.Topic.Id}/{DeleteReplyId}");
            await LoadDataAsync();
        }

        protected void Cancel()
        {
            Model.Post.Id = null;
            Model.Post.Content = null;
            Model.Topic.MemberId = Guid.Empty;
        }

        protected async Task SetAnswerAsync(Guid replyId, bool isAnswer)
        {
            Model.Replies = null;
            await ApiService.PostAsJsonAsync($"api/public/replies/set-reply-as-answer/{Model.Forum.Id}/{Model.Topic.Id}/{replyId}", isAnswer);
            await OnInitializedAsync();
        }

        protected bool CanEditTopic()
        {
            return Model.CanEdit && Model.Topic.UserId == CurrentUserId && !Model.Topic.Locked || Model.CanModerate;
        }

        protected bool CanDeleteTopic()
        {
            return Model.CanDelete && Model.Topic.UserId == CurrentUserId && !Model.Topic.Locked || Model.CanModerate;
        }

        protected bool CanCreateReply()
        {
            return Model.CanReply && !Model.Topic.Locked || Model.CanModerate;
        }

        protected bool CanEditReply(string replyUserId)
        {
            return Model.CanEdit && replyUserId == CurrentUserId && !Model.Topic.Locked || Model.CanModerate;
        }

        protected bool CanDeleteReply(string replyUserId)
        {
            return Model.CanDelete && replyUserId == CurrentUserId && !Model.Topic.Locked || Model.CanModerate;
        }
    }
}
