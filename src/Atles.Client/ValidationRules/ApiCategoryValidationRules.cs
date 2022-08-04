using System;
using System.Threading.Tasks;
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

    public async Task<bool> IsCategoryNameUnique(Guid siteId, string name, Guid? id = null)
    {
        if (id != null)
        {
            return await _apiService.GetFromJsonAsync<bool>($"api/admin/categories/is-name-unique/{name}/{id}");
        }

        return await _apiService.GetFromJsonAsync<bool>($"api/admin/categories/is-name-unique/{name}");
    }

    public async Task<bool> IsCategoryValid(Guid siteId, Guid id)
    {
        return await _apiService.GetFromJsonAsync<bool>($"api/admin/categories/is-valid/{id}");
    }
}
