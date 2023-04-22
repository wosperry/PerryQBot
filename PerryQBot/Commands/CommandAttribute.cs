[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class CommandAttribute : Attribute
{
    public CommandAttribute(string command)
    {
        Command = command;
    }

    public CommandAttribute()
    { }

    public string Command { get; set; }
}