using Microsoft.AspNetCore.Mvc;

namespace AutoAtendimento.WEB.Controllers
{
    public class ProdutoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
