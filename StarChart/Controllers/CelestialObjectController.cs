using Microsoft.AspNetCore.Mvc;
using StarChart.Data;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        public CelestialObjectController(ApplicationDbContext context) : base()
        {
            _context = context;
        }

        private readonly ApplicationDbContext _context;
    }
}
