using Atles.Data;
using Microsoft.EntityFrameworkCore;

namespace Atles.Domain.Rules.Handlers.Tests
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
