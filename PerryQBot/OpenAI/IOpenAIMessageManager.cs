namespace PerryQBot.OpenAI;

public interface IOpenAIMessageManager
{
    Task<List<OpenAiMessage>> BuildUserRequestMessagesAsync(string senderId, string senderName, string message);
}