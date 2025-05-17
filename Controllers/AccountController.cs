using LanceCerto.WebApp.Data;
using LanceCerto.WebApp.Models;
using LanceCerto.WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LanceCerto.WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly LanceCertoDbContext _context;
        private readonly RecaptchaService _recaptcha;

        public AccountController(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager,
            LanceCertoDbContext context,
            RecaptchaService recaptcha)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _recaptcha = recaptcha;
        }

        // GET: /Account/Cadastro
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Cadastro()
        {
            return View();
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

            // 🔐 reCAPTCHA v2
            var captchaResponse = Request.Form["g-recaptcha-response"];
            var isHuman = await _recaptcha.VerifyAsync(captchaResponse);
            if (!isHuman)
            {
                ModelState.AddModelError(string.Empty, "Confirmação de segurança falhou. Por favor, confirme que você não é um robô.");
                return View(model);
            }

            var usuario = new Usuario
            {
                UserName = model.Email,
                Email = model.Email,
                Nome = model.Nome,
                DataNascimento = model.DataNascimento
            };

            var result = await _userManager.CreateAsync(usuario, model.Senha);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(usuario, isPersistent: false);
                return RedirectToAction("Index", "Imovel");
            }

            foreach (var erro in result.Errors)
                ModelState.AddModelError(string.Empty, erro.Description);

            return View(model);
        }

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            // 🔐 reCAPTCHA v2
            var captchaResponse = Request.Form["g-recaptcha-response"];
            var isHuman = await _recaptcha.VerifyAsync(captchaResponse);
            if (!isHuman)
            {
                ModelState.AddModelError(string.Empty, "Confirmação de segurança falhou. Por favor, confirme que você não é um robô.");
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email!);
            if (user != null && !string.IsNullOrWhiteSpace(user.UserName))
            {
                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName,
                    model.Senha!,
                    model.LembrarMe,
                    lockoutOnFailure: true
                );

                if (result.Succeeded)
                    return RedirectToLocal(returnUrl);

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Conta bloqueada por múltiplas tentativas inválidas. Tente novamente mais tarde.");
                    return View(model);
                }
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

        // ✅ Redireciona com segurança
        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Imovel");
        }
    }
}