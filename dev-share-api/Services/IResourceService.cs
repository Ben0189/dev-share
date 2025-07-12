using Models;

namespace Services;

public interface IResourceService
{
    public Task AddResourceAsync(ResourceDTO resourceDto);

    public Task<ResourceDTO?> GetResourceByUrl(string normalizeUrl);
    
    public Task<ResourceDTO> GetResourceById(long resourceId); 
    
}