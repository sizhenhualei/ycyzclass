using System.Diagnostics.CodeAnalysis;
using YcyzClass.Core.Abstractions.Models.Speech;

namespace YcyzClass.Core.Abstractions.Services.SpeechService;

/// <summary>
/// TTS服务接口
/// </summary>
public interface ISpeechService
{
    /// <summary>
    /// 向TTS队列中添加文本。
    /// </summary>
    /// <param name="text">要朗读的文本</param>
    public void EnqueueSpeechQueue(string text);

    /// <summary>
    /// 清空TTS队列。
    /// </summary>
    public void ClearSpeechQueue();

    /// <summary>
    /// 语音全局设置
    /// </summary>
    [NotNull]
    public static IGlobalSpeechSettings? GlobalSettings { get; internal set; }
}