using JsonDataApi.Models;
using System.Threading.Tasks;

namespace JsonDataApi.Services
{
    public interface IAuthenticationService
    {
        Task<string> Register(User user);
        Task<string> Login(User user);
    }
}