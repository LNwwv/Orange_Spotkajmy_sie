namespace Orange_Spotkajmy_sie
{
    public class CalendarModel
    {
        public string Name { get; set; }
        public DataModel[] DataModels { get; set; }
    }

    public class DataModel
    {
        public Working_Hours working_hours { get; set; }
        public Planned_Meeting[] planned_meeting { get; set; }
    }

    public class Working_Hours
    {
        public string start { get; set; }
        public string end { get; set; }
    }

    public class Planned_Meeting
    {
        public string start { get; set; }
        public string end { get; set; }
    }
}