namespace VismaShortageManagement.Models
{
    public class User
    {
        public string Name { get; set; }
        public bool IsAdministrator { get; set; }

        public User(string name, bool isAdministrator = false)
        {
            Name = name;
            IsAdministrator = isAdministrator;
        }
    }
}