using Atlas.Data;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Tests.Data
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
