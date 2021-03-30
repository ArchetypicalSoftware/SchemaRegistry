using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Archetypical.Software.SchemaRegistry.Models;
using Archetypical.Software.SchemaRegistry.Shared.Data;
using Archetypical.Software.SchemaRegistry.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Archetypical.Software.SchemaRegistry.Controllers
{
    public class HomeController : Controller
    {
        private readonly Context _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(Context context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int? page, int? size)
        {
            var groups = await _context
                .SchemaGroups
                .OrderBy(x => x.Id)
                .Skip(page.GetValueOrDefault(0) * size.GetValueOrDefault(10))
                .Take(size.GetValueOrDefault(10)).ToListAsync();

            return View(groups);
        }

        public async Task<IActionResult> Schema(string id, string schemaId, int? version)
        {
            Schema model = null;
            if (version.HasValue)
            {
                model = await _context.Schemata.FirstOrDefaultAsync(x =>
                    x.SchemaGroupId == id && x.Id == schemaId && x.Version == version);
            }
            else
            {
                model = await _context.Schemata.Where(x =>
                        x.SchemaGroupId == id && x.Id == schemaId)
                    .OrderByDescending(x => x.Version)
                    .FirstOrDefaultAsync();
            }
            return View(model);
        }

        public async Task<IActionResult> Diff(string id, string schemaId, int version, int previousVersion)
        {
            var model = await _context.Schemata
                .Where(x => x.SchemaGroupId == id && x.Id == schemaId && x.Version == version)
                .Select(x => x.Format)
                .FirstOrDefaultAsync();
            return View(new DiffModel
            {
                GroupId = id,
                SchemaId = schemaId,
                Format = model.GetValueOrDefault(),
                Version = version,
                PreviousVersion = previousVersion
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}