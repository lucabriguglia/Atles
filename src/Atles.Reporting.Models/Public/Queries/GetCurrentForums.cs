using Atles.Models.Public;
using OpenCqrs.Queries;
using System.Collections.Generic;

namespace Atles.Reporting.Public.Queries
{
    public class GetCurrentForums : IQuery<IList<CurrentForumModel>>
    {
    }
}
