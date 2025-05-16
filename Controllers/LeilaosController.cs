using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LanceCertoNovo.Data;
using LanceCertoNovo.Models;

namespace LanceCertoNovo.Controllers
{
    public class LeilaosController : Controller
    {
        private readonly LanceCertoContext _context;

        public LeilaosController(LanceCertoContext context)
        {
            _context = context;
        }

        // GET: Leilaos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Leiloes.ToListAsync());
        }

        // GET: Leilaos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leilao = await _context.Leiloes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leilao == null)
            {
                return NotFound();
            }

            return View(leilao);
        }

        // GET: Leilaos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Leilaos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titulo,Descricao,ValorInicial,DataInicio,DataFim")] Leilao leilao)
        {
            if (ModelState.IsValid)
            {
                _context.Add(leilao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(leilao);
        }

        // GET: Leilaos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leilao = await _context.Leiloes.FindAsync(id);
            if (leilao == null)
            {
                return NotFound();
            }
            return View(leilao);
        }

        // POST: Leilaos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Descricao,ValorInicial,DataInicio,DataFim")] Leilao leilao)
        {
            if (id != leilao.Id)
            {
                return NotFound();
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
                    if (!LeilaoExists(leilao.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(leilao);
        }

        // GET: Leilaos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leilao = await _context.Leiloes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leilao == null)
            {
                return NotFound();
            }

            return View(leilao);
        }

        // POST: Leilaos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var leilao = await _context.Leiloes.FindAsync(id);
            if (leilao != null)
            {
                _context.Leiloes.Remove(leilao);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LeilaoExists(int id)
        {
            return _context.Leiloes.Any(e => e.Id == id);
        }
    }
}
