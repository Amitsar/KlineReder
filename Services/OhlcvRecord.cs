namespace KlineReader.Services
{
    public class OhlcvRecord
    {
        public long Timestamp { get; set; }  // UNIX time in seconds
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
    }
}