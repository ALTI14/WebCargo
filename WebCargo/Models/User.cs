namespace WebCargo.Models
{
    public class User
    {
        public int Id
        {
            get;
            set;
        }

        public string Username
        {
            get;
            set;
        } = "";

        public string PasswordHash
        {
            get;
            set;
        } = "";

        public string Email
        {
            get;
            set;
        } = "";

        public DateTime RegisterDate
        {
            get;
            set;
        }

        public string Tc { get; set; } = "";
    }
}