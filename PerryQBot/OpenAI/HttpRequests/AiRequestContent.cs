public class AiRequestContent
{
    public AiRequestContent()
    {
    }

    public AiRequestContent(List<OpenAiMessage> messages)
    {
        Messages = messages;
    }

    public string Model { get; set; } = "gpt-3.5-turbo";
    public List<OpenAiMessage> Messages { get; set; } = new();
    public double Temperature { get; set; } = 1;
}