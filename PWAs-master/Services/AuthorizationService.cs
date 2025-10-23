using Microsoft.Extensions.Configuration;
using NetTopologySuite.Geometries;
using PWAs.Models.Conexion;
using PWAs.Sistema;
using System.Data;
using System.Data.SqlClient;

namespace PWAs.Services
{
    public class AuthorizationService
    {
        private IConfiguration Configuration;
        string sCadenaConexion;

        public AuthorizationService(IConfiguration _config)
        {
            Configuration = _config;
        }


    }
}
