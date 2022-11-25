using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer.Client1.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            //kullanıcının id'sini almak istersek.
            //var userId = User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value;
            
            return View();
        }
        public async Task LogOut()
        {
            await HttpContext.SignOutAsync("Cookies"); //uygulamamızdan çıkış yapar.
            await HttpContext.SignOutAsync("oidc"); //identity server'dan çıkış yapar.
        }
    }
}
