using GLMS.API.Data;
using GLMS.API.Factories;
using GLMS.API.Interfaces;
using GLMS.API.Observers;
using GLMS.API.Services;
using GLMS.API.Observers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ServiceRequestValidator>();

builder.Services.AddHttpClient<CurrencyService>();

builder.Services.AddScoped<ICurrencyConverter, UsdToZarConverter>();

builder.Services.AddScoped<IServiceRequestFactory, ServiceRequestFactory>();

builder.Services.AddScoped<IServiceRequestObserver, ServiceRequestLogger>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMvcClient", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowMvcClient");

app.UseAuthorization();

app.MapControllers();

app.Run();