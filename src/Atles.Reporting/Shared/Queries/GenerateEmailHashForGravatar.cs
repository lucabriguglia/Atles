using Atles.Infrastructure.Queries;

namespace Atles.Reporting.Shared.Queries
{
    public class GenerateEmailHashForGravatar : QueryBase<string>
    {
        public string Email { get; set; }
    }
}
