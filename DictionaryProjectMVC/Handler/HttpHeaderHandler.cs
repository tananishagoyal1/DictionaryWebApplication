using DictionaryApp.Services;
using System.Net.Http.Headers;


namespace DictionaryApp.Handler
{
    public class HttpHeaderHandler : DelegatingHandler
    {
        private readonly ITokenService _tokenService;
        public HttpHeaderHandler(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = _tokenService.GetAccessToken();
            if (token != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }            
            return base.SendAsync(request, cancellationToken);
        }
    }
}
