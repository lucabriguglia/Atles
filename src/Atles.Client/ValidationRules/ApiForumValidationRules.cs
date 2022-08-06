using Atles.Client.Services.Api;
using Atles.Validators.ValidationRules;

namespace Atles.Client.ValidationRules;

public class ApiForumValidationRules : IForumValidationRules
{
    private readonly ApiService _apiService;

    public ApiForumValidationRules(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<bool> IsForumNameUnique(Guid siteId, Guid categoryId, Guid id, string name) => 
        await _apiService.GetFromJsonAsync<bool>($"api/admin/forums/is-name-unique/{categoryId}/{id}/{name}");

    public async Task<bool> IsForumSlugUnique(Guid siteId, Guid id, string slug) => 
        await _apiService.GetFromJsonAsync<bool>($"api/admin/forums/is-slug-unique/{id}/{slug}");

    public async Task<bool> IsForumValid(Guid siteId, Guid id) => 
        await _apiService.GetFromJsonAsync<bool>($"api/admin/forums/is-forum-valid/{id}");
}
