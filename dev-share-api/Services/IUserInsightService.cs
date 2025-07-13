using Models;

namespace Services;

public interface IUserInsightService
{
    public Task AddUserInsightAsync(UserInsightDTO userInsight);

    public Task<List<UserInsightDTO>> GetUserInsight(long resourceId);
}