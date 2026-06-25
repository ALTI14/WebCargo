namespace WebCargo.Models
{
    public class Cargo
    {
        public int Id { get; set; }

        public string Alici { get; set; } = "";

        public string Email { get; set; } = "";

        public string Durum { get; set; } = "";

        public int UserId { get; set; }

        public string TrackingNumber { get; set; } = "";

        public DateTime SonDurumDegisiklikTarihi { get; set; }

        public string Tc { get; set; } = "";

        public string Adres { get; set; } = "";

    }
}