using System.Threading.Tasks;
using LanceCerto.WebApp.Data;
using LanceCerto.WebApp.Models;
using LanceCerto.WebApp.Services;
using LanceCerto.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LanceCerto.WebApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly RecaptchaService _recaptcha;

        private const string RecaptchaFormField = "g-recaptcha-response";

        public AccountController(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager,
            RecaptchaService recaptcha)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _recaptcha = recaptcha;
        }

        // GET: /Account/Cadastro
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Cadastro()
        {
            return View(new CadastroViewModel());
        }

        // POST: /Account/Cadastro
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastro(CadastroViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (string.IsNullOrWhiteSpace(model.Senha))
            {
                ModelState.AddModelError(string.Empty, "A senha é obrigatória.");
                return View(model);
            }

            var token = Request.Form[RecaptchaFormField];
            if (!await _recaptcha.VerifyAsync(token))
            {
                ModelState.AddModelError(string.Empty, "Confirmação de segurança falhou. Por favor, confirme que você não é um robô.");
                return View(model);
            }

            var usuario = new Usuario
            {
                UserName = model.Email,
                Email = model.Email,
                Nome = model.Nome,
                DataNascimento = model.DataNascimento,
                Estado = model.Estado,
                EhVendedor = model.EhVendedor,
                EhCorretor = model.EhCorretor,
                Creci = model.Creci
            };

            var result = await _userManager.CreateAsync(usuario, model.Senha);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(usuario, isPersistent: false);
                return RedirectToAction(nameof(ImovelController.Index), "Imovel");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            ViewData["ReturnUrl"] = model.ReturnUrl;
            if (!ModelState.IsValid)
                return View(model);

            var token = Request.Form[RecaptchaFormField];
            if (!await _recaptcha.VerifyAsync(token))
            {
                ModelState.AddModelError(string.Empty, "Confirmação de segurança falhou. Por favor, confirme que você não é um robô.");
                return View(model);
            }

            // Tenta autenticar diretamente pelo email
            var result = await _signInManager.PasswordSignInAsync(
                model.Email!,
                model.Senha!,
                model.LembrarMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
                return RedirectToLocal(model.ReturnUrl);

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Conta bloqueada por múltiplas tentativas inválidas. Tente novamente mais tarde.");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "E-mail ou senha incorretos.");
            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(ImovelController.Index), "Imovel");
        }
    }
}