namespace PerryQBot.Options;

public class WeatherOptions
{
    public string QueryUrl { get; set; }
    public string Key { get; set; }
    public bool ResponseByAi { get; set; } = true;
}