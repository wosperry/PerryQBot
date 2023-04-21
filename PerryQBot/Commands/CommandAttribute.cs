[AttributeUsage(AttributeTargets.Class)]
public class CommandAttribute : Attribute
{
    public CommandAttribute(string command, string description)
    {
        Command = command;
        Description = description;
    }

    public CommandAttribute()
    {
    }

    public string Command { get; set; }
    public string Description { get; set; }
}