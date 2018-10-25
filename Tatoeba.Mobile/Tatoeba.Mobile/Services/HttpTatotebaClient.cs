using ModernHttpClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Tatoeba.Mobile.Services
{
    public class InvalidSessionException : Exception { }

    public enum TatoebaStatus { Success, InvalidSession, ParsingError, Error, }

    public class TatoebaResponse
    {
        public string Error { get; set; }        
        public TatoebaStatus Status { get; set; }
    }

    public class TatoebaResponse<T> : TatoebaResponse
    {   
        public T Content { get; set; }        
    }

    public class HttpTatotebaClient
    {
        private int timeout_in_seconds;
        private HttpClient client;
        private HttpClientHandler handler;
        NativeCookieHandler cookieHandler;


        public HttpTatotebaClient() : this(10) { }

        public HttpTatotebaClient(int timeout_in_seconds)
        {
            this.timeout_in_seconds = timeout_in_seconds;
            InitClient(new CookieContainer());
        }        

        public void InitClient(CookieContainer cookieContainer)
        {
            if(client != null)
            {
                client.Dispose();
            }

            if (Device.RuntimePlatform == Device.Android
                || Device.RuntimePlatform == Device.iOS)
            {
                var cookies = cookieContainer.List();
                cookieHandler = new NativeCookieHandler();
                cookieHandler.SetCookies(cookieContainer.List());              

                handler = new NativeMessageHandler(false, false, cookieHandler)
                {
                    AllowAutoRedirect = true,
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };
            }
            else
            {
                handler = new HttpClientHandler
                {
                    CookieContainer = cookieContainer,
                    AllowAutoRedirect = true,
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };
            }

            client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(timeout_in_seconds),
                DefaultRequestHeaders =
                {
                    CacheControl = CacheControlHeaderValue.Parse("no-cache, must-revalidate"),            
                }
            };

            client.DefaultRequestHeaders.UserAgent.ParseAdd("Tatoeba.Mobile/1.1");
        }

        public CookieContainer Cookies
        {
            get
            {               
                if (Device.RuntimePlatform == Device.Android
                    || Device.RuntimePlatform == Device.iOS)
                {   
                    CookieContainer cookiecontainer = new CookieContainer();

                    foreach (var cookie in cookieHandler.Cookies)
                    {
                        cookiecontainer.Add(cookie);
                    }

                    return cookiecontainer;
                }
                else
                {
                    return handler.CookieContainer;
                }

            }

            set => InitClient(value);
        }

        
        public async Task<TatoebaResponse<T>> GetAsync<T>(string requestUri) where T : class
        {
            HttpResponseMessage resp;
            var cts = new CancellationTokenSource();

            try
            {   
                resp = await client.GetAsync(requestUri, cts.Token).ConfigureAwait(false);
                resp.EnsureSuccessStatusCode();
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken == cts.Token)
                {
                    return new TatoebaResponse<T>
                    {
                        Status = TatoebaStatus.Error,
                        Error = ex.Message,
                    };
                }
                else
                {
                    return new TatoebaResponse<T>
                    {
                        Status = TatoebaStatus.Error,
                        Error = "The operation has timed out.",
                    };
                }
            }
            catch (Exception ex)
            {
                return new TatoebaResponse<T>
                {
                    Status = TatoebaStatus.Error,
                    Error = ex.Message,
                };
            }

            string respStr = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
            return Deserialize<T>(respStr);
        }


        public async Task<byte[]> GetByteArrayAsync(string requestUri)
        {
            try
            {
                return await client.GetByteArrayAsync(requestUri).ConfigureAwait(false);                
            }           
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<TatoebaResponse<T>> PostAsync<T>(string requestUri, string postData) where T : class
        {
            HttpResponseMessage resp;
            var cts = new CancellationTokenSource();

            try
            {
                resp = await client.PostAsync(requestUri, new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded"), cts.Token);
                resp.EnsureSuccessStatusCode();
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken == cts.Token)
                {
                    return new TatoebaResponse<T>
                    {
                        Status = TatoebaStatus.Error,
                        Error = ex.Message,
                    };
                }
                else
                {
                    return new TatoebaResponse<T>
                    {
                        Status = TatoebaStatus.Error,
                        Error = "The operation has timed out.",
                    };
                }
            }
            catch (Exception ex)
            {
                return new TatoebaResponse<T>
                {
                    Status = TatoebaStatus.Error,
                    Error = ex.Message,
                };
            }

            string respStr = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
            return Deserialize<T>(respStr);
        }

        public TatoebaResponse<T> Deserialize<T>(string resp) where T : class
        {
            var resType = typeof(T);

            if (resType == typeof(string))
            {
                return new TatoebaResponse<T>
                {
                    Content = resp as T,
                };
            }

            var m = typeof(TatoebaScraper).GetMethods()
              .Where(x => x.IsPublic
              && x.IsStatic
              && x.ReturnType == resType
              && x.GetParameters().Count() == 1
              && x.GetParameters().First().ParameterType == typeof(string))
              .LastOrDefault() ?? throw new Exception("No method returns " + resType.Name);      

            try
            {
                return new TatoebaResponse<T>
                {
                    Content = m.Invoke(null, new object[] { resp }) as T,
                    Status = TatoebaStatus.Success,
                };
            }
            catch(InvalidSessionException)
            {
                return new TatoebaResponse<T>
                {
                    Status = TatoebaStatus.InvalidSession,
                };
            }
            catch (Exception ex)
            {
                return new TatoebaResponse<T>
                {
                    Status = TatoebaStatus.ParsingError,
                    Error =  ex.Message,
                };
            }
        }
    }

    public static class CookieContainerExtension
    {
        public static List<Cookie> List(this CookieContainer container)
        {
            var cookies = new List<Cookie>();

            var table = (Hashtable)container.GetType().InvokeMember("m_domainTable",
                                                                    BindingFlags.NonPublic |
                                                                    BindingFlags.GetField |
                                                                    BindingFlags.Instance,
                                                                    null,
                                                                    container,
                                                                    new object[] { });

            foreach (var key in table.Keys)
            {
                if (!(key is string domain))
                    continue;

                if (domain.StartsWith("."))
                {
                    domain = domain.Substring(1);
                }

                var address = string.Format("http://{0}/", domain);

                if (Uri.TryCreate(address, UriKind.RelativeOrAbsolute, out Uri uri) == false)
                    continue;

                foreach (Cookie cookie in container.GetCookies(uri))
                {
                    cookies.Add(cookie);
                }
            }

            return cookies;
        }
    }

}
