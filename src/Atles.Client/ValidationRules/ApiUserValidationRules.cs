using Atles.Client.Services.Api;
using Atles.Validators.ValidationRules;

namespace Atles.Client.ValidationRules;

public class ApiUserValidationRules : IUserValidationRules
{
    private readonly ApiService _apiService;

    public ApiUserValidationRules(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<bool> IsUserEmailUnique(Guid id, string email) => 
        await _apiService.GetFromJsonAsync<bool>($"api/admin/users/is-email-unique/{id}/{email}");

    public async Task<bool> IsUserDisplayNameUnique(Guid id, string displayName) =>
        await _apiService.GetFromJsonAsync<bool>($"api/admin/users/is-display-name-unique/{id}/{displayName}");
}
