using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Moon.Core.Utilities;
using NLog.Web;
using System.Collections;
using System.Text;

//var xx = Authentication.ToMD5("90311258");
//string xxx = Convert.ToBase64String(Encoding.ASCII.GetBytes($"135a4f9d99b6eebd:xumXFBzNIB4pSmYO6sRZmhD2YPshM6Mvlgg5Mu1jBdJ"));

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

//NLog
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Host.UseNLog();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        // 是否开启签名认证
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Authentication.SecretKey)),
        // 发行人验证，这里要和token类中Claim类型的发行人保持一致
        ValidateIssuer = true,
        ValidIssuer = "Moon.Core",//发行人
        // 接收人验证
        ValidateAudience = true,
        ValidAudience = "Edgerunners",//订阅人
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (true || app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
