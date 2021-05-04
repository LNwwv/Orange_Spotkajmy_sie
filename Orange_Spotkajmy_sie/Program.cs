using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace Orange_Spotkajmy_sie
{
    class Program
    {
        static void Main(string[] args)
        {
            //how long will the meeting take?
            //Change to set up meeting duration meeting time
            var meetingDurationInMinutes = 0;

            //Set up default 30 minutes duration
            if (meetingDurationInMinutes <= 0)
            {
                meetingDurationInMinutes = 30;
            }

            //create two calendars to compare
            var calendarObj1 = new DataModel
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
                        end = "11:30"
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
            var calendarObj2 = new DataModel
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

            //Load JSON from file.
            var pathToCalendar1 = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                @"calendar1.json");
            var pathToCalendar2 = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                @"calendar2.json");

            if (File.Exists(pathToCalendar1) && File.Exists(pathToCalendar2))
            {
                var calendar1 = JsonConvert.DeserializeObject<DataModel>(File.ReadAllText(pathToCalendar1));
                var calendar2 = JsonConvert.DeserializeObject<DataModel>(File.ReadAllText(pathToCalendar2));
                //Check calendar for data from file
                CheckCalendar(calendar1, calendar2, meetingDurationInMinutes);

            }
            else
            {
                Console.WriteLine("Calendar files not found");
            }

            /*
             * check these calendars with CheckCalendar() method
             * and you will see the available appointments
            */

            //Check calendar for data fromn DataModel obj
            CheckCalendar(calendarObj1, calendarObj2, meetingDurationInMinutes);
        }

        private static void CheckCalendar(DataModel firstCalendar, DataModel secondCalendar, int duration)
        {
            var c1 = CreateNewCalendar(firstCalendar);
            var c2 = CreateNewCalendar(secondCalendar);

            var freeTimeIntervalsForC1 = c1.GetFreeTimeIntervals();
            var freeTimeIntervalsForC2 = c2.GetFreeTimeIntervals();

            var intersectedFreeTimes =
                GetFreeTimeIntersections(freeTimeIntervalsForC1, freeTimeIntervalsForC2, duration);

            Console.WriteLine("Available meetings:");
            foreach (var item in intersectedFreeTimes)
            {
                Console.WriteLine($"Start: {item.Start} end: {item.End}");
            }

            Console.WriteLine();
        }

        static List<DateTimeInterval> GetFreeTimeIntersections(List<DateTimeInterval> freeIntersection1,
            List<DateTimeInterval> freeIntersection2, int minutes)
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

                        //check how much meeting availabale in this time interval
                        var minutesIn = TimeSpan.FromMinutes(minutes).Ticks;
                        var meetingsInInterval = (timeIntervalEnd - timeIntervalStart) / minutesIn;

                        for (var k = 0; k < meetingsInInterval; k++)
                        {
                            var timeInterval = new DateTimeInterval
                            {
                                Start = TimeSpan.FromTicks(timeIntervalStart + (minutesIn * k)),
                                End = TimeSpan.FromTicks(timeIntervalStart + (minutesIn * (k + 1))),
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