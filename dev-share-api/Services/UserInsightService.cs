using Data;
using Entities;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Services;

public class UserInsightService : IUserInsightService
{
    private readonly DevShareDbContext _dbContext;

    public UserInsightService(DevShareDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddUserInsightAsync(UserInsightDto userInsight)
    {
        _dbContext.UserInsights.Add(new UserInsight
        {
            ResourceId = userInsight.ResourceId,
            Content = userInsight.Content,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        });
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<UserInsightDto>> GetUserInsight(long resourceId)
    {
        return await _dbContext.UserInsights
            .Where(insight => insight.ResourceId == resourceId)
            .Select(insight => new UserInsightDto
            {
                ResourceId = insight.ResourceId,
                Content = insight.Content
            }).ToListAsync();
    }
}