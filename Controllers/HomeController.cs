using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LanceCerto.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // GET: /Home/Index
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            // Se o usuário já estiver autenticado, redireciona para o sistema
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Imovel");
            }

            // Página inicial pública com "Entrar" e "Criar Conta"
            return View();
        }

        // GET: /Home/Error
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Error()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionFeature != null)
            {
                _logger.LogError(exceptionFeature.Error,
                    "Erro não tratado na rota: {Path}", exceptionFeature.Path);
            }

            return View("Error");
        }

        // GET: /Home/Privacy
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View(); // View opcional com a política de privacidade
        }
    }
}