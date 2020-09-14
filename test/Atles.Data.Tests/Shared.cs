using Atles.Data;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Data.Tests
{
    public static class Shared
    {
        public static DbContextOptions<AtlesDbContext> CreateContextOptions()
        {
            var builder = new DbContextOptionsBuilder<AtlesDbContext>();
            builder.UseInMemoryDatabase("Atlas");
            return builder.Options;
        }
    }
}
