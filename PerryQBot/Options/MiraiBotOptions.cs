public class MiraiBotOptions
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string QQ { get; set; }
    public string AdminQQ { get; set; }
    public string VerifyKey { get; set; }
    public int MaxHistory { get; set; }
    public string CommandStartChar { get; set; } = "$$";
}