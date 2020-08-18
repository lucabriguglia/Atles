using Microsoft.EntityFrameworkCore;

namespace Atlify.Data.Tests
{
    public static class Shared
    {
        public static DbContextOptions<AtlifyDbContext> CreateContextOptions()
        {
            var builder = new DbContextOptionsBuilder<AtlifyDbContext>();
            builder.UseInMemoryDatabase("Atlify");
            return builder.Options;
        }
    }
}
