using Meli.Controller;
using MercadoLibre.SDK.Meta;
using MercadoLibre.SDK;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Meli
{
    class Program
    {
        static void Main(string[] args)
        {            
            Database.Usuarios usr = new Database.Usuarios();
            MercadoLibreController MeliController;
            using (var _context = new Database.MercadoLibreEntities())
            {
                usr = _context.Usuarios.FirstOrDefault(u => u.Id == 1);
                if (usr.ServerCode == string.Empty)
                {
                    var m = new MercadoLibre.SDK.MeliApiService
                    {
                        Credentials = new MeliCredentials(MeliSite.Mexico, usr.ClientId, usr.ClientSecret)
                    };
                    string RedirectUrl = m.GetAuthUrl(usr.ClientId, MeliSite.Mexico, usr.RedirectUrl);
                    RedirectUrl = RedirectUrl.Replace("&", "^&");
                    Server(usr, RedirectUrl);
                    MeliController = new MercadoLibreController(usr.ClientId,
                        usr.ClientSecret, usr.ServerCode, usr.RedirectUrl);
                }
                else
                {
                    MeliController = new MercadoLibreController(usr.ClientId,
                        usr.ClientSecret, usr.ServerCode);
                }
                usr.ServerCode = MeliController.ServerCode;
                _context.SaveChanges();
            }

            DateTime Fecha = new DateTime(2019, 1, 1, 1, 1, 1);
            ReportesController Reporte = new ReportesController(MeliController);
            var Orders = Reporte.GetOrders(Fecha, "397953529");
            foreach (var Order in Orders)
            {
                Console.WriteLine("OrderId => " + Order.JOBID);
            }
            Console.WriteLine("Total Orders gotten => " + Orders.Count);
            string line = "FEORD|HORDE|JOBID|TIEND|NUMTI|DESPR|PRVPU|UNIVE|TASIM|MONVE|WAERK|REFPA|NOMEN|NOMCT|APECT|DIREC|DIRE1|SUBEN|EDOEN|PSTLZ|PAISE|MAILC|TELNU|MATNR|SHIPID|SALE_FEE|LIST_COST|STATUS";
            using (StreamWriter sw = File.CreateText("list.csv"))
            {
                sw.WriteLine(line);
                foreach (var orden in Orders)
                {
                    line = orden.FEORD + "|"
                        + orden.HORDE + "|"
                        + orden.JOBID + "|"
                        + orden.TIEND + "|"
                        + orden.NUMTI + "|"
                        + orden.DESPR + "|"
                        + orden.PRVPU + "|"
                        + orden.UNIVE + "|"
                        + orden.TASIM + "|"
                        + orden.MONVE + "|"
                        + orden.WAERK + "|"
                        + orden.REFPA + "|"
                        + orden.NOMEN + "|"
                        + orden.NOMCT + "|"
                        + orden.APECT + "|"
                        + orden.DIREC + "|"
                        + orden.DIRE1 + "|"
                        + orden.SUBEN + "|"
                        + orden.EDOEN + "|"
                        + orden.PSTLZ + "|"
                        + orden.PAISE + "|"
                        + orden.MAILC + "|"
                        + orden.TELNU + "|"
                        + orden.MATNR + "|"
                        + orden.SHIPID + "|"
                        + orden.SALE_FEE + "|"
                        + orden.LIST_COST + "|"
                        + orden.STATUS;
                    sw.WriteLine(line);
                }
            }
            Console.ReadKey();
        }
        static void Server(Database.Usuarios Usr, string Url)
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }
            // URI prefixes are required,
            var prefixes = new List<string>() { "http://*:8888/" };

            // Create a listener.
            HttpListener listener = new HttpListener();
            
            // Add the prefixes.
            foreach (string s in prefixes)
            {
                listener.Prefixes.Add(s);
            }
            listener.Start();
            Console.WriteLine("Listening...");
            var p = Process.Start(new ProcessStartInfo("cmd", $"/c start {Url}") { CreateNoWindow = true });
            while (true)
            {
                // Note: The GetContext method blocks while waiting for a request.
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;

                string documentContents;
                using (Stream receiveStream = request.InputStream)
                {
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        documentContents = readStream.ReadToEnd();
                    }
                }
                Console.WriteLine($"Recived request for {request.Url}");
                Console.WriteLine(documentContents);

                Usr.ServerCode = request.Url.Query.Replace("?code=", "");

                // Obtain a response object.
                HttpListenerResponse response = context.Response;
                // Construct a response.
                string responseString = "<HTML><BODY> Got the Server Code! " + request.Url + "</BODY></HTML>";
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                // You must close the output stream.
                output.Close();
                if (Usr.ServerCode.Length > 0)
                {
                    p.Close();
                    break;
                }
                
            }
            listener.Stop();
        }
    }
}
