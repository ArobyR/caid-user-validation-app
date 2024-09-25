
namespace UserAuthApp.Models
{
    public class User
    {
        public int IdUser { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}