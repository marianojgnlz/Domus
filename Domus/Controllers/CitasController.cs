﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Domus.Models;
using Domus.ViewModels;

namespace Domus.Controllers
{
    public class CitasController : Controller
    {
        private readonly TPIContext _context;
        private readonly IClientesService _clientesService;
        private readonly ICalendarioService _calendarioService;


        public CitasController(TPIContext context, IClientesService clientesService, ICalendarioService calendarioService)
        {
            _context = context;
            _clientesService = clientesService;
            _calendarioService = calendarioService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult<Request<Cliente>> BuscarCliente([FromForm] CitaView cita)
        {
            Request<Cliente> req = _clientesService.GetClienteByDNI(cita.Cliente.DNI);
            if (req.Success)
            {
                cita.Cliente = req.Data;

                return View("Create", cita);
            }
            else
            {
                return BadRequest(req);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult<Request<Calendario>> BuscarFecha([FromForm] CitaView citaView)
        {
            Request<Cliente> reqCliente = _clientesService.GetClienteByDNI(citaView.Cliente.DNI);
            citaView.Cliente = reqCliente.Data;
            Request<Calendario> reqCalendario = _calendarioService.GetCalendarioByFecha((DateTime) citaView.FechaBuscada);
            if (reqCalendario.Success)
            {
                citaView.Horarios = reqCalendario.Data.Horarios.ToList();
                return View("Create", citaView);
            }
            else
            {
                return BadRequest(reqCalendario);
            }
        }

        // GET: Citas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Cita.ToListAsync());
        }

        // GET: Citas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cita = await _context.Cita
                .FirstOrDefaultAsync(m => m.IdCita == id);
            if (cita == null)
            {
                return NotFound();
            }

            return View(cita);
        }

        // GET: Citas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Citas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCita,Hora,Fecha")] Cita cita)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cita);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cita);
        }

        // GET: Citas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cita = await _context.Cita.FindAsync(id);
            if (cita == null)
            {
                return NotFound();
            }
            return View(cita);
        }

        // POST: Citas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCita,Hora,Fecha")] Cita cita)
        {
            if (id != cita.IdCita)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cita);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CitaExists(cita.IdCita))
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
            return View(cita);
        }

        // GET: Citas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cita = await _context.Cita
                .FirstOrDefaultAsync(m => m.IdCita == id);
            if (cita == null)
            {
                return NotFound();
            }

            return View(cita);
        }

        // POST: Citas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cita = await _context.Cita.FindAsync(id);
            _context.Cita.Remove(cita);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CitaExists(int id)
        {
            return _context.Cita.Any(e => e.IdCita == id);
        }
    }
}
