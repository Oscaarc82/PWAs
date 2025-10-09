using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;

namespace PWAs.Models.Geolocalizacion
{
    public class LocationRecord
    {
        [Key]
        public int IId { get; set; }
        [Required]
        public int IUsuario {  get; set; }
        public DateTime Timestamp { get; set; }
        [Required]
        public Point Location { get; set; }
    }
}
