using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

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

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();

            PopulateSatelliteObjects(celestialObjects);
            return Ok(celestialObjects);
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Where(x => x.Id == id).FirstOrDefault();

            if (celestialObject == null)
            {
                return NotFound();
            }

            var satelliteObjects = _context.CelestialObjects.Where(x => x.OrbitedObjectId == id).ToList();
            celestialObject.Satellites = satelliteObjects;

            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToList();

            if (celestialObjects == null || !celestialObjects.Any())
            {
                return NotFound();
            }

            PopulateSatelliteObjects(celestialObjects);
            return Ok(celestialObjects);
        }

        private void PopulateSatelliteObjects(List<CelestialObject> celestialObjects)
        {
            foreach (var celestial in celestialObjects)
            {
                var satelliteObjects = _context.CelestialObjects.Where(x => x.OrbitedObjectId == celestial.Id).ToList();
                celestial.Satellites = satelliteObjects;
            }
        }

        private readonly ApplicationDbContext _context;
    }
}
