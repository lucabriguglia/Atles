using System.Collections.Generic;
using OpenCqrs.Queries;

namespace Atles.Reporting.Models.Public.Queries
{
    public class GetCurrentForums : IQuery<IList<CurrentForumModel>>
    {
    }
}
