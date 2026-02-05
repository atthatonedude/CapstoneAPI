namespace CapstoneAPI.Model
{
    public class UserLoginModal
    {
        //public int? UserId { get; set; } = null;
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

