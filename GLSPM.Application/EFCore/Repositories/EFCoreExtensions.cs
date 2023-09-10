using GLSPM.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Application.EFCore.Repositories
{
    public static class EFCoreExtensions
    {
        public static IServiceCollection ConfigEFCoreLayer(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient(typeof(IRepository<,>),typeof(BaseRepository<,>));
            return services;
        }
    }
}
