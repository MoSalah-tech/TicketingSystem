using Booking.API.Data;
using Booking.API.Services;
using Booking.API.Workers;
using Booking.API.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FluentValidation;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using Booking.API.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// HttpClient to call the Event Catalog service
builder.Services.AddHttpClient<IBookingService, BookingService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["EventCatalogUrl"] ?? "https://localhost:7266");
});

builder.Services.AddHttpContextAccessor();

var jwt = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
            RoleClaimType = ClaimTypes.Role

        };
    });
// Named HttpClient for the background worker (separate from the typed BookingService client)
builder.Services.AddHttpClient("Event Catalog", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["EventCatalogUrl"] ?? "https://localhost:7266");
});

// Service-to-service token provider
builder.Services.AddScoped<IServiceTokenProvider, ServiceTokenProvider>();
// Background worker
builder.Services.AddHostedService<BookingExpirationWorker>();
// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateBookingValidator>();
builder.Services.AddScoped<ValidationFilter>();

builder.Services.AddAuthorization();

builder.Services.AddControllers(options => 
{
    options.Filters.Add<ValidationFilter>();

});
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header

    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"

                }

            },
            Array.Empty<string>()


        }

    });



});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization(); 
app.MapControllers();
app.Run();