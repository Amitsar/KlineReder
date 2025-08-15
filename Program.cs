using KlineReader.Services;

namespace KlineReader
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();
            var symbols = new[] { "BTCUSD", "ETHUSD" };
            var ohlcvDataBySymbol = symbols.ToDictionary(
                symbol => symbol,
                symbol => KlineLoader.Load($"{symbol}.csv")
            );

            app.MapGet("/history", (string symbol, long from, long to) =>
            {
                var ohlcvData = ohlcvDataBySymbol[symbol];
                var filtered = ohlcvData
                    .Where(r => r.Timestamp >= from && r.Timestamp <= to)
                    .OrderBy(r => r.Timestamp)
                    .ToList();

                return new
                {
                    s = "ok",
                    t = filtered.Select(x => x.Timestamp).ToArray(),
                    o = filtered.Select(x => x.Open).ToArray(),
                    h = filtered.Select(x => x.High).ToArray(),
                    l = filtered.Select(x => x.Low).ToArray(),
                    c = filtered.Select(x => x.Close).ToArray(),
                    v = filtered.Select(x => x.Volume).ToArray()
                };
            });

            app.MapGet("/history/meta", (string symbol) =>
            {
                var data = ohlcvDataBySymbol[symbol];

                return new
                {
                    min = data.Min(r => r.Timestamp),
                    max = data.Max(r => r.Timestamp)
                };
            });
            app.MapGet("/symbols", () =>
            {
                return ohlcvDataBySymbol.Keys.ToArray();
            });
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
