using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LanceCerto.WebApp.Data;
using LanceCerto.WebApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace LanceCerto.WebApp.Controllers
{
    [Authorize]
    public class LeilaoController : Controller
    {
        private readonly LanceCertoDbContext _context;

        public LeilaoController(LanceCertoDbContext context)
        {
            _context = context;
        }

        // GET: Leilao
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var leiloes = await _context.Leiloes
                .Include(l => l.Imovel)
                .Include(l => l.Vencedor)
                .OrderByDescending(l => l.FimEm)
                .ToListAsync();

            return View(leiloes);
        }

        // GET: Leilao/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return BadRequest();

            var leilao = await _context.Leiloes
                .Include(l => l.Imovel)
                .Include(l => l.Vencedor)
                .FirstOrDefaultAsync(m => m.LeilaoId == id);

            return leilao == null ? NotFound() : View(leilao);
        }

        // GET: Leilao/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["ImovelId"] = new SelectList(_context.Imoveis, "ImovelId", "Titulo");
            ViewBag.StatusList = new SelectList(new[] { "PENDENTE", "ATIVO", "ENCERRADO" });
            return View();
        }

        // POST: Leilao/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LeilaoId,ImovelId,InicioEm,FimEm,Status,MaiorLanceAtual")] Leilao leilao)
        {
            if (leilao.InicioEm >= leilao.FimEm)
            {
                ModelState.AddModelError("FimEm", "A data de fim deve ser posterior à data de início.");
            }

            if (ModelState.IsValid)
            {
                leilao.MaiorLanceAtual = 0;
                _context.Add(leilao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ImovelId"] = new SelectList(_context.Imoveis, "ImovelId", "Titulo", leilao.ImovelId);
            ViewBag.StatusList = new SelectList(new[] { "PENDENTE", "ATIVO", "ENCERRADO" }, leilao.Status);

            return View(leilao);
        }

        // GET: Leilao/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest();

            var leilao = await _context.Leiloes.FindAsync(id);
            if (leilao == null) return NotFound();

            ViewData["ImovelId"] = new SelectList(_context.Imoveis, "ImovelId", "Titulo", leilao.ImovelId);
            ViewData["VencedorId"] = new SelectList(_context.Users, "Id", "Email", leilao.VencedorId);

            return View(leilao);
        }

        // POST: Leilao/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LeilaoId,ImovelId,VencedorId,InicioEm,FimEm,Status,MaiorLanceAtual")] Leilao leilao)
        {
            if (id != leilao.LeilaoId) return BadRequest();

            if (leilao.InicioEm >= leilao.FimEm)
            {
                ModelState.AddModelError("FimEm", "A data de fim deve ser posterior à data de início.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(leilao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Leiloes.AnyAsync(e => e.LeilaoId == leilao.LeilaoId))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["ImovelId"] = new SelectList(_context.Imoveis, "ImovelId", "Titulo", leilao.ImovelId);
            ViewData["VencedorId"] = new SelectList(_context.Users, "Id", "Email", leilao.VencedorId);

            return View(leilao);
        }

        // GET: Leilao/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            var leilao = await _context.Leiloes
                .Include(l => l.Imovel)
                .Include(l => l.Vencedor)
                .FirstOrDefaultAsync(m => m.LeilaoId == id);

            return leilao == null ? NotFound() : View(leilao);
        }

        // POST: Leilao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var leilao = await _context.Leiloes.FindAsync(id);
            if (leilao != null)
            {
                _context.Leiloes.Remove(leilao);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}