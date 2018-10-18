using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Tatoeba.Mobile.Services
{   
    public class HttpTatotebaClient
    {       
        private HttpClient client;
        private HttpClientHandler handler;

        public HttpTatotebaClient()
        {
            InitClient(new CookieContainer());
        }

        public void InitClient(CookieContainer cookieContainer)
        {
            handler = new HttpClientHandler()
            {
                AllowAutoRedirect = true,
                CookieContainer = cookieContainer,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            client = new HttpClient(handler)
            {
                DefaultRequestHeaders =
                {
                    CacheControl = CacheControlHeaderValue.Parse("no-cache, must-revalidate"),
                }
            };
        }

        public CookieContainer Cookies
        {
            get => handler.CookieContainer;
            set => InitClient(value);
        }
        
        public async Task<T> GetAsync<T>(string requestUri, Type[] parsers) where T : class
        {
            var resp = await GetStringAsync(requestUri);
            return Deserialize<T>(resp, parsers);
        }

        public async Task<T> PostAsync<T>(string requestUri, string postData, Type[] parsers) where T : class
        {
            var resp = await PostAsync(requestUri, postData);
            return Deserialize<T>(resp, parsers);
        }

        public T Deserialize<T>(string resp, Type[] parsers) where T : class
        {
            var resType = typeof(T);

            if (resType == typeof(string))
            {
                return resp as T;
            }

            var m = parsers.SelectMany(t => t.GetMethods())
              .Where(x => x.IsPublic
              && x.IsStatic
              && x.ReturnType == resType
              && x.GetParameters().Count() == 1
              && x.GetParameters().First().ParameterType == typeof(string))
              .LastOrDefault() ?? throw new Exception("No method returns " + resType.Name);

            return m.Invoke(null, new object[] { resp }) as T;
        }


        /// <summary>Gets the text of page from web.</summary>
        /// <param name="requestUri">Absolute URI of page to get.</param>
        /// <returns>Returns source code.</returns>
        public async Task<string> GetStringAsync(string requestUri)
            => await client.GetStringAsync(requestUri);

        /// <summary>Posts specified string to requested resource
        /// and gets the result text.</summary>
        /// <param name="requestUri">Absolute URI of page to get.</param>
        /// <param name="postData">String to post to site with web request.</param>
        /// <returns>Returns text.</returns>
        public async Task<string> PostAsync(string requestUri, string postData)        
           => await (await client.PostAsync(requestUri, new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded")))
            .Content?.ReadAsStringAsync();
    }

}
