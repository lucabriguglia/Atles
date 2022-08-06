using Atles.Client.Services.Api;
using Atles.Validators.ValidationRules;

namespace Atles.Client.ValidationRules;

public class ApiPermissionSetValidationRules : IPermissionSetValidationRules
{
    private readonly ApiService _apiService;

    public ApiPermissionSetValidationRules(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<bool> IsPermissionSetNameUnique(Guid siteId, Guid id, string name) => 
        await _apiService.GetFromJsonAsync<bool>($"api/admin/permission-sets/is-name-unique/{id}/{name}");

    public async Task<bool> IsPermissionSetValid(Guid siteId, Guid id) => 
        await Task.FromResult(true);
}