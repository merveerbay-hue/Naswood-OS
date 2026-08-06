using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Naswood.BuildingBlocks.Application.Storage;

namespace Naswood.BuildingBlocks.Infrastructure.Storage;

public static class FileStorageDependencyInjection
{
    public static IServiceCollection AddFileStorage(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<FileStorageOptions>(configuration.GetSection(FileStorageOptions.SectionName));

        var provider = configuration.GetSection(FileStorageOptions.SectionName)["Provider"] ?? "Local";

        services.AddSingleton<LocalFileStorageProvider>();
        services.AddSingleton<IS3FileStorageProvider, UnimplementedS3FileStorageProvider>();
        services.AddSingleton<IAzureBlobFileStorageProvider, UnimplementedAzureBlobFileStorageProvider>();

        if (string.Equals(provider, "S3", StringComparison.OrdinalIgnoreCase))
        {
            services.AddSingleton<IFileStorage>(sp => sp.GetRequiredService<IS3FileStorageProvider>());
        }
        else if (string.Equals(provider, "AzureBlob", StringComparison.OrdinalIgnoreCase))
        {
            services.AddSingleton<IFileStorage>(sp => sp.GetRequiredService<IAzureBlobFileStorageProvider>());
        }
        else
        {
            services.AddSingleton<IFileStorage>(sp => sp.GetRequiredService<LocalFileStorageProvider>());
        }

        return services;
    }
}
