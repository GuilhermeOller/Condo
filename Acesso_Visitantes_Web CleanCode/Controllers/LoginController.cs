using Microsoft.AspNetCore.Mvc;

namespace Acesso_Moradores_Visitantes.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
