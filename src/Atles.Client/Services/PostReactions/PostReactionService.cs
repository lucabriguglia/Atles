using System;
using System.Text.Json;
using System.Threading.Tasks;
using Atles.Client.Models;
using Atles.Client.Services.Api;
using Atles.Models.Public;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace Atles.Client.Services.PostReactions;

public class PostReactionService : IPostReactionService
{
    private readonly ApiService _apiService;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly IJSRuntime _jSRuntime;

    public PostReactionService(ApiService apiService, AuthenticationStateProvider authenticationStateProvider, IJSRuntime jSRuntime)
    {
        _apiService = apiService;
        _authenticationStateProvider = authenticationStateProvider;
        _jSRuntime = jSRuntime;
    }

    public async Task AddReaction(Guid forumId, Guid topicId, ReactionCommandModel command)
    {
        await _apiService.PostAsJsonAsync($"api/public/reactions/add-reaction/{forumId}/{command.PostId}", command.PostReactionType);

        var localData = await GetReactions(topicId);
        localData.PostReactions.Add(command.PostId, command.PostReactionType);

        var jsonData = JsonSerializer.Serialize(localData);

        await _jSRuntime.InvokeVoidAsync("sessionStorage.setItem", $"Reactions|{topicId}", jsonData).ConfigureAwait(false);

    }

    public async Task RemoveReaction(Guid forumId, Guid topicId, ReactionCommandModel command)
    {
        await _apiService.PostAsJsonAsync($"api/public/reactions/remove-reaction/{forumId}/{command.PostId}", command.PostReactionType);

        var localData = await GetReactions(topicId);
        localData.PostReactions.Remove(command.PostId);

        var jsonData = JsonSerializer.Serialize(localData);

        await _jSRuntime.InvokeVoidAsync("sessionStorage.setItem", $"Reactions|{topicId}", jsonData).ConfigureAwait(false);
    }

    public async Task<UserTopicReactionsModel> GetReactions(Guid topicId)
    {
        var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = state.User;

        if (!user.Identity.IsAuthenticated)
        {
            return new UserTopicReactionsModel();
        }

        var localData = await _jSRuntime.InvokeAsync<string>("sessionStorage.getItem", $"Reactions|{topicId}").ConfigureAwait(false);

        if (localData != null)
        {
            return JsonSerializer.Deserialize<UserTopicReactionsModel>(localData);
        }

        var serverData = await _apiService.GetFromJsonAsync<UserTopicReactionsModel>($"api/public/reactions/topic-reactions/{topicId}").ConfigureAwait(false);
        var jsonData = JsonSerializer.Serialize(serverData);

        await _jSRuntime.InvokeVoidAsync("sessionStorage.setItem", $"Reactions|{topicId}", jsonData).ConfigureAwait(false);

        return serverData;
    }
}