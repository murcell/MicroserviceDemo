using FreeCourse.Services.Basket.Consumers;
using FreeCourse.Services.Basket.Services;
using FreeCourse.Services.Basket.Settings;
using FreeCourse.Shared.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

var requireAuthorizePolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    // bu microservise tokendaðýtmaktan görevli arkadaþ
    options.Authority = builder.Configuration["IdentityServerUrl"];
    options.Audience = "resource_basket";
    options.RequireHttpsMetadata = false;
});


// Add services to the container.
builder.Services.AddControllers(opt =>
{
    // tüm kontrolleri authorize ettik
    opt.Filters.Add(new AuthorizeFilter(requireAuthorizePolicy));
});

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

// bu ayarlar önemli
builder.Services.AddMassTransit(x =>
{
    
    x.AddConsumer<CourseNameChangedEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQUrl"], "/", host =>
        {
            host.Username("guest");
            host.Password("guest");
        });

        cfg.ReceiveEndpoint("course-nama-change-event-basket-service", e =>
        {
            e.ConfigureConsumer<CourseNameChangedEventConsumer>(context);
        });

    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ISharedIdentityService, SharedIdentityService>();
builder.Services.AddScoped<IBasketService, BasketService>(); // bi requestte bir nesne örneði
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
