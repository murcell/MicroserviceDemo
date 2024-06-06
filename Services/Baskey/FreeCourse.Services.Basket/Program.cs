using FreeCourse.Services.Basket.Services;
using FreeCourse.Services.Basket.Settings;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("RedisSettings"));

builder.Services.AddSingleton<RedisService>(sp =>
{
    //uygulama ayaða kalktýðýnda otomaik olarak da redis baþlatýlacak.
    var redisSetting = sp.GetRequiredService<IOptions<RedisSettings>>().Value;
    var redis = new RedisService(redisSetting.Host, redisSetting.Port);
    redis.Connect();
    return redis;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
