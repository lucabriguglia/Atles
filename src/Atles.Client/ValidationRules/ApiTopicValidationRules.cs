using Atles.Client.Services.Api;
using Atles.Validators.ValidationRules;

namespace Atles.Client.ValidationRules;

public class ApiTopicValidationRules : ITopicValidationRules
{
    private readonly ApiService _apiService;

    public ApiTopicValidationRules(ApiService apiService)
    {
        _apiService = apiService;
    }

    public Task<bool> IsTopicValid(Guid siteId, Guid forumId, Guid id)
    {
        throw new NotImplementedException();
    }
}
