using System;
using System.Threading.Tasks;
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

    public Task<bool> IsPermissionSetNameUnique(Guid siteId, string name, Guid? id = null)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IsPermissionSetValid(Guid siteId, Guid id)
    {
        return await _apiService.GetFromJsonAsync<bool>($"api/admin/permission-sets/is-valid/{id}");
    }
}