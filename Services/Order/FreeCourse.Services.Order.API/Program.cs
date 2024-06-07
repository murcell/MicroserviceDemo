using FreeCourse.Services.Order.Infrastructure;
using FreeCourse.Shared.Services;
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



var app = builder.Build();

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
