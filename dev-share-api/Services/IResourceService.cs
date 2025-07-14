using Models;

namespace Services;

public interface IResourceService
{
    public Task AddResourceAsync(ResourceDto resourceDto);

    public Task<ResourceDto?> GetResourceByUrl(string normalizeUrl);
    
    public Task<ResourceDto> GetResourceById(long resourceId); 
    
}