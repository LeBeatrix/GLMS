using GLMS.Web.Data;
using Microsoft.EntityFrameworkCore;
using GLMS.Web.Services;
using GLMS.Web.Interfaces;
using GLMS.Web.Factories;
using GLMS.Web.Observers;
using GLMS.Web.ApiServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ServiceRequestValidator>();
builder.Services.AddHttpClient<CurrencyService>();
builder.Services.AddScoped<ICurrencyConverter, UsdToZarConverter>();
builder.Services.AddScoped<IServiceRequestFactory, ServiceRequestFactory>();
builder.Services.AddScoped<IServiceRequestObserver, ServiceRequestLogger>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<ContractApiService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5001/"); // Update with your API base URL
});
builder.Services.AddHttpClient<ClientApiService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5001/");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
