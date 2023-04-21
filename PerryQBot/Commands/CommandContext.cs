using Mirai.Net.Data.Messages;

public class CommandContext
{
    public MessageReceivers Type { get; set; }
    public string SenderId { get; set; }
    public string SenderName { get; set; }
    public string GroupId { get; set; }
    public string GroupName { get; set; }
    public string Message { get; set; }
    public string CommandString { get; internal set; }
}