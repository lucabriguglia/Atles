using Atles.Data;
using Microsoft.EntityFrameworkCore;

namespace Atles.Tests.Unit
{
    public static class Shared
    {
        public static DbContextOptions<AtlesDbContext> CreateContextOptions()
        {
            var builder = new DbContextOptionsBuilder<AtlesDbContext>();
            builder.UseInMemoryDatabase("Atles");
            return builder.Options;
        }
    }
}
