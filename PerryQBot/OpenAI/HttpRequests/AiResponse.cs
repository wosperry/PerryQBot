public class AiResponse
{
    public string Id { get; set; }
    public string Object { get; set; }
    public long Created { get; set; }
    public string Model { get; set; }
    public AiResponseUsage Usage { get; set; }
    public List<AiResponseChoice> Choices { get; set; }
}