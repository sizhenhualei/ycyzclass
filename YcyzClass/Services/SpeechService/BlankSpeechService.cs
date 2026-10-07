using YcyzClass.Core.Abstractions.Services.SpeechService;
using YcyzClass.Shared.Abstraction.Services;

namespace YcyzClass.Services.SpeechService;

public class BlankSpeechService : ISpeechService
{
    public void EnqueueSpeechQueue(string text)
    {
    }

    public void ClearSpeechQueue()
    {
    }
}