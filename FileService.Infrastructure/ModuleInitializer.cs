using Chen.Commons;
using FileService.Domain;
using FileService.Domain.IService;
using FileService.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FileService.Infrastructure
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IFSRespository, FSRespository>();
            services.AddScoped<IStorageClient, MockCloudStorageClient>();
            services.AddScoped<IStorageClient, SMBStorageClient>();
            services.AddScoped<IFSDomainService, FSDomainService>();
            services.AddHttpClient();
        }
    }
}
