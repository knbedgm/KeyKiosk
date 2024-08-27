using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeyKiosk.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Pin { get; set; } = "000000";
        [Column(TypeName = "TEXT")]
        public UserType UserType { get; set; } = UserType.User;
    }

    public enum UserType
    {
        User,
        Manager,
        Admin
    }
}
