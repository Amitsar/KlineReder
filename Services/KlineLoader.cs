using CsvHelper;
using CsvHelper.Configuration;
using System.Formats.Asn1;
using System.Globalization;

namespace KlineReader.Services
{
    public class KlineLoader
    {
        public static List<OhlcvRecord> Load(string path)
        {
            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            });
            csv.Context.RegisterClassMap<OhlcvMap>();
            return csv.GetRecords<OhlcvRecord>().ToList();
        }
    }
}
