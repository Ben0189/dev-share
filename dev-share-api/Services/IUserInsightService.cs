using Models;

namespace Services;

public interface IUserInsightService
{
    public Task AddUserInsightAsync(UserInsightDto userInsight);

    public Task<List<UserInsightDto>> GetUserInsight(long resourceId);
}