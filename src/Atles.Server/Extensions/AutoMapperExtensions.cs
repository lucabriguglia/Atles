using System;
using System.Linq;
using System.Reflection;
using Atles.Core.Events;
using Atles.Events.Sites;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Atles.Server.Extensions
{
    public static class AutoMapperExtensions
    {
        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var autoMapperConfig = new MapperConfiguration(cfg =>
            {
                var typesToMap = typeof(SiteCreated).Assembly.GetTypes()
                    .Where(t =>
                        t.GetTypeInfo().IsClass &&
                        !t.GetTypeInfo().IsAbstract &&
                        typeof(IEvent).IsAssignableFrom(t))
                    .ToList();

                foreach (var typeToMap in typesToMap)
                {
                    cfg.CreateMap(typeToMap, typeToMap);
                }
            });

            services.AddSingleton(sp => autoMapperConfig.CreateMapper());

            return services;
        }
    }
}
