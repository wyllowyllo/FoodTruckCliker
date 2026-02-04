using Cysharp.Threading.Tasks;
using OutGame.UserData.Domain;

namespace OutGame.UserData.Repository
{
    public interface IAccountRepository
    {
        UniTask<AccountResult> Register(string email, string password);
        UniTask<AccountResult> Login(string email, string password);
        void Logout();
    }
}