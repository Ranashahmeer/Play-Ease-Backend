namespace Play_ease_Backend.NewFolder
{
    public class LoginModels
    {
        public class RegisterRequestDto
        {
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Password { get; set; }
            public int RoleID { get; set; } = 2; // default: normal user
        }
        public class User
        {
            public int UserID { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string PasswordHash { get; set; }
            public int RoleID { get; set; }
            public DateTime? CreatedAt { get; set; }
            public bool IsActive { get; set; }
            public int Age { get; internal set; }
            public string Cnic { get; internal set; }
        }

        public class LoginRequestDto
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class UserResponseDto
        {
            public int UserID { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public int RoleID { get; set; }
        }
    }
}
