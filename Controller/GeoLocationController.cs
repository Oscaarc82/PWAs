using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using PWAs.Context;
using PWAs.Models.Geolocalizacion;
using PWAs.Services;
using System.Text.Json.Nodes;

namespace PWAs.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeoLocationController : ControllerBase
    {
        private readonly AppDbContext _ctx;
        private readonly GeometryFactory _gFc;

        public GeoLocationController(AppDbContext ctx)
        {
            _ctx = ctx;
            _gFc = new GeometryFactory(new PrecisionModel(), 4326);
        }

        [HttpPost]
        [Route("ActualizarLocacion")]
        public async Task<IActionResult> ActualizarLocacion(UpdateLocationDto locacion)
        {
            var nvaLocacion = _gFc.CreatePoint(new Coordinate(locacion.Longitude, locacion.Latitude));

            var nvoRecord = new LocationRecord
            {
                IUsuario = locacion.iUsuario,
                Timestamp = DateTime.UtcNow,
                Location = nvaLocacion
            };

            _ctx.tLocationRecords.Add(nvoRecord);
            await _ctx.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        [Route("ListarCercanos")]
        public async Task<IActionResult> ListarCercanos([FromQuery] NearbyRequestDto req)
        {
            var uLocacion = _gFc.CreatePoint(new Coordinate(req.Longitude, req.Latitude));

            var cercanos = await _ctx.tLocationRecords
                .Where(record => record.Location.IsWithinDistance(uLocacion, req.RadiusInMeters))
                .OrderBy(record => record.Location.Distance(uLocacion))
                .Select(record => new
                {
                    record.IUsuario,
                    Latitude = record.Location.Y,
                    Longitude = record.Location.X,
                    Distance = record.Location.Distance(uLocacion)
                })
                .ToListAsync();

            return Ok(new { cercanos });
        }

        public class UpdateLocationDto
        {
            [JsonRequired] public double Latitude { get; set; }
            [JsonRequired] public double Longitude { get; set; }
            [JsonRequired] public int iUsuario { get; set; }
        }

        public class NearbyRequestDto
        {
            [JsonRequired] public double Latitude { get; set; }
            [JsonRequired] public double Longitude { get; set; }
            public int RadiusInMeters { get; set; } = 1000;
        }
    }
}
