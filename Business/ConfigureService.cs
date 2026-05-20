using Business.Services.Documents;
using Business.Services.PaymentValidation;
using Business.Services.Report;
using Business.Services.Students;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Business
{
    public static class ConfigureService
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {

            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<IStudentService , StudentService>();
            services.AddScoped<IPaymentValidationApi, PaymentValidationApi>();
            services.AddScoped<IDocumentReport, DocumentReport>();
            return services;
        }
    }
}
