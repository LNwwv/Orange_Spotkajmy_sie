using System;

public class DateTimeInterval
{
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }

    public double Lenght => (End - Start).TotalSeconds;

    public DateTimeInterval(TimeSpan start, TimeSpan end)
    {
        Start = start;
        End = end;
    }

    public DateTimeInterval()
    {
    }
}