using System.Text.Json.Serialization;

namespace PWAs.Sistema
{
    public class Response
    {
        public string statusCode { get; set; }
        public string status { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string sMensaje { get; set; }
        [JsonIgnore]
        public int iTipoError { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<object> lstDatosNP { get; set; }
    }
}
