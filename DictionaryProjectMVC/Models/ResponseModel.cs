using System.Net;

namespace DictionaryApi.Models.AuthenticationDataModels
{
    public class ResponseModel
    {
        public string? StatusMessage { get; set; }

        public HttpStatusCode StatusCode { get; set; }
    }
}
