namespace MC.LaundryShop.App.Class
{
    public class UserDetails
    {
        public long Id { get; set; } = 0;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = null;
        public string LastName { get; set; } = string.Empty;
        public string Suffix { get; set; } = null;
        public string FullName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public byte[] Image { get; set; } = null;
    }
}
