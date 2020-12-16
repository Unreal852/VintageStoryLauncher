namespace VintageStoryLauncher.Core.Auth
{
    public class AuthInfo
    {
        public AuthInfo(string email, string firstName, string lastName, string playerName)
        {
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            PlayerName = playerName;
        }

        public string Email      { get; set; }
        public string FirstName  { get; set; }
        public string LastName   { get; set; }
        public string PlayerName { get; set; }
    }
}