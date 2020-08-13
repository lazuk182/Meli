using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace Meli.Controller
{
    public class MercadoLibreController
    {
        public MercadoLibreController(long ClientID, string SecretKey, string ServerCode, string RedirectUrl)
        {
            Init(ClientID, SecretKey, ServerCode, RedirectUrl);
            var AuthorizationObject = GenerateAccessToken();
            AccessToken = AuthorizationObject.access_token;
            this.ServerCode = AuthorizationObject.refresh_token;
        }
        public MercadoLibreController(long ClientID, string SecretKey, string ServerCode)
        {
            Init(ClientID, SecretKey, ServerCode, "");
            var AuthorizationObject = RefreshAccessToken(ServerCode);
            AccessToken = AuthorizationObject.access_token;
            this.ServerCode = AuthorizationObject.refresh_token;
        }
        private void Init(long ClientID, string SecretKey, string ServerCode, string RedirectUrl)
        {
            this.ClientID = ClientID;
            this.SecretKey = SecretKey;
            this.ServerCode = ServerCode;
            this.RedirectUrl = RedirectUrl;
        }
        private Models.Authorization RefreshAccessToken(string RefreshToken)
        {
            string AppID = Convert.ToString(ClientID);
            string AuthorizationCodeUrl = "https://api.mercadolibre.com/oauth/";
            string AuthorizationCodeUri = "token?grant_type=refresh_token&client_id=$APP_ID&client_secret=$SECRET_KEY&refresh_token=$REFRESH_TOKEN";
            AuthorizationCodeUri = AuthorizationCodeUri.Replace("$APP_ID", AppID);
            AuthorizationCodeUri = AuthorizationCodeUri.Replace("$SECRET_KEY", SecretKey);
            AuthorizationCodeUri = AuthorizationCodeUri.Replace("$REFRESH_TOKEN", RefreshToken);
            return Post<Models.Authorization>(AuthorizationCodeUrl, AuthorizationCodeUri, null).GetAwaiter().GetResult();
        }

        private Models.Authorization GenerateAccessToken()
        {
            string AppID = Convert.ToString(ClientID);
            string AuthorizationCodeUrl = "https://api.mercadolibre.com/oauth/";
            string AuthorizationCodeUri = "token?grant_type=authorization_code&client_id=$APP_ID&client_secret=$SECRET_KEY&code=$SERVER_GENERATED_AUTHORIZATION_CODE&redirect_uri=$REDIRECT_URI";
            AuthorizationCodeUri = AuthorizationCodeUri.Replace("$APP_ID", AppID);
            AuthorizationCodeUri = AuthorizationCodeUri.Replace("$SECRET_KEY", SecretKey);
            AuthorizationCodeUri = AuthorizationCodeUri.Replace("$SERVER_GENERATED_AUTHORIZATION_CODE", ServerCode);
            AuthorizationCodeUri = AuthorizationCodeUri.Replace("$REDIRECT_URI", RedirectUrl);
            return Post<Models.Authorization>(AuthorizationCodeUrl, AuthorizationCodeUri, null).GetAwaiter().GetResult();
        }

        public long ClientID { get; private set; }
        public string SecretKey { get; private set; }
        public string ServerCode { get; set; }
        public string RedirectUrl { get; set; }
        private string AccessToken { get; set; }
        public List<Models.Order> GetOrders(string Seller, DateTime From)
        {
            List<Models.Order> Orders = new List<Models.Order>();
            _ = new Models.OrderResponse();
            int Total = 0;
            int Offset = 0;
            do
            {
                string Url = "https://api.mercadolibre.com/orders/search/";
                string Uri = "?seller=" + Seller + "&order.status=paid&order.date_closed.from="
                    + From.ToString("yyyy-MM-dd") + "T00:00:00.000-00:00&access_token="
                    + AccessToken + "&offset=" + Offset;
                Models.OrderResponse Response = Get<Models.OrderResponse>(Url, Uri).GetAwaiter().GetResult();
                Total = Convert.ToInt32(Response.paging.total);
                Orders.AddRange(Response.results);
                Offset += 50;
            } while (Total > Orders.Count);
            return Orders;
        }

        public Models.Shipment GetShipment(string ShipmentId)
        {
            string Url = "https://api.mercadolibre.com/shipments/";
            string Uri = ShipmentId + "?access_token=" + AccessToken;
            var shipment = Get<Models.Shipment>(Url, Uri).GetAwaiter().GetResult();
            return shipment;
        }

        public void AddNote(string OrderID, string Note)
        {
            string Url = "https://api.mercadolibre.com/orders/" + OrderID + "/";
            string Uri = "notes?access_token=" + AccessToken;
            HttpContent _data = CreateHttpContent(new { note = Note });
            Post<object>(Url, Uri, _data).GetAwaiter().GetResult();
        }

        public string DownloadTrackingPdf(Models.Order order)
        {
            try
            {
                string url = "https://api.mercadolibre.com/shipment_labels?shipment_ids="
                    + order.shipping.id + "&response_type=pdf&access_token=" + AccessToken;
                var RedirectedUrl = GetFinalRedirect(url, new List<Cookie>());
                string Path = HttpContext.Current.Server.MapPath("~/temp") + @"\";
                using (WebClient WebClient = new WebClient())
                {
                    string myStringWebResource = RedirectedUrl;
                    WebClient.DownloadFile(myStringWebResource, Path + order.id + ".pdf");
                }
                return Path + order.id + ".pdf";
            }
            catch (Exception e)
            {
                return "Exception caught => " + e.Message;
            }
        }

        #region Private Methods
        private async Task<T> Get<T>(string Url, string Uri)
        {
            using (var handler = new HttpClientHandler())
            {
                handler.PreAuthenticate = true;
                using (var client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri(Url);
                    var result = await client.GetAsync(Uri);
                    result.EnsureSuccessStatusCode();
                    string resultContentString = await result.Content.ReadAsStringAsync();
                    T resultContent = JsonConvert.DeserializeObject<T>(resultContentString);
                    return resultContent;
                }
            }
        }
        private async Task<T> Post<T>(string Url, string Uri, HttpContent data)
        {
            using (var handler = new HttpClientHandler())
            {
                handler.PreAuthenticate = true;
                using (var client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri(Url);
                    var result = await client.PostAsync(Uri, data);
                    result.EnsureSuccessStatusCode();
                    string resultContentString = await result.Content.ReadAsStringAsync();
                    T resultContent = JsonConvert.DeserializeObject<T>(resultContentString);
                    return resultContent;
                }
            }
        }
        private string GetFinalRedirect(string url, List<Cookie> Cookies)
        {
            if (string.IsNullOrWhiteSpace(url))
                return url;

            int maxRedirCount = 8;  // prevent infinite loops
            string newUrl = url;
            do
            {
                HttpWebResponse resp = null;
                try
                {
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                    req.Method = "GET";
                    req.AllowAutoRedirect = false;
                    req.Timeout = 10000;
                    CookieContainer cookieContainer = new CookieContainer();
                    foreach (var cookie in Cookies)
                    {
                        cookieContainer.Add(new Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain));
                    }
                    req.CookieContainer = cookieContainer;
                    resp = (HttpWebResponse)req.GetResponse();
                    switch (resp.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            return newUrl;
                        case HttpStatusCode.Redirect:
                        case HttpStatusCode.MovedPermanently:
                        case HttpStatusCode.RedirectKeepVerb:
                        case HttpStatusCode.RedirectMethod:
                            newUrl = resp.Headers["Location"];
                            if (newUrl == null)
                                return url;

                            if (newUrl.IndexOf("://", System.StringComparison.Ordinal) == -1)
                            {
                                // Doesn't have a URL Schema, meaning it's a relative or absolute URL
                                Uri u = new Uri(new Uri(url), newUrl);
                                newUrl = u.ToString();
                            }
                            break;
                        default:
                            return newUrl;
                    }
                    url = newUrl;
                }
                catch (WebException)
                {
                    // Return the last known good URL
                    return newUrl;
                }
                catch
                {
                    return null;
                }
                finally
                {
                    if (resp != null)
                        resp.Close();
                }
            } while (maxRedirCount-- > 0);

            return newUrl;
        }
        private static void SerializeJsonIntoStream(object value, Stream stream)
        {
            using (var sw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true))
            using (var jtw = new JsonTextWriter(sw) { Formatting = Formatting.None })
            {
                var js = new JsonSerializer();
                js.Serialize(jtw, value);
                jtw.Flush();
            }
        }
        private static HttpContent CreateHttpContent(object content)
        {
            HttpContent httpContent = null;
            if (content != null)
            {
                var ms = new MemoryStream();
                SerializeJsonIntoStream(content, ms);
                ms.Seek(0, SeekOrigin.Begin);
                httpContent = new StreamContent(ms);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }
            return httpContent;
        }
        #endregion
    }
}
