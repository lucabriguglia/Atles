using Microsoft.EntityFrameworkCore;

namespace Atlas.Data.Tests
{
    public static class Shared
    {
        public static DbContextOptions<AtlasDbContext> CreateContextOptions()
        {
            var builder = new DbContextOptionsBuilder<AtlasDbContext>();
            builder.UseInMemoryDatabase("Atlas");
            return builder.Options;
        }
    }
}
