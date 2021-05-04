using System.Collections.Generic;
using System.Linq;

public class Calendar
{
    public DateTimeInterval WorkingHours { get; set; }

    public List<DateTimeInterval> PlannedMeetings { get; set; }

    

    public List<DateTimeInterval> GetFreeTimeIntervals()
    {
        var workingHours = WorkingHours;
        var plannedMeetings = PlannedMeetings;
        var freeTimeIntervals = new List<DateTimeInterval>
        {
            new DateTimeInterval(workingHours.Start, plannedMeetings.First().Start)
        };

        for (var i = 0; i < plannedMeetings.Count - 1; i++)
        {
            freeTimeIntervals.Add(new DateTimeInterval(plannedMeetings[i].End, plannedMeetings[i + 1].Start));
        }

        freeTimeIntervals.Add(new DateTimeInterval(plannedMeetings.Last().End, workingHours.End));

        return freeTimeIntervals.Where(timeInterval => timeInterval.Lenght > 0).ToList();
    }
}