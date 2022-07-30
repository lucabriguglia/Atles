using System;
using System.Linq;
using System.Threading.Tasks;
using Atles.Client.Models;
using Atles.Client.Services;
using Atles.Client.Services.PostReactions;
using Atles.Client.Shared;
using Atles.Models;
using Atles.Models.Public;
using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Atles.Client.Components.Themes
{
    public abstract class TopicComponent : ThemeComponentBase
    {
        [Parameter] public string ForumSlug { get; set; }
        [Parameter] public string TopicSlug { get; set; }

        [Inject] public IPostReactionService PostReactionService { get; set; }

        protected TopicPageModel Model { get; set; }
        protected UserTopicReactionsModel UserTopicReactions { get; set; }

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
        protected string Search { get; set; }
        protected int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        protected bool DisplayPage { get; set; }
        protected bool EditingAnswer { get; set; }
        protected bool DeletingAnswer { get; set; }

        protected PagerComponent Pager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Model = await ApiService.GetFromJsonAsync<TopicPageModel>($"api/public/topics/{ForumSlug}/{TopicSlug}?page=1");
                TotalPages = Model.Replies.TotalPages;
                DisplayPage = true;

                var claimsPrincipal = await GetClaimsPrincipal();
                UserTopicReactions = await PostReactionService.GetReactions(Model.Topic.Id);
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
            TotalPages = Model.Replies.TotalPages;
            StateHasChanged();
        }

        protected async Task SearchAsync()
        {
            CurrentPage = 1;
            Model.Replies = null;
            await LoadDataAsync();
            Pager.ReInitialize(TotalPages);
        }

        protected async Task ClearSearchAsync()
        {
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = string.Empty;
                CurrentPage = 1;
                Model.Replies = null;
                await LoadDataAsync();
                Pager.ReInitialize(TotalPages);
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
            CurrentPage = page;
            Model.Replies = null;
            await LoadDataAsync();
            await JsRuntime.InvokeVoidAsync("atlas.interop.scrollToTarget", "replies");
        }

        protected void SetDeleteReplyId(Guid id, bool isAnswer = false)
        {
            DeletingAnswer = isAnswer;
            DeleteReplyId = id;
        }

        protected async Task NewReplyAsync()
        {
            await JsRuntime.InvokeVoidAsync("atlas.interop.scrollToTarget", "reply");
        }

        protected async Task AddSubscriptionAsync()
        {
            await ApiService.PostAsync($"api/public/subscriptions/add-subscription/{Model.Forum.Id}/{Model.Topic.Id}", null);
            Model.Topic.Subscribed = !Model.Topic.Subscribed;
        }

        protected async Task RemoveSubscriptionAsync()
        {
            await ApiService.PostAsync($"api/public/subscriptions/remove-subscription/{Model.Forum.Id}/{Model.Topic.Id}", null);
            Model.Topic.Subscribed = !Model.Topic.Subscribed;
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

        protected async Task EditReplyAsync(Guid id, string content, Guid userId, bool isAnswer = false)
        {
            EditingAnswer = isAnswer;
            Model.Post.Id = id;
            Model.Post.Content = content;
            Model.Post.UserId = userId;
            await JsRuntime.InvokeVoidAsync("atlas.interop.scrollToTarget", "reply");
        }

        protected async Task SaveReplyAsync()
        {
            var requestUri = Model.Post.Id != null
                ? "api/public/replies/update-reply"
                : "api/public/replies/create-reply";

            await SaveDataAsync(() => ApiService.PostAsJsonAsync(requestUri, Model));

            var previousTotalPages = TotalPages;

            if (Model.Post.Id != null)
            {
                await JsRuntime.InvokeVoidAsync("atlas.interop.scrollToTarget", Model.Post.Id.Value);
            }

            if (Model.Post.Id == null)
            {
                CurrentPage = TotalPages;
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

            if (Model.Post.Id == null)
            {
                Pager.ReInitialize(TotalPages);
                if (TotalPages > previousTotalPages)
                {
                    await ChangePageAsync(TotalPages);
                }
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

            CurrentPage = 1;

            if (DeletingAnswer)
            {
                await OnInitializedAsync();
            }
            else
            {
                await LoadDataAsync();
            }

            Pager.ReInitialize(TotalPages);
        }

        protected async Task AddReactionAsync(ReactionCommandModel command)
        {
            await PostReactionService.AddReaction(Model.Forum.Id, Model.Topic.Id, command);

            UserTopicReactions.PostReactions.Add(command.PostId, command.PostReactionType);

            if (command.PostId == Model.Topic.Id)
            {
                Model.Topic.AddReaction(command.PostReactionType);
            }
            else if(command.PostId == Model.Answer.Id)
            {
                Model.Answer.AddReaction(command.PostReactionType);
            }
            else
            {
                var reply = Model.Replies.Items.FirstOrDefault(x => x.Id == command.PostId);
                reply?.AddReaction(command.PostReactionType);
            }
        }

        protected async Task RemoveReactionAsync(ReactionCommandModel command)
        {
            await PostReactionService.RemoveReaction(Model.Forum.Id, Model.Topic.Id, command);

            UserTopicReactions.PostReactions.Remove(command.PostId);

            if (command.PostId == Model.Topic.Id)
            {
                Model.Topic.RemoveReaction(command.PostReactionType);
            }
            else if (command.PostId == Model.Answer.Id)
            {
                Model.Answer.RemoveReaction(command.PostReactionType);
            }
            else
            {
                var reply = Model.Replies.Items.FirstOrDefault(x => x.Id == command.PostId);
                reply?.RemoveReaction(command.PostReactionType);
            }
        }

        protected void Cancel()
        {
            Model.Post.Id = null;
            Model.Post.Content = null;
            Model.Topic.UserId = Guid.Empty;
        }

        protected async Task SetAnswerAsync(Guid replyId, bool isAnswer)
        {
            Model.Replies = null;
            await ApiService.PostAsJsonAsync($"api/public/replies/set-reply-as-answer/{Model.Forum.Id}/{Model.Topic.Id}/{replyId}", isAnswer);
            await OnInitializedAsync();
        }

        protected bool CanEditTopic()
        {
            return Model.Permissions.CanEdit && Model.Topic.IdentityUserId == User.IdentityUserId && !Model.Topic.Locked || Model.Permissions.CanModerate;
        }

        protected bool CanDeleteTopic()
        {
            return Model.Permissions.CanDelete && Model.Topic.IdentityUserId == User.IdentityUserId && !Model.Topic.Locked || Model.Permissions.CanModerate;
        }

        protected bool CanCreateReply()
        {
            return Model.Permissions.CanReply && !Model.Topic.Locked || Model.Permissions.CanModerate;
        }

        protected bool CanEditReply(string replyUserId)
        {
            return Model.Permissions.CanEdit && replyUserId == User.IdentityUserId && !Model.Topic.Locked || Model.Permissions.CanModerate;
        }

        protected bool CanDeleteReply(string replyUserId)
        {
            return Model.Permissions.CanDelete && replyUserId == User.IdentityUserId && !Model.Topic.Locked || Model.Permissions.CanModerate;
        }

        protected bool CanReact()
        {
            return Model.Permissions.CanReact || Model.Permissions.CanModerate;
        }
    }
}
