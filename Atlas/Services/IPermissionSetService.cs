using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Services
{
    public interface IPermissionSetService
    {
        Task<IEnumerable<PermissionSet>> GetAll(Guid siteId);
    }
}