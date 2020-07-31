using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atlas.Domain.PermissionSets;
using Atlas.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace Atlas.Client.Services
{
    public interface ISecurityService
    {
        Task<bool> HasPermission(Task<AuthenticationState> authenticationStateTask, PermissionModel model);
        Task<bool> HasPermission(Task<AuthenticationState> authenticationStateTask, PermissionType type, IList<PermissionModel> models);
    }
}
