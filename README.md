# 📦 WebCargo

A cargo tracking and management system developed with **ASP.NET Core MVC**.

---

🌐 **Language / Dil**

* 🇹🇷 [Türkçe](#-türkçe)
* 🇬🇧 [English](#-english)

---

# 📷 Screenshots

## Home

![Home](Screenshots/Home.png)

---

## Cargo Management

![Cargo Management](Screenshots/Cargo.png)

---

## User Panel

![User Panel](Screenshots/User.png)

---

# 🇹🇷 Türkçe

<details open>

<summary><b>📖 Proje Açıklaması</b></summary>

WebCargo, **ASP.NET Core MVC** kullanılarak geliştirilmiş bir **kargo takip ve yönetim sistemidir**.

Projede kullanıcı yönetimi, rol tabanlı yetkilendirme, güvenli giriş sistemi, kargo yönetimi ve takip sistemi bulunmaktadır.

Sistem üç farklı kullanıcı rolünü desteklemektedir:

* 👑 Admin
* 👤 User
* 👀 Guest

</details>

<details>

<summary><b>✨ Özellikler</b></summary>

### Kullanıcı Sistemi

* Kullanıcı kaydı
* Güvenli giriş sistemi
* Çıkış işlemi
* Şifre sıfırlama (E-posta)
* SHA256 parola hashleme
* Google reCAPTCHA
* Session yönetimi

### Admin

* Kargo oluşturma
* Kargo düzenleme
* Kargo silme
* Tüm kargoları görüntüleme
* Sayfalama (Pagination)

### User

* Kendi kargo bilgilerini güncelleme
* Ad, adres ve e-posta düzenleme

### Guest

* Takip numarası ile kargo sorgulama

### Kargo Yönetimi

* Kargo oluşturma
* Kargo güncelleme
* Kargo silme
* Takip numarası ile arama
* Rastgele benzersiz takip numarası oluşturma
* Durum güncelleme
* Durum değiştiğinde otomatik e-posta gönderimi

### Güvenlik

* Google reCAPTCHA
* TC Kimlik Numarası doğrulama
* SHA256 parola hashleme
* Parametreli SQL sorguları
* Session tabanlı yetkilendirme
* Rol tabanlı erişim kontrolü
* Sunucu taraflı doğrulama

</details>

<details>

<summary><b>🛠 Kullanılan Teknolojiler</b></summary>

* ASP.NET Core MVC
* C#
* SQLite
* ADO.NET
* HTML5
* CSS3
* JavaScript
* Google reCAPTCHA
* SMTP (E-mail)

</details>

<details>

<summary><b>🚀 Kurulum</b></summary>

1. Repoyu klonlayın.

```bash
git clone https://github.com/ALTI14/WebCargo.git
```

2. Visual Studio 2022 ile açın.

3. `appsettings.json` içerisine kendi bilgilerinizi girin.

```json
"Recaptcha": {
  "SiteKey": "YOUR_RECAPTCHA_SITE_KEY",
  "SecretKey": "YOUR_RECAPTCHA_SECRET_KEY"
},

"Mail": {
  "Address": "your-email@example.com",
  "Password": "YOUR_APP_PASSWORD"
}
```

4. SQLite veritabanını oluşturun.

5. Projeyi çalıştırın.

</details>

---

# 🇬🇧 English

<details>

<summary><b>📖 About</b></summary>

WebCargo is a **cargo tracking and management system** developed using **ASP.NET Core MVC**.

The application includes secure authentication, role-based authorization, cargo management, and cargo tracking functionalities.

Supported user roles:

* 👑 Admin
* 👤 User
* 👀 Guest

</details>

<details>

<summary><b>✨ Features</b></summary>

### Authentication

* User registration
* Secure login
* Logout
* Password reset via e-mail
* SHA256 password hashing
* Google reCAPTCHA
* Session-based authentication

### Admin

* Create cargo
* Edit cargo
* Delete cargo
* View all cargos
* Pagination support

### User

* Update personal cargo information
* Edit name, address and e-mail

### Guest

* Search cargo using tracking number

### Cargo Management

* Cargo creation
* Cargo editing
* Cargo deletion
* Search by tracking number
* Random unique tracking number generation
* Cargo status updates
* Automatic status notification e-mails

### Security

* Google reCAPTCHA
* Turkish Identity Number validation
* SHA256 password hashing
* Parameterized SQL queries
* Session management
* Role-based authorization
* Server-side validation

</details>

<details>

<summary><b>🛠 Technologies</b></summary>

* ASP.NET Core MVC
* C#
* SQLite
* ADO.NET
* HTML5
* CSS3
* JavaScript
* Google reCAPTCHA
* SMTP (E-mail)

</details>

<details>

<summary><b>🚀 Installation</b></summary>

1. Clone the repository.

```bash
git clone https://github.com/ALTI14/WebCargo.git
```

2. Open the solution with Visual Studio 2022.

3. Configure your own credentials in `appsettings.json`.

```json
"Recaptcha": {
  "SiteKey": "YOUR_RECAPTCHA_SITE_KEY",
  "SecretKey": "YOUR_RECAPTCHA_SECRET_KEY"
},

"Mail": {
  "Address": "your-email@example.com",
  "Password": "YOUR_APP_PASSWORD"
}
```

4. Create the SQLite database.

5. Build and run the project.

</details>

---

## 📄 License

This project was developed for educational and internship purposes.

---

⭐ If you like this project, consider giving it a star.
