using OutGame.UserData.Domain;

namespace OutGame.UserData.Repository
{
    public interface IAccountRepository
    {
        AuthResult Register(string email, string password);
        AuthResult Login(string email, string password);
        void Logout();
    }
}