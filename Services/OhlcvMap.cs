using CsvHelper.Configuration;
using KlineReader.Services;
using System.Globalization;

public sealed class OhlcvMap : ClassMap<OhlcvRecord>
{
    public OhlcvMap()
    {
        Map(m => m.Timestamp)
            .Name("Date")
            .Convert(args =>
            {
                var dateStr = args.Row.GetField("Date");
                // European format: day/month/year (e.g., 12/8/2025)
                var formats = new[] { "d/M/yyyy", "dd/MM/yyyy" }; // Add more formats if needed
                if (DateTime.TryParseExact(dateStr, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                {
                    return new DateTimeOffset(dt).ToUnixTimeSeconds();
                }

                return 0L;
            });

        Map(m => m.Close).Name("Price");
        Map(m => m.Open).Name("Open");
        Map(m => m.High).Name("High");
        Map(m => m.Low).Name("Low");
        Map(m => m.Volume).Name("Vol.")
            .Convert(args =>
            {              
                var volStr = args.Row.GetField("Vol.").Trim();
                if (volStr.EndsWith("K"))
                    return decimal.Parse(volStr.TrimEnd('K'), CultureInfo.InvariantCulture) * 1000;
                if (volStr.EndsWith("M"))
                    return decimal.Parse(volStr.TrimEnd('M'), CultureInfo.InvariantCulture) * 1_000_000;
                return decimal.Parse(volStr, CultureInfo.InvariantCulture);
            });
    }
}
