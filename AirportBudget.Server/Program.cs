using AirportBudget.Server.Datas;
using Microsoft.EntityFrameworkCore;
using AirportBudget.Server.Filters;
using System.Text.Json.Serialization;
//using AirportBudget.Server.Mappings;
using AirportBudget.Server.Utilities;
using AirportBudget.Server.Interfaces.Repositorys;
using AirportBudget.Server.Repositorys;
using AutoMapper;
using AirportBudget.Server.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using AirportBudget.Server.Services;
using System.Text;
using AirportBudget.Server.Biz;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new CustomExceptionFilterAttribute());
    options.Filters.Add(new CustomResultFilterAttribute());
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    //options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    options.JsonSerializerOptions.MaxDepth = 64;
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    //options.JsonSerializerOptions.Converters.Add(new JsonConverterUtility.DateTimeJsonConverter());
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettingsDTO>();

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings!.Issuer,
        ValidAudience = jwtSettings!.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings!.SecretKey))
    };
});

builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<TokenService>();

//builder.Services.AddAutoMapper(typeof(ViewModelMapping));
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<DESEncryptionUtility>();
builder.Services.AddScoped<BudgetAmountExcelExportService>();
builder.Services.AddScoped<ExportBudgetExcelService>();
builder.Services.AddScoped<ExportFundExcelService>();

// µù¥U IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<AirportBudgetDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("AirportBudget")));

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
