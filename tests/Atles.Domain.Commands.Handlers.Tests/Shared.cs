using Atles.Data;
using Microsoft.EntityFrameworkCore;

namespace Atles.Domain.Commands.Handlers.Tests
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
