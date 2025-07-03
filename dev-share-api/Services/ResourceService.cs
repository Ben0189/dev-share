using Microsoft.EntityFrameworkCore;
using Data;
using Entities;
using Models;

namespace Services;

public interface IResourceService
{
    public Task AddResourceAsync(ResourceDTO resourceDto);

    public Task<ResourceDTO> GetResource(String url); 
}

public class ResourceService : IResourceService
{
    private readonly DevShareDbContext _dbContext;
    
    public ResourceService(DevShareDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task AddResourceAsync(ResourceDTO resourceDto)
    {
        _dbContext.Resources.Add(new Resource
        {
            Url = resourceDto.Url,
            Content = resourceDto.Content
        });
        await _dbContext.SaveChangesAsync();
    }

    public async Task<ResourceDTO?> GetResource(string url)
    {
        return await _dbContext.Resources
            .Where(resource => resource.Url == url)
            .Select(resource => new ResourceDTO()
            {
                Url = resource.Url,
                Content = resource.Content
            }).FirstOrDefaultAsync();
    }
}