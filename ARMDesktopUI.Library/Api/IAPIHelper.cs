using ARMDesktopUI.Library.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace ARMDesktopUI.Library.Api
{
    public interface IAPIHelper
    {
        HttpClient ApiClient { get; }
        Task<AuthenticatedUser> Authenticate(string username, string password);
        Task GetLoggedInUserInfo(string token);
    }
}