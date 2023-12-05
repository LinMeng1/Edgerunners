using Microsoft.AspNetCore.Mvc;
using Moon.Core.Models;
using Moon.Core.Models.Edgerunners;

namespace Lucy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthorization();

            builder.Services.Configure<IPWhitelistOptions>(builder.Configuration.GetSection("IPWhitelistOptions"));

            var app = builder.Build();

            app.UseIPWhitelist();

            app.UseAuthorization();

            app.MapPost("/connection", ([FromBody] RequestBody body) =>
            {
                if (body.Client != null)
                {
                    ConnectionHistories histories = new()
                    {
                        Client = body.Client
                    };
                    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(body.Timestamp).ToLocalTime();
                    histories.Time = dateTimeOffset.DateTime;
                    if (body.Event == "client.disconnected") histories.IsConnected = false;
                    else if (body.Event == "client.connected") histories.IsConnected = true;
                    else return;
                    Database.Edgerunners.Insertable(histories).IgnoreColumns("Id").ExecuteCommand();
                }
            });

            app.Run();
        }
    }

    public class RequestBody
    {
        public string Client { get; set; }
        public string Event { get; set; }
        public long Timestamp { get; set; }
    }
}