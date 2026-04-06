using Microsoft.EntityFrameworkCore;

using MVCShopping1.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Đăng ký Database (đọc connection string từ appsettings.json)
builder.Services.AddDbContext<MVCShopping1Context>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("MVCShopping1Context")
        ?? throw new InvalidOperationException("Connection string not found.")
    )
);

// 2. Đăng ký MVC (Controllers + Views)
builder.Services.AddControllersWithViews();

// 3. Đăng ký Session (PHẢI có để Login và Cart hoạt động)
builder.Services.AddSession();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();       // PHẢI đặt ở đây, trước UseAuthorization
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();