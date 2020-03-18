using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atlas.Models;

namespace Atlas.Services
{
    public interface IPermissionSetService
    {
        Task<IEnumerable<PermissionSet>> GetAll(Guid siteId);
    }
}