using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Services;

public class UrlManageUtilTest
{
    
    private readonly IResourceService _resourceService;
    
    
    [Fact]
    public async void NormalizeUrl()
    {
        var normalizeUrl = UrlManageUtil.NormalizeUrl(
            "https://www.jetbrains.com/academy/career-in-it/?source=google&medium=cpc&campaign=APAC_en_ASIA_JBAcademy_Boost_Your_Career_Search&term=learn%20java&content=726159867494&gad_source=1&gad_campaignid=22045194704&gbraid=0AAAAADloJzgkigv1D38jQ0Jl58NdqcZLe&gclid=CjwKCAjw4K3DBhBqEiwAYtG_9HNYVMG-wpMeOEeWGKnFU4ocVZn_QwP1uoBOUseJAaIPKDyq9i_nOBoCEUIQAvD_BwE");
        
        Assert.Equal(normalizeUrl, "https://www.jetbrains.com/academy/career-in-it?campaign=APAC_en_ASIA_JBAcademy_Boost_Your_Career_Search&content=726159867494&gad_campaignid=22045194704&gad_source=1&gbraid=0AAAAADloJzgkigv1D38jQ0Jl58NdqcZLe&gclid=CjwKCAjw4K3DBhBqEiwAYtG_9HNYVMG-wpMeOEeWGKnFU4ocVZn_QwP1uoBOUseJAaIPKDyq9i_nOBoCEUIQAvD_BwE&medium=cpc&source=google&term=learn+java");
    }
}