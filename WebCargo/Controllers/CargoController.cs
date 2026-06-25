using Microsoft.AspNetCore.Mvc;
using WebCargo.Models;
using WebCargo.Services;

namespace WebCargo.Controllers
{
    public class CargoController : Controller
    {
        public IActionResult Index( string? search,
    int page = 1)
        {
            string? role =
                HttpContext.Session.GetString(
                    "Role"
                );

            if (role == null)
            {
                return RedirectToAction(
                    "Index",
                    "Login"
                );
            }

            // Guest kullanıcı arama yapmadan tüm kargoları göremez
            if (role == "Guest" &&
               string.IsNullOrWhiteSpace(search))
            {
                ViewBag.Role =
                    role;

                return View(
                    new List<Cargo>()
                );
            }

            if (page < 1)
            {
                page = 1;
            }

            List<Cargo> kargolar = new();
            ViewBag.HasNextPage = false;
            ViewBag.Page = 1;

            // Admin için sayfalı kargo listesi getir
            if (role == "Admin" &&
                string.IsNullOrWhiteSpace(search))
            {
                const int pageSize = 5;

                kargolar =
                    Database.GetCargoPage(
                        page,
                        pageSize
                    );

                ViewBag.HasNextPage =
                    kargolar.Count > pageSize;

                if (kargolar.Count > pageSize)
                {
                    kargolar.RemoveAt(
                        pageSize
                    );
                }

                ViewBag.Page = page;
            }
            else if (!string.IsNullOrWhiteSpace(search))
            {
                // Takip numarasına göre kargo ara
                Cargo? cargo =
                    Database.GetCargoByTrackingNumber(
                        search
                    );

                if (cargo != null)
                {
                    kargolar.Add(cargo);
                }
            }

            ViewBag.Role =
                role;

            return View(kargolar);
        }

        // İsim bilgisini gizle (Al********)
        public static string MaskName(
    string name)
        {
            if (name.Length <= 2)
                return name;

            return name.Substring(0, 2) +
                   new string('*', name.Length - 2);
        }

        // Mail adresini gizle
        // alper@gmail.com -> al***@gm***.com
        public static string MaskEmail(
    string email)
        {
            string[] parts =
                email.Split('@');

            string user =
                parts[0];

            string domain =
                parts[1];

            string maskedUser =
                user.Length <= 2
                ? user
                : user.Substring(0, 2) +
                  new string('*', user.Length - 2);

            int dotIndex =
                domain.IndexOf('.');

            string domainName =
                domain.Substring(0, dotIndex);

            string extension =
                domain.Substring(dotIndex);

            string maskedDomain =
                domainName.Length <= 2
                ? domainName
                : domainName.Substring(0, 2) +
                  new string('*', domainName.Length - 2);

            return maskedUser +
                   "@" +
                   maskedDomain +
                   extension;
        }


        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(
    string alici, string tc, string adres, string email)
        {
            int? userId =
                HttpContext.Session.GetInt32(
                    "UserId"
                );

            // Giriş yapmayan kullanıcı kargo oluşturamaz
            if (userId == null)
            {
                return RedirectToAction(
                    "Index",
                    "Login"
                );
            }

            Cargo cargo = new();

            // Benzersiz takip numarası oluştur
            cargo.TrackingNumber =
                 GenerateTrackingNumber();

            cargo.UserId =
                userId.Value;

            cargo.Alici =
                alici;

            // TC Kimlik Numarasını doğrula
            string? tcError =
            TcValidator.ValidateTCNumber(
            tc
            );

            if (tcError != null)
            {
                ViewBag.Error =
                    tcError;

                return View();
            }

            cargo.Tc =
                tc;

            cargo.Adres =
                adres;

            cargo.Email =
                email;

            if (string.IsNullOrWhiteSpace(
            alici))
            {
                return RedirectToAction(
                    "Index"
                    );
            }

            // Yeni oluşturulan kargonun başlangıç durumu
            cargo.Durum =
                "Hazırlanıyor";

            cargo.SonDurumDegisiklikTarihi =
                DateTime.Now;

            Database.InsertCargo(
                cargo
            );

            TempData["Success"] =
                "Kargo başarıyla oluşturuldu.";

            return RedirectToAction(
                "Index"
            );
        }

        // Veritabanında olmayan rastgele takip numarası üret
        public static string GenerateTrackingNumber()
        {
            string trackingNumber;

            // Aynı takip numarası varsa tekrar üret
            do
            {
                trackingNumber =
                    Random.Shared
                    .NextInt64(
                        1000000000,
                        9999999999
                    )
                    .ToString();
            }
            while (
                Database.TrackingNumberExists(
                    trackingNumber
                )
            );

            return trackingNumber;
        }


        [HttpPost]
        public IActionResult Delete(int id)
        {
            string? role =
                HttpContext.Session.GetString(
                    "Role"
                );

            // Sadece admin kargo silebilir
            if (role != "Admin")
            {
                return RedirectToAction(
                    "Index"
                );
            }

            Database.DeleteCargo(id);

            TempData["Success"] =
                "Kargo başarıyla silindi.";

            return RedirectToAction(
                "Index"
            );
        }

        public IActionResult Edit(int id)
        {
            Cargo? cargo =
                Database.GetCargoById(
                id
            );

            if (cargo == null)
            {
                return RedirectToAction(
                    "Index"
                );
            }

            ViewBag.Role =
                HttpContext.Session
                .GetString("Role");

            return View(cargo);
        }


        [HttpPost]
        public IActionResult Edit(
    int id,
    string alici,
    string tc,
    string adres,
    string email,
    string durum)
        {
            Cargo? cargo =
                Database.GetCargoById(
                    id
                );

            if (cargo == null)
            {
                return RedirectToAction(
                    "Index"
                );
            }

            string? role =
                HttpContext.Session
                .GetString("Role");

            cargo.Alici =
                alici;

            cargo.Adres =
                adres;

            cargo.Email =
                email;

            string eskiDurum =
                cargo.Durum;

            cargo.Durum =
                durum;

            if (role == "Admin")
            {
                string? tcError =
                    TcValidator.ValidateTCNumber(
                        tc
                    );

                if (tcError != null)
                {
                    ViewBag.Error =
                        tcError;

                    ViewBag.Role =
                        role;

                    return View(cargo);
                }

                cargo.Tc =
                    tc;
            }

            cargo.SonDurumDegisiklikTarihi =
                DateTime.Now;

            // Durum değiştiyse müşteriye bilgilendirme maili gönder
            if (eskiDurum != cargo.Durum)
            {
                MailService.SendMail(
                    cargo.Email,
                    "Kargo Durumu Bildirimi",
                    $"""
                    Sayın {cargo.Alici},

                    Kargo durumunuz güncellenmiştir.

                    Takip No: {cargo.TrackingNumber}
                    Yeni Durum: {cargo.Durum}

                    İyi günler dileriz.
                    """
                );
            }

            Database.UpdateCargo(
                cargo
            );

            TempData["Success"] =
                "Kargo başarıyla güncellendi.";

            return RedirectToAction(
                "Index"
            );
        }

    }
}