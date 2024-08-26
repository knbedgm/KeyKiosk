namespace KeyKiosk.Services
{
    public class UserSessionService
    {
        public string? User { get => _user; }
        string? _user = null;
        public UserSessionService() { }

        public bool Login(string pin)
        { 
            if (pin == "123456")
            {
                _user = "Paul";
                return true;
            }
            if (pin == "555555")
            {
                _user = "Kyle";
                return true;
            }

        public void Logout()
        {
            _user = null;
            navigationManager.NavigateTo("splash");
        }

    }
}
