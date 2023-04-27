namespace PerryQBot.OpenAI;

public class WarningException : Exception
{
    public WarningException(string msg) : base(msg)
    {
    }

    public string QQ { get; set; }
    public int WarnTime { get; set; }
}