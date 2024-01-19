namespace blazor.Models
{
    public class RegisterModel
    {
        public string Nif { get; set; }
        public string Nome { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int UserType { get; set; }
        public decimal Saldo { get; set; }
    }

}
