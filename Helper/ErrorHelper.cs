using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PWAs.Helper
{
    public class ErrorHelper
    {
        public static ResponseObject Response(int StatusCode, string Message, string HttpMessage)
        {
            return new ResponseObject()
            {
                httpCode = StatusCode, //Custom
                httpMessage = HttpMessage,
                moreInformation = Message
            };
        }

        public static ModeloToken ResponseToken(int StatusCode, string sToken, string sExpire)
        {
            return new ModeloToken()
            {
                httpCode = StatusCode, //Custom
                token = sToken,
                expire_in = sExpire
            };
        }

        public static List<ModelErrors> GetModelStateErrors(ModelStateDictionary Model)
        {
            return Model.Select(x => new ModelErrors() { Type = "M", Key = x.Key, Messages = x.Value.Errors.Select(y => y.ErrorMessage).ToList() }).ToList();
        }

        public class ModeloToken
        {
            public int httpCode { get; set; }
            public string token { get; set; }
            public string expire_in { get; set; }
        }

        public class ResponseObject
        {

            public int httpCode { get; set; }
            public string httpMessage { get; set; }
            public string moreInformation { get; set; }
        }

        public class ModelErrors
        {
            public string Type { get; set; }
            public string Key { get; set; }
            public List<string> Messages { get; set; }
        }
    }
}
