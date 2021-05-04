using System;
using System.Collections.Generic;

namespace Orange_Spotkajmy_sie
{
    class Program
    {
        static void Main(string[] args)
        {
            //create two calendars to compare
            var calendar1 = new DataModel
            {
                working_hours = new Working_Hours
                {
                    start = "09:00",
                    end = "19:55",
                },
                planned_meeting = new Planned_Meeting[]
                {
                    new Planned_Meeting
                    {
                        start = "09:00",
                        end = "10:30"
                    },
                    new Planned_Meeting
                    {
                        start = "12:00",
                        end = "13:00"
                    },
                    new Planned_Meeting
                    {
                        start = "16:00",
                        end = "18:00"
                    }
                }
            };
            var calendar2 = new DataModel
            {
                working_hours = new Working_Hours
                {
                    start = "10:00",
                    end = "18:30",
                },
                planned_meeting = new Planned_Meeting[]
                {
                    new Planned_Meeting
                    {
                        start = "10:00",
                        end = "11:30"
                    },
                    new Planned_Meeting
                    {
                        start = "12:30",
                        end = "14:30"
                    },
                    new Planned_Meeting
                    {
                        start = "14:30",
                        end = "15:00"
                    },
                    new Planned_Meeting
                    {
                        start = "16:00",
                        end = "17:00"
                    },
                }
            };

            /*
             * check these calendars with CheckCalendar() method
             * and you will see the available appointments
            */

            CheckCalendar(calendar1, calendar2);
        }

        private static void CheckCalendar(DataModel firstCalendar, DataModel secondCalendar)
        {
            var c1 = CreateNewCalendar(firstCalendar);
            var c3 = CreateNewCalendar(secondCalendar);

            var freeTimeIntervalsForC1 = c1.GetFreeTimeIntervals();
            var freeTimeIntervalsForC2 = c3.GetFreeTimeIntervals();

            var intersectedFreeTimes = GetFreeTimeIntersections(freeTimeIntervalsForC1, freeTimeIntervalsForC2);
            foreach (var item in intersectedFreeTimes)
            {
                Console.WriteLine("Start: {0}, end: {1}", item.Start, item.End);
            }

            Console.WriteLine();
        }

        static List<DateTimeInterval> GetFreeTimeIntersections(List<DateTimeInterval> freeIntersection1,
            List<DateTimeInterval> freeIntersection2)
        {
            var intersectedFreeTimes = new List<DateTimeInterval>();

            for (int i = 0; i < freeIntersection1.Count; i++)
            {
                for (int j = 0; j < freeIntersection2.Count; j++)
                {
                    if (freeIntersection1[i].Start <= freeIntersection2[j].End &&
                        freeIntersection1[i].End >= freeIntersection2[j].Start)
                    {
                        var timeIntervalStart = Math.Max(freeIntersection1[i].Start.Ticks,
                            freeIntersection2[j].Start.Ticks);
                        var timeIntervalEnd = Math.Min(freeIntersection1[i].End.Ticks, freeIntersection2[j].End.Ticks);
                        var is30Minutes = timeIntervalEnd - timeIntervalStart;

                        if (TimeSpan.FromTicks(is30Minutes) >= TimeSpan.FromMinutes(30))
                        {
                            var timeInterval = new DateTimeInterval
                            {
                                Start = TimeSpan.FromTicks(timeIntervalStart),
                                End = TimeSpan.FromTicks(timeIntervalEnd),
                            };
                            intersectedFreeTimes.Add(timeInterval);
                        }
                    }
                }
            }

            return intersectedFreeTimes;
        }

        static TimeSpan ConvertToTimeSpan(string dataModel)
        {
            return Convert.ToDateTime(dataModel).TimeOfDay;
        }

        static Calendar CreateNewCalendar(DataModel model)
        {
            var workHours = new DateTimeInterval(ConvertToTimeSpan(model.working_hours.start),
                ConvertToTimeSpan(model.working_hours.end));

            var meetingSchedule = new List<DateTimeInterval>();

            for (var i = 0; i < model.planned_meeting.Length; i++)
            {
                meetingSchedule.Add(
                    new DateTimeInterval()
                    {
                        Start = ConvertToTimeSpan(model.planned_meeting[i].start),
                        End = ConvertToTimeSpan(model.planned_meeting[i].end)
                    });
            }

            return new Calendar
            {
                WorkingHours = workHours,
                PlannedMeetings = meetingSchedule
            };
        }
    }
}