using FreeCourse.Services.Order.Application.Consumer;
using FreeCourse.Services.Order.Infrastructure;
using FreeCourse.Shared.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var requireAuthorizePolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();


// Add services to the container.
builder.Services.AddControllers(opt =>
{
    // tüm kontrolleri authorize ettik
    opt.Filters.Add(new AuthorizeFilter(requireAuthorizePolicy));
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    // bu microservise token daðýtmaktan görevli arkadaþ
    options.Authority = builder.Configuration["IdentityServerUrl"];
    options.Audience = "resource_order";
    options.RequireHttpsMetadata = false;
});

builder.Services.AddDbContext<OrderDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), configure =>
    {
        configure.MigrationsAssembly("FreeCourse.Services.Order.Infrastructure");
    });
});


builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ISharedIdentityService,SharedIdentityService>();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(FreeCourse.Services.Order.Application.Handlers.CreateOrderCommandHandler).Assembly);
    //cfg.RegisterServicesFromAssembly(typeof(FreeCourse.Services.Order.Application.Handlers.GetOrderByUserIdQueryHandler).Assembly);
});


// bu ayarlar önemli
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<CreateOrderMessageCommandConsumer>();
    x.AddConsumer<CourseNameChangedEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQUrl"], "/", host =>
        {
            host.Username("guest");
            host.Password("guest");
        });

        cfg.ReceiveEndpoint("create-order-service", e =>
        {
            e.ConfigureConsumer<CreateOrderMessageCommandConsumer>(context);
        });

        cfg.ReceiveEndpoint("course-nama-change-event-order-service", e =>
        {
            e.ConfigureConsumer<CourseNameChangedEventConsumer>(context);
        });

    });
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var orderDbContext = serviceProvider.GetRequiredService<OrderDbContext>();
    orderDbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
