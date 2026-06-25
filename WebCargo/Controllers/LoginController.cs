using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebCargo.Models;
using WebCargo.Services;

namespace WebCargo.Controllers
{
    public class LoginController : Controller // mvc controllerı olarak deklare ediyoruz
    {
        private readonly IConfiguration _configuration; // ASP.NET ayarlarını okumak için (appsettings)
        
        public LoginController( // constructor
               IConfiguration configuration)
        { _configuration = configuration;
        } // configurationı kaydediyoruz _configuration içine sitekey kullanmak için


        public IActionResult Index() // login index, IActionResult View() kullanmak için.
        {
            string siteKey =
                _configuration[
                    "Recaptcha:SiteKey"
                    ]!; // sitekey okumak için

            ViewBag.SiteKey = siteKey; // viewbag'e koyuyoruz @ViewBag.SiteKey
                                       // ile göstermek için

            return View(); // Index.cshtml açılır 
        }

        public IActionResult Register()
        {
            string siteKey =
                _configuration[
                    "Recaptcha:SiteKey"
                ]!;

            ViewBag.SiteKey =
                siteKey;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register( // kayıt ol butonuna basınca çalışır
    string username,                               // await kullandigimiz için async method
    string email,
    string password,
    string passwordAgain,
    string tc)
        {

            string captcha =
    Request.Form["g-recaptcha-response"]!; // captcha sonucu tokenin sonucu

            string secretKey =
                _configuration[
                    "Recaptcha:SecretKey"
                ]!;

            using HttpClient client =
                new HttpClient(); // http isteği göndermek için

            var response =
                await client.PostAsync(
                    $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={captcha}",
                    null // captcha tokeni doğru mu kontrol etmek için
                );

            string json =
                await response.Content.ReadAsStringAsync(); // HttpResponseMessage nesnesini stringe çevir

            var result =
                JsonSerializer.Deserialize<RecaptchaResponse>( // stringi nesneye çevir
                    json
                );

            if (result == null ||
                !result.Success)
            {
                ViewBag.Error =
                    "Captcha doğrulanamadı.";

                ViewBag.SiteKey =
                    _configuration["Recaptcha:SiteKey"];

                return View();
            }

            if (username.Contains(" "))
            {
                ViewBag.Error =
                    "Kullanıcı adı boşluk içeremez.";

                return View();
            }


            if (Database.UserExists(
                    username))
            {
                ViewBag.Error =
                    "Bu kullanıcı adı zaten kullanılıyor.";
            ViewBag.SiteKey =
                _configuration[
                    "Recaptcha:SiteKey"
                 ];

                return View();
            }

            if (password.Length < 8)
            {
                ViewBag.Error =
                    "Şifre en az 8 karakter olmalıdır.";

                ViewBag.SiteKey =
                    _configuration["Recaptcha:SiteKey"];

                return View();
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.Error =
                    "E-posta boş bırakılamaz.";

                ViewBag.SiteKey =
                    _configuration["Recaptcha:SiteKey"];

                return View();
            }

            if (password != passwordAgain)
            {
                ViewBag.Error =
                    "Şifreler eşleşmiyor.";

                ViewBag.SiteKey =
                    _configuration["Recaptcha:SiteKey"];

                return View();
            }

            if (TcValidator.ValidateTCNumber(tc) != null)
            {
                ViewBag.Error =
                    TcValidator.ValidateTCNumber(tc);

                ViewBag.SiteKey =
                    _configuration["Recaptcha:SiteKey"];

                return View();
            }

            User? user =
                Database.GetUserByEmail(
                    email
            );

            if (user == null)
            {
                ViewBag.Error =
                    "Bu e-posta adresi için kayıt izni bulunmamaktadır.";

                ViewBag.SiteKey =
                    _configuration["Recaptcha:SiteKey"];

                return View();
            }

            if (!string.IsNullOrEmpty(user!.PasswordHash))
            {
                ViewBag.Error =
                    "Bu e-posta adresi zaten kayıtlı.";

                ViewBag.SiteKey =
                    _configuration["Recaptcha:SiteKey"];

                return View();
            }

            Database.CompleteRegistration(
                username,
                Security.HashPassword(password),
                email,
                tc,
                DateTime.Now
            );

            TempData["Message"] =
                "Kayıt başarıyla oluşturuldu.";

            return RedirectToAction(
                "Index"
            );
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(
    string email)
        {

            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.Error =
                    "E-posta boş bırakılamaz.";

                return View();
            }

            User? user =
                Database.GetUserByEmail(
                    email
                );

            if (user != null)
            {
                string code =
                Random.Shared
                .Next(100000, 999999)
                .ToString();

                HttpContext.Session.SetString(
                    "ResetCode",
                    code
                );

                HttpContext.Session.SetString(
                    "ResetEmail",
                    email
                );

                MailService.SendMail(
                email,
                "Şifre Sıfırlama",
                $"Şifre sıfırlama kodunuz: {code}"
                );
            }
            TempData["Message"] =
                "E-posta adresi kayıtlıysa kod gönderilmiştir.";

            return RedirectToAction(
                "VerifyCode"
                );
            }

        public IActionResult VerifyCode()
        {

            return View();
        }

        [HttpPost]
        public IActionResult VerifyCode(
            string code)
        {
            string? savedCode =
                HttpContext.Session.GetString(
                    "ResetCode"
                    );

   
            if (code != savedCode)
            {
                ViewBag.Error =
                    "Doğrulama kodu hatalı.";

                return View();
            }

            return RedirectToAction(
                "ResetPassword"
                );
        }

        public IActionResult ResetPassword()
        {
            string? email =
                HttpContext.Session.GetString(
                    "ResetEmail"
                );

            if (email == null)
            {
                return RedirectToAction(
                    "ForgotPassword"
                );
            }

            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(
            string password,
            string passwordAgain)
        {
            if (password != passwordAgain)
            {
                ViewBag.Error =
                    "Şifreler eşleşmiyor.";

                return View();
            }

            if (password.Length < 8)
            {
                ViewBag.Error =
                    "Şifre en az 8 karakter olmalıdır.";

                return View();
            }

            string? email =
                HttpContext.Session.GetString(
                    "ResetEmail"
                );

            if (email == null)
            {
                return RedirectToAction(
                    "ForgotPassword"
                );
            }

            Database.UpdatePassword(
                email,
                Security.HashPassword(
                    password
                )
            );

            HttpContext.Session.Remove(
                "ResetCode"
            );

            HttpContext.Session.Remove(
                "ResetEmail"
            );

            return RedirectToAction(
                "Index"
            );
        }


        [HttpPost]
        public ActionResult Guest()
        {

            HttpContext.Session.SetString(
                "Role",
                "Guest"
            );

            return RedirectToAction(
                "Index",
                "Cargo"
            );
        }

        [HttpPost]
        public async Task<IActionResult> AdminLogin(
            string username,
            string password)
        {
            int failCount =
            HttpContext.Session.GetInt32(
                "FailCount"
            ) ?? 0;

            if (failCount >= 3)
            {
                ViewBag.FailCount =
                failCount;
                string captcha =
                    Request.Form["g-recaptcha-response"]!;

                string secretKey =
                    _configuration[
                        "Recaptcha:SecretKey"
                    ]!;

                using HttpClient client =
                    new HttpClient();

                var response =
                    await client.PostAsync(
                        $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={captcha}",
                        null
                    );

                string json =
                    await response.Content.ReadAsStringAsync();

                var result =
                    JsonSerializer.Deserialize<RecaptchaResponse>(
                        json
                    );

                if (result == null ||
                    !result.Success)
                {
                    ViewBag.Error =
                        "Captcha doğrulanamadı.";

                    string siteKey =
                        _configuration["Recaptcha:SiteKey"]!;

                    ViewBag.SiteKey =
                        siteKey;
                    return View("Index");
                }
            }
            

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error =
                    "Kullanıcı adı ve şifre boş bırakılamaz.";

                ViewBag.SiteKey =
                    _configuration["Recaptcha:SiteKey"];

                return View("Index");
            }

            var userId = Database.GetId(
                username
                );

            if (userId == null)
            {

                ViewBag.Error = "Kullanıcı doğrulanamadı.";

                ViewBag.SiteKey =
                    _configuration["Recaptcha:SiteKey"];

                ViewBag.FailCount =
                    failCount;

                return View("Index");
            }

            HttpContext.Session.SetInt32(
                "UserId",
                (int)userId.Value
                );


            if (!Database.Login(
                username,
                Security.HashPassword(password)))
            {
                ViewBag.Error =
                    "Kullanıcı adı veya şifre hatalı.";
                
                failCount++;

                HttpContext.Session.SetInt32(
                    "FailCount",
                    failCount
                );

                ViewBag.FailCount =
                    failCount;


                ViewBag.SiteKey =
                    _configuration["Recaptcha:SiteKey"];

                return View("Index");
            }

            if (username.ToLower() == "admin")
            {
                HttpContext.Session.SetString(
                    "Role",
                    "Admin"
                );
            }
            else
            {
                HttpContext.Session.SetString(
                    "Role",
                    "User"
                );
            }

            HttpContext.Session.SetString(
            "Username",
            username
            );

            HttpContext.Session.SetInt32(
            "FailCount",
             0
            );


            return RedirectToAction(
                "Index",
                "Cargo"
            );
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction(
                "Index",
                "Home"
            );
        }


    }
}