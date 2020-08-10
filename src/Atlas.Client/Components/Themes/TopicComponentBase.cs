using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Atlas.Domain;
using Atlas.Models;
using Atlas.Models.Public;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Atlas.Client.Components.Themes
{
    public class TopicComponentBase : ThemeComponentBase
    {
        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; set; }

        [Parameter] public Guid ForumId { get; set; }
        [Parameter] public Guid TopicId { get; set; }

        protected TopicPageModel Model { get; set; }

        protected string ReplyTitleText => Model.Post.Id != null
            ? Loc["Edit Reply"]
            : Loc["Reply"];

        protected string ReplyButtonText => Model.Post.Id != null
            ? Loc["Update"]
            : Loc["Save"];

        protected string NoRecordsText => string.IsNullOrWhiteSpace(Search)
            ? Loc["No replies found for this topic."]
            : Loc["No replies found for current search."];

        protected Guid DeleteReplyId { get; set; }

        protected ClaimsPrincipal CurrentUser { get; set; }
        protected string CurrentUserId { get; set; } = Guid.Empty.ToString();

        protected bool Savings { get; set; }
        protected string Search { get; set; }
        protected int CurrentPage { get; set; } = 1;
        protected bool DisplayPage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateTask;
            CurrentUser = authState.User;

            try
            {
                Model = await ApiService.GetFromJsonAsync<TopicPageModel>($"api/public/topics/{ForumId}/{TopicId}?page=1");

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
            Model.Replies = null;
            Model.Replies = await ApiService.GetFromJsonAsync<PaginatedData<TopicPageModel.ReplyModel>>($"api/public/topics/{ForumId}/{TopicId}/replies?page={CurrentPage}&search={Search}");
        }

        protected async Task SearchAsync()
        {
            CurrentPage = 1;
            await LoadDataAsync();
        }

        protected async Task ClearSearchAsync()
        {
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = string.Empty;
                CurrentPage = 1;
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

        protected async Task EditReplyAsync(Guid id, string content, Guid memberId)
        {
            Model.Post.Id = id;
            Model.Post.Content = content;
            Model.Post.MemberId = memberId;
            await JsRuntime.InvokeVoidAsync("scrollToTarget", "reply");
        }

        protected async Task SaveReplyAsync()
        {
            Savings = true;

            var requestUri = Model.Post.Id != null
                ? "api/public/replies/update-reply"
                : "api/public/replies/create-reply";

            await ApiService.PostAsJsonAsync(requestUri, Model);

            Savings = false;

            if (Model.Post.Id != null)
            {
                await JsRuntime.InvokeVoidAsync("scrollToTarget", Model.Post.Id.Value);
            }

            if (Model.Post.Id == null)
            {
                CurrentPage = Model.Replies.TotalPages;
            }

            Model.Post = new TopicPageModel.PostModel();

            await LoadDataAsync();
        }

        protected async Task DeleteTopicAsync(MouseEventArgs e)
        {
            await ApiService.DeleteAsync($"api/public/topics/delete-topic/{ForumId}/{TopicId}");
            Navigation.NavigateTo($"/forum/{ForumId}");
        }

        protected async Task DeleteReplyAsync(MouseEventArgs e)
        {
            await ApiService.DeleteAsync($"api/public/replies/delete-reply/{ForumId}/{TopicId}/{DeleteReplyId}");
            await LoadDataAsync();
        }

        protected void Cancel()
        {
            Model.Post.Id = null;
            Model.Post.Content = null;
            Model.Topic.MemberId = Guid.Empty;
        }

        protected bool CanEditTopic()
        {
            return Model.CanEdit && Model.Topic.UserId == CurrentUserId || Model.CanModerate || CurrentUser.IsInRole(Consts.RoleNameAdmin);
        }

        protected bool CanDeleteTopic()
        {
            return Model.CanDelete && Model.Topic.UserId == CurrentUserId || Model.CanModerate || CurrentUser.IsInRole(Consts.RoleNameAdmin);
        }

        protected bool CanEditReply(string replyUserId)
        {
            return Model.CanEdit && replyUserId == CurrentUserId || Model.CanModerate || CurrentUser.IsInRole(Consts.RoleNameAdmin);
        }

        protected bool CanDeleteReply(string replyUserId)
        {
            return Model.CanDelete && replyUserId == CurrentUserId || Model.CanModerate || CurrentUser.IsInRole(Consts.RoleNameAdmin);
        }
    }
}