using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Time2Pay.BLL.Services;
using Time2Pay.BLL.Services.Interfaces;

namespace Time2Pay.BLL;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddBll()
        {
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
            
            services.AddScoped<IEmploymentService, EmploymentService>();

            services.AddScoped<IWorkDayService, WorkDayService>();
            
            return services;
        }
    }
}
