using System;
using YcyzClass.Shared.IPC.Abstractions.Services;

namespace YcyzClass.Services;

public class FooService : IFooService
{
    public void DoSomething()
    {
        Console.WriteLine("Foobar");
    }
}