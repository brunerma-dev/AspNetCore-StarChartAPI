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

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjects = _context.CelestialObjects.Where(x => x.Id == id || x.OrbitedObjectId == id).ToList();

            if(celestialObjects == null || !celestialObjects.Any())
            {
                return NotFound();
            }

            _context.CelestialObjects.RemoveRange(celestialObjects);
            _context.SaveChanges();
            return NoContent();
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

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestialObject = _context.CelestialObjects.Where(x => x.Id == id).FirstOrDefault();

            if (celestialObject == null)
            {
                return NotFound();
            }

            celestialObject.Name = name;
            _context.Update(celestialObject);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject updatedObject)
        {
            var celestialObject = _context.CelestialObjects.Where(x => x.Id == id).FirstOrDefault();

            if (celestialObject == null)
            {
                return NotFound();
            }

            celestialObject.Name = updatedObject.Name;
            celestialObject.OrbitalPeriod = updatedObject.OrbitalPeriod;
            celestialObject.OrbitedObjectId = updatedObject.OrbitedObjectId;
            _context.Update(celestialObject);
            _context.SaveChanges();

            return NoContent();
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
