using YcyzClass.Platforms.Abstraction.Services;

namespace YcyzClass.Platforms.Abstraction.Stubs.Services;

public class DesktopServiceStub : IDesktopService
{
    /// <inheritdoc />
    public bool IsAutoStartEnabled
    {
        get => false;
        set { }
    }

    /// <inheritdoc />
    public bool IsUrlSchemeRegistered
    {
        get => false;
        set { }
    }
}