using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tatoeba.Mobile.Services
{
   
    public class HttpTatotebaClient
    {
        /// <summary>Site's cookies.</summary>
        public CookieContainer cookies = new CookieContainer();

        /// <summary>This is a maximum degree of server load when bot is
        /// still allowed to edit pages. Higher values mean more aggressive behaviour.
        /// See <see href="https://www.mediawiki.org/wiki/Manual:Maxlag_parameter">this page</see>
        /// for details.</summary>
        public int MaxLag { get; set; } = 5;

        /// <summary>Number of times to retry bot web action in case of temporary connection
        ///  failure or some server problems.</summary>
        public int RetryCountPerRequest { get; set; } = 3;


        public async Task<T> GetAsync<T>(string requestUri, Type[] parsers) where T : class
        {
            var resp = await GetAsync(requestUri);
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
        public async Task<string> GetAsync(string requestUri)
            => await MakeHttpRequestAsync(requestUri, null, false, true);

        /// <summary>gets specified string to requested resource
        /// and gets the result text.</summary>
        /// <param name="requestUri">Absolute URI of page to get.</param>        
        /// <returns>Returns text.</returns>
        public async Task<string> GetAndSaveCookiesAsync(string requestUri, bool allowRedirect = false)
            => await MakeHttpRequestAsync(requestUri, null, true, allowRedirect);

        /// <summary>Posts specified string to requested resource
        /// and gets the result text.</summary>
        /// <param name="requestUri">Absolute URI of page to get.</param>
        /// <param name="postData">String to post to site with web request.</param>
        /// <returns>Returns text.</returns>
        public async Task<string> PostAsync(string requestUri, string postData)
            => await MakeHttpRequestAsync(requestUri, postData, false, true);

        /// <summary>Posts specified string to requested resource
        /// and gets the result text.</summary>
        /// <param name="requestUri">Absolute URI of page to get.</param>
        /// <param name="postData">String to post to site with web request.</param>
        /// <returns>Returns text.</returns>
        public async Task<string> PostAndSaveCookiesAsync(string requestUri, string postData, bool allowRedirect = false)
            => await MakeHttpRequestAsync(requestUri, postData, true, allowRedirect);

        /// <summary>Posts specified string to requested resource
        /// and gets the result text.</summary>
        /// <param name="requestUri">Absolute URI of page to get.</param>
        /// <param name="postData">String to post to site with web request.</param>
        /// <param name="saveCookies">If set to true, gets cookies from web response and
        /// saves it in Site.cookies container.</param>
        /// <param name="allowRedirect">Allow auto-redirection of web request by server.</param>
        /// <returns>Returns text.</returns>
        private async Task<string> MakeHttpRequestAsync(string requestUri, string postData, bool saveCookies, bool allowRedirect = false)
        {
            if (string.IsNullOrEmpty(requestUri))
            {
                throw new ArgumentNullException(nameof(requestUri), "No URL specified.");
            }

            HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(requestUri);
            webReq.Proxy.Credentials = CredentialCache.DefaultCredentials;
            webReq.UseDefaultCredentials = true;
            webReq.ContentType = "application/x-www-form-urlencoded";
            webReq.Headers.Add("Cache-Control", "no-cache, must-revalidate");
            webReq.AllowAutoRedirect = allowRedirect;
            webReq.CookieContainer = cookies;
            webReq.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");

            if (!string.IsNullOrEmpty(postData))
            {
                webReq.Method = "POST";
                byte[] postBytes = Encoding.UTF8.GetBytes(postData);
                webReq.ContentLength = postBytes.Length;

                Stream reqStrm = await webReq.GetRequestStreamAsync();
                await reqStrm.WriteAsync(postBytes, 0, postBytes.Length);
                reqStrm.Close();
            }


            HttpWebResponse webResp = (HttpWebResponse)(await webReq.GetResponseAsync());



            Stream respStream = webResp.GetResponseStream();
            if (webResp.ContentEncoding.ToLower().Contains("gzip"))
            {
                respStream = new GZipStream(respStream, CompressionMode.Decompress);
            }
            else if (webResp.ContentEncoding.ToLower().Contains("deflate"))
            {
                respStream = new DeflateStream(respStream, CompressionMode.Decompress);
            }

            if (saveCookies)
            {
                Uri siteUri = new Uri(requestUri);

                foreach (Cookie cookie in webResp.Cookies)
                {
                    if (cookie.Domain[0] == '.' &&
                        cookie.Domain.Substring(1) == siteUri.Host)
                    {
                        cookie.Domain = cookie.Domain.TrimStart(new char[] { '.' });
                    }

                    cookies.Add(cookie);
                }
            }

            StreamReader strmReader = new StreamReader(respStream, Encoding.UTF8);
            string respStr = await strmReader.ReadToEndAsync();
            strmReader.Close();
            webResp.Close();
            return respStr;

        }

    }

}
