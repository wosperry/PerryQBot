[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class CommandAttribute : Attribute
{
    public CommandAttribute(string command, bool withPrefix = true)
    {
        Command = command;
        WithPrefix = withPrefix;
    }

    public CommandAttribute()
    { }

    public bool WithPrefix { get; set; } = true;
    public string Command { get; set; }
    public bool WorkerCanSchedule { get; set; } = false;
}