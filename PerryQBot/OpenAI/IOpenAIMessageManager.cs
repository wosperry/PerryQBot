namespace PerryQBot.OpenAI;

public interface IOpenAIMessageManager
{
    Task<List<(string Role, string Message)>> BuildUserRequestMessagesAsync(string senderId, string senderName, string message);
}