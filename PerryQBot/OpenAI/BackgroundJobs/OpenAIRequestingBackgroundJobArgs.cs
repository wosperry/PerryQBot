using Mirai.Net.Data.Messages;

public class OpenAIRequestingBackgroundJobArgs
{
    public string GroupId { get; set; }
    public string SenderId { get; set; }
    public string SenderName { get; set; }
    public MessageReceivers Type { get; set; }
    public List<(string Role, string Message)> Messages { get; set; }
    public string GroupName { get; set; }
}