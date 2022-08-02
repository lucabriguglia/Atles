//using System;
//using System.Threading.Tasks;
//using Atles.Client.Services.Api;

//namespace Atles.Client.ValidationRules;

//public class ApiForumValidationRules : IForumValidationRules
//{
//    private readonly ApiService _apiService;

//    public ApiForumValidationRules(ApiService apiService)
//    {
//        _apiService = apiService;
//    }

//    public async Task<bool> IsForumNameUnique(Guid siteId, Guid categoryId, string name, Guid? id = null)
//    {
//        if (id != null)
//        {
//            return await _apiService.GetFromJsonAsync<bool>($"api/admin/forums/is-name-unique/{categoryId}/{name}/{id}");
//        }

//        return await _apiService.GetFromJsonAsync<bool>($"api/admin/forums/is-name-unique/{categoryId}/{name}");
//    }

//    public async Task<bool> IsForumSlugUnique(Guid siteId, string slug, Guid? id = null)
//    {
//        if (id != null)
//        {
//            return await _apiService.GetFromJsonAsync<bool>($"api/admin/forums/is-name-unique/{slug}/{id}");
//        }

//        return await _apiService.GetFromJsonAsync<bool>($"api/admin/forums/is-name-unique/{slug}");
//    }

//    public async Task<bool> IsForumValid(Guid siteId, Guid id)
//    {
//        return await _apiService.GetFromJsonAsync<bool>($"api/admin/forums/is-forum-valid/{id}");
//    }
//}
