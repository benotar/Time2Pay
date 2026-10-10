using Microsoft.Extensions.DependencyInjection;
using Time2Pay.BLL.Services;
using Time2Pay.BLL.Services.Interfaces;

namespace Time2Pay.BLL;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddServices()
        {
            services.AddScoped<IEmploymentService, EmploymentService>();

            return services;
        }
    }
}
