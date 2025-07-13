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

    public async Task AddUserInsightAsync(UserInsightDTO userInsight)
    {
        _dbContext.UserInsights.Add(new UserInsight
        {
            ResourceId = userInsight.ResourceId,
            Content = userInsight.Content
        });
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<UserInsightDTO>> GetUserInsight(long resourceId)
    {
        return await _dbContext.UserInsights
            .Where(insight => insight.ResourceId == resourceId)
            .Select(insight => new UserInsightDTO
            {
                ResourceId = insight.ResourceId,
                Content = insight.Content
            }).ToListAsync();
    }
}