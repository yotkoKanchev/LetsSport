namespace LetsSport.Web.Areas.Administration.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Services.Data;
    using LetsSport.Services.Data.AddressServices;
    using LetsSport.Web.ViewModels.Admin;
    using LetsSport.Web.ViewModels.Admin.Events;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    [Area("Administration")]
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEventsService eventsService;
        private readonly ICountriesService countriesService;

        public EventsController(ApplicationDbContext context, IEventsService eventsService, ICountriesService countriesService)
        {
            _context = context;
            this.eventsService = eventsService;
            this.countriesService = countriesService;
        }

        public async Task<IActionResult> Index()
        {
            //var viewModel = new IndexViewModel
            //{
            //    Events = this.eventsService.GetAll<InfoViewModel>(),
            //    Filter = new FilterBarViewModel
            //    {
            //        Countries = this.countriesService.GetAll(),
            //    },
            //};

            return this.View(/*viewModel*/);
        }

        // GET: Administration/Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Admin)
                .Include(e => e.Arena)
                .Include(e => e.City)
                .Include(e => e.Country)
                .Include(e => e.Sport)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Administration/Events/Create
        public IActionResult Create()
        {
            ViewData["AdminId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            ViewData["ArenaId"] = new SelectList(_context.Arenas, "Id", "Name");
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name");
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name");
            ViewData["SportId"] = new SelectList(_context.Sports, "Id", "Id");
            return View();
        }

        // POST: Administration/Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,MinPlayers,MaxPlayers,Gender,GameFormat,DurationInHours,Date,StartingHour,AdditionalInfo,Status,RequestStatus,ArenaRequestStatus,CountryId,CityId,ArenaId,SportId,AdminId,Id,CreatedOn,ModifiedOn")] Event @event)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AdminId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", @event.AdminId);
            ViewData["ArenaId"] = new SelectList(_context.Arenas, "Id", "Name", @event.ArenaId);
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", @event.CityId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", @event.CountryId);
            ViewData["SportId"] = new SelectList(_context.Sports, "Id", "Id", @event.SportId);
            return View(@event);
        }

        // GET: Administration/Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            ViewData["AdminId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", @event.AdminId);
            ViewData["ArenaId"] = new SelectList(_context.Arenas, "Id", "Name", @event.ArenaId);
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", @event.CityId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", @event.CountryId);
            ViewData["SportId"] = new SelectList(_context.Sports, "Id", "Id", @event.SportId);
            return View(@event);
        }

        // POST: Administration/Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,MinPlayers,MaxPlayers,Gender,GameFormat,DurationInHours,Date,StartingHour,AdditionalInfo,Status,RequestStatus,ArenaRequestStatus,CountryId,CityId,ArenaId,SportId,AdminId,Id,CreatedOn,ModifiedOn")] Event @event)
        {
            if (id != @event.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
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
            ViewData["AdminId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", @event.AdminId);
            ViewData["ArenaId"] = new SelectList(_context.Arenas, "Id", "Name", @event.ArenaId);
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", @event.CityId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", @event.CountryId);
            ViewData["SportId"] = new SelectList(_context.Sports, "Id", "Id", @event.SportId);
            return View(@event);
        }

        // GET: Administration/Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Admin)
                .Include(e => e.Arena)
                .Include(e => e.City)
                .Include(e => e.Country)
                .Include(e => e.Sport)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Administration/Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
    }
}
