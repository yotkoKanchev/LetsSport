namespace LetsSport.Web.Areas.Administration.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Services.Data;
    using LetsSport.Services.Data.AddressServices;
    using LetsSport.Web.ViewModels.Administration.Arenas;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    [Area("Administration")]
    public class ArenasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IArenasService arenasService;
        private readonly ICountriesService countriesService;
        private readonly ISportsService sportsService;
        private readonly ICitiesService citiesService;

        public ArenasController(
            ApplicationDbContext context,
            IArenasService arenasService,
            ICountriesService countriesService,
            ISportsService sportsService,
            ICitiesService citiesService)
        {
            _context = context;
            this.arenasService = arenasService;
            this.countriesService = countriesService;
            this.sportsService = sportsService;
            this.citiesService = citiesService;
        }

        // GET: Administration/Arenas
        public async Task<IActionResult> Index()
        {
            //var applicationDbContext = _context.Arenas.Include(a => a.ArenaAdmin).Include(a => a.City).Include(a => a.Country).Include(a => a.MainImage).Include(a => a.Sport);
            //return View(await applicationDbContext.ToListAsync());

            var arenas = this.arenasService.GetAll()
              .Select(c => new ArenaInfoViewModel
              {
                  Id = c.Id,
                  Name = c.Name,
                  CityName = c.City.Name,
                  CountryName = c.Country.Name,
                  SportName = c.Sport.Name,
                  IsDeleted = c.IsDeleted,
              })
              .ToList();

            var viewModel = new ArenasIndexViewModel
            {
                Arenas = arenas,
                Filter = new ArenasFilterBarViewModel
                {
                    Countries = this.countriesService.GetAll(),
                },
            };

            return this.View(viewModel);
        }

        public IActionResult Filter(ArenasFilterBarViewModel inputModel)
        {
            ArenasIndexViewModel viewModel;

            if (inputModel.City == null && inputModel.Sport == null && inputModel.IsDeleted == null)
            {
                viewModel = this.arenasService.FilterArenasByCountryId(inputModel.Country);
            }
            else
            {
                viewModel = this.arenasService.FilterArenas(inputModel.Country, inputModel.City, inputModel.Sport, inputModel.IsDeleted);
            }

            return this.View(nameof(this.Index), viewModel);
        }

        // GET: Administration/Arenas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var arena = await _context.Arenas
                .Include(a => a.ArenaAdmin)
                .Include(a => a.City)
                .Include(a => a.Country)
                .Include(a => a.MainImage)
                .Include(a => a.Sport)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (arena == null)
            {
                return NotFound();
            }

            return View(arena);
        }

        // GET: Administration/Arenas/Create
        public IActionResult Create()
        {
            ViewData["ArenaAdminId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name");
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name");
            ViewData["MainImageId"] = new SelectList(_context.Images, "Id", "Id");
            ViewData["SportId"] = new SelectList(_context.Sports, "Id", "Id");
            return View();
        }

        // POST: Administration/Arenas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,PricePerHour,PhoneNumber,WebUrl,Email,Address,Description,Status,CountryId,CityId,SportId,MainImageId,ArenaAdminId,IsDeleted,DeletedOn,Id,CreatedOn,ModifiedOn")] Arena arena)
        {
            if (ModelState.IsValid)
            {
                _context.Add(arena);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ArenaAdminId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", arena.ArenaAdminId);
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", arena.CityId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", arena.CountryId);
            ViewData["MainImageId"] = new SelectList(_context.Images, "Id", "Id", arena.MainImageId);
            ViewData["SportId"] = new SelectList(_context.Sports, "Id", "Id", arena.SportId);
            return View(arena);
        }

        // GET: Administration/Arenas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var arena = await _context.Arenas.FindAsync(id);
            if (arena == null)
            {
                return NotFound();
            }
            ViewData["ArenaAdminId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", arena.ArenaAdminId);
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", arena.CityId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", arena.CountryId);
            ViewData["MainImageId"] = new SelectList(_context.Images, "Id", "Id", arena.MainImageId);
            ViewData["SportId"] = new SelectList(_context.Sports, "Id", "Id", arena.SportId);
            return View(arena);
        }

        // POST: Administration/Arenas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,PricePerHour,PhoneNumber,WebUrl,Email,Address,Description,Status,CountryId,CityId,SportId,MainImageId,ArenaAdminId,IsDeleted,DeletedOn,Id,CreatedOn,ModifiedOn")] Arena arena)
        {
            if (id != arena.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(arena);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArenaExists(arena.Id))
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
            ViewData["ArenaAdminId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", arena.ArenaAdminId);
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", arena.CityId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", arena.CountryId);
            ViewData["MainImageId"] = new SelectList(_context.Images, "Id", "Id", arena.MainImageId);
            ViewData["SportId"] = new SelectList(_context.Sports, "Id", "Id", arena.SportId);
            return View(arena);
        }

        // GET: Administration/Arenas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var arena = await _context.Arenas
                .Include(a => a.ArenaAdmin)
                .Include(a => a.City)
                .Include(a => a.Country)
                .Include(a => a.MainImage)
                .Include(a => a.Sport)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (arena == null)
            {
                return NotFound();
            }

            return View(arena);
        }

        // POST: Administration/Arenas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var arena = await _context.Arenas.FindAsync(id);
            _context.Arenas.Remove(arena);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArenaExists(int id)
        {
            return _context.Arenas.Any(e => e.Id == id);
        }
    }
}
