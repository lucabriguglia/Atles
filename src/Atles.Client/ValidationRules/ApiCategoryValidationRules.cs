using Atles.Client.Services.Api;
using Atles.Validators.ValidationRules;

namespace Atles.Client.ValidationRules;

public class ApiCategoryValidationRules : ICategoryValidationRules
{
    private readonly ApiService _apiService;

    public ApiCategoryValidationRules(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<bool> IsCategoryNameUnique(Guid siteId, Guid id, string name) => 
        await _apiService.GetFromJsonAsync<bool>($"api/admin/categories/is-name-unique/{id}/{name}");

    public async Task<bool> IsCategoryValid(Guid siteId, Guid id) => 
        await Task.FromResult(true);
}
