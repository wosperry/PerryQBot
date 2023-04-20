public class OpenAiMessage
{
    public OpenAiMessage(string role, string content)
    {
        Role = role;
        Content = content;
    }

    public string Role { get; set; }
    public string Content { get; set; }
}