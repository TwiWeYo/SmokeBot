namespace SmokeBot.Model
{
    public class Room
    {
        public long Id { get; }
        public int Interval { get; set; }
        public int StartHour { get; set; }
        public int EndHour { get; set; }
        public Room(long id)
        {
            Id = id;
        }
    }
}