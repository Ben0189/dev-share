using Data;
using Entities;
using Microsoft.EntityFrameworkCore;
using Models;
using Services;

public class ResourceService : IResourceService
{
    private readonly DevShareDbContext _dbContext;

    public ResourceService(DevShareDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddResourceAsync(ResourceDto resourceDto)
    {
        resourceDto.NormalizeUrl = UrlManageUtil.NormalizeUrl(resourceDto.Url);
        _dbContext.Resources.Add(new Resource
        {
            ResourceId = resourceDto.ResourceId,
            NormalizeUrl = resourceDto.NormalizeUrl,
            Url = resourceDto.Url,
            Content = resourceDto.Content,
            Title = resourceDto.Title,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        });
        await _dbContext.SaveChangesAsync();
    }

    public async Task<ResourceDto?> GetResourceByUrl(string normalizeUrl)
    {
        return await _dbContext.Resources
            .Where(resource => resource.NormalizeUrl == normalizeUrl)
            .Select(resource => new ResourceDto
            {
                ResourceId = resource.ResourceId,
                Url = resource.Url,
                NormalizeUrl = resource.NormalizeUrl,
                Content = resource.Content,
                Title = resource.Title
            }).FirstOrDefaultAsync();
    }
    
    public async Task<ResourceDto?> GetResourceById(long resourceId)
    {
        return await _dbContext.Resources
            .Where(resource => resource.ResourceId == resourceId)
            .Select(resource => new ResourceDto
            {
                ResourceId = resource.ResourceId,
                Url = resource.Url,
                NormalizeUrl = resource.NormalizeUrl,
                Content = resource.Content,
                Title = resource.Title,
                UserInsights = resource.UserInsights
                    .Select(insight => new UserInsightDto
                    {
                        ResourceId = insight.ResourceId,
                        Content = insight.Content
                    }).ToList()
            }).FirstOrDefaultAsync();
    }
}