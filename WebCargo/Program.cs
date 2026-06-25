using WebCargo.Models;
using WebCargo.Services;

var builder =
    WebApplication.CreateBuilder(args);


// MailService'in appsettings.json'a erişebilmesi için
MailService.Configuration =
    builder.Configuration;

builder.Services.AddControllersWithViews(); // mvc desteği

builder.Services.AddSession(); // session desteği

Database.Configuration =
    builder.Configuration;

var app = builder.Build(); // yapılandırmayı kullanarak uygulamayı oluştur
app.UseSession(); // session aktif et

Database.CreateDatabase(); // Veritabanı ve tabloları oluştur.

// hata sayfası
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // https
}

app.UseHttpsRedirection(); // http istekleri httpse gonderir.
app.UseRouting();      // url controller action eşleştirmesi

app.UseAuthorization(); // yetkilendirme sistemi aç

app.MapStaticAssets(); // statik dosyaları ekle resim, css, js vs.

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();