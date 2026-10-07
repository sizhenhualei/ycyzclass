using YcyzClass.Platforms.Abstraction.Models;
using YcyzClass.Platforms.Abstraction.Services;

namespace YcyzClass.Platforms.Abstraction.Stubs.Services;

/// <summary>
/// 位置服务桩
/// </summary>
public class LocationServiceStub : ILocationService
{
    internal LocationServiceStub()
    {
        
    }
    
    public async Task<LocationCoordinate> GetLocationAsync()
    {
        return new LocationCoordinate()
        {
            Longitude = 0,
            Latitude = 0
        };
    }
}