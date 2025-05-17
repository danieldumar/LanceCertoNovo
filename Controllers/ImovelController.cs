using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LanceCerto.WebApp.Data;
using LanceCerto.WebApp.Models;

namespace LanceCerto.WebApp.Controllers
{
    [Authorize]
    public class ImovelController : Controller
    {
        private readonly LanceCertoDbContext _context;

        public ImovelController(LanceCertoDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Popula dropdowns de Estado, Tipo de Imóvel e Status em ViewData.
        /// </summary>
        private void PopularDropdowns(string? estadoSelecionado = null,
                                      string? tipoSelecionado = null,
                                      string? statusSelecionado = null)
        {
            // 1) Listagem de UFs
            var estadosList = EstadoUF.Lista
                .Select(uf => new SelectListItem { Value = uf, Text = uf })
                .ToList();
            if (!string.IsNullOrWhiteSpace(estadoSelecionado))
            {
                var selEstado = estadosList.FirstOrDefault(i => i.Value == estadoSelecionado);
                if (selEstado != null) selEstado.Selected = true;
            }
            ViewData["Estados"] = estadosList;

            // 2) Lista de Tipos (enum)
            var tiposList = Enum.GetValues<TipoImovel>()
                .Select(t => new SelectListItem { Value = t.ToString(), Text = t.ToString() })
                .ToList();
            if (!string.IsNullOrWhiteSpace(tipoSelecionado))
            {
                var selTipo = tiposList.FirstOrDefault(i => i.Value == tipoSelecionado);
                if (selTipo != null) selTipo.Selected = true;
            }
            ViewData["Tipos"] = tiposList;

            // 3) Status estáticos
            var statusItems = new[] { "Disponível", "Indisponível", "Vendido", "Em Leilão" }
                .Select(s => new SelectListItem { Value = s, Text = s })
                .ToList();
            if (!string.IsNullOrWhiteSpace(statusSelecionado))
            {
                var selStatus = statusItems.FirstOrDefault(i => i.Value == statusSelecionado);
                if (selStatus != null) selStatus.Selected = true;
            }
            ViewData["StatusList"] = statusItems;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? cidade,
                                               string? estado,
                                               TipoImovel? tipo,
                                               decimal? precoMaximo)
        {
            // Popula filtros
            PopularDropdowns(estado, tipo?.ToString(), null);

            var query = _context.Imoveis.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(cidade))
                query = query.Where(i => i.Cidade.Contains(cidade));
            if (!string.IsNullOrWhiteSpace(estado))
                query = query.Where(i => i.Estado == estado);
            if (tipo.HasValue)
                query = query.Where(i => i.Tipo == tipo.Value.ToString());
            if (precoMaximo.HasValue)
                query = query.Where(i => i.PrecoMinimo <= precoMaximo.Value);

            var imoveis = await query.OrderBy(i => i.Titulo).ToListAsync();
            return View(imoveis);
        }

        [HttpGet]
        public IActionResult Create()
        {
            PopularDropdowns();
            return View(new Imovel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Imovel imovel)
        {
            if (!ModelState.IsValid)
            {
                PopularDropdowns(imovel.Estado, imovel.Tipo, imovel.Status);
                return View(imovel);
            }

            // Associa ao usuário logado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userId, out var uid))
                imovel.UsuarioId = uid;

            _context.Imoveis.Add(imovel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return BadRequest();
            var imovel = await _context.Imoveis
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.ImovelId == id.Value);
            return imovel == null ? NotFound() : View(imovel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest();
            var imovel = await _context.Imoveis.FindAsync(id.Value);
            if (imovel == null) return NotFound();

            PopularDropdowns(imovel.Estado, imovel.Tipo, imovel.Status);
            return View(imovel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Imovel imovel)
        {
            if (id != imovel.ImovelId) return BadRequest();
            if (!ModelState.IsValid)
            {
                PopularDropdowns(imovel.Estado, imovel.Tipo, imovel.Status);
                return View(imovel);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userId, out var uid))
                    imovel.UsuarioId = uid;

                _context.Imoveis.Update(imovel);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Imoveis.AnyAsync(e => e.ImovelId == id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();
            var imovel = await _context.Imoveis
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.ImovelId == id.Value);
            return imovel == null ? NotFound() : View(imovel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var imovel = await _context.Imoveis.FindAsync(id);
            if (imovel != null)
            {
                _context.Imoveis.Remove(imovel);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        public IActionResult Error() => View("Error");
    }
}