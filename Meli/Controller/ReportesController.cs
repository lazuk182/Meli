using Meli.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meli.Controller
{
    public class ReportesController
    {
        public ReportesController(MercadoLibreController Meli)
        {
            this.Meli = Meli;
        }
        public MercadoLibreController Meli { get; private set; }
        public List<Models.Reportes.Orden> GetOrders(DateTime From, string Seller)
        {
            List<Models.Reportes.Orden> ListOfOrders = new List<Models.Reportes.Orden>();
            try
            {
                var orders = Meli.GetOrders(Seller, From);
                foreach(var order in orders)
                {
                    order.shipping = Meli.GetShipment(order.shipping.id);
                    ListOfOrders.AddRange(ConvertMeliOrder(order));
                }
            }
            catch(Exception e)
            {
                ListOfOrders = new List<Models.Reportes.Orden>();
                Console.WriteLine("Exception caught in ReportesController.GetOrders => " + e.Message);
            }
            return ListOfOrders;
        }
        private List<Models.Reportes.Orden> ConvertMeliOrder(Models.Order Order)
        {
            List<Models.Reportes.Orden> ordenes = new List<Models.Reportes.Orden>();
            Models.Reportes.Orden orden;
            foreach (var OrderItem in Order.order_items)
            {
                orden = new Models.Reportes.Orden();
                orden.FEORD = Convert.ToDateTime(Order.date_created).ToString("yyyyMMdd");
                orden.HORDE = Convert.ToDateTime(Order.date_created).ToString("hhmmss");
                orden.POSNR = "";
                orden.JOBID = Order.id;
                orden.TIEND = "Mercado Libre";
                orden.NUMTI = "901091";
                orden.DESPR = OrderItem.item.title;
                orden.PRVPU = OrderItem.unit_price;
                orden.UNIVE = OrderItem.quantity;
                orden.TASIM = "16";
                orden.MONVE = Convert.ToString(Convert.ToDecimal(orden.PRVPU) * Convert.ToDecimal(orden.UNIVE));
                orden.WAERK = Order.currency_id.ToUpper();
                orden.REFPA = Order.payments.Any() ? Order.payments.FirstOrDefault().id : "";
                orden.NOMEN = Order.shipping.receiver_address.receiver_name;
                orden.NOMCT = Order.buyer.first_name;
                orden.APECT = Order.buyer.last_name;
                orden.DIREC = Order.shipping.receiver_address.address_line;
                orden.DIRE1 = Order.shipping.receiver_address.comment;
                orden.SUBEN = Order.shipping.receiver_address.city.name;
                orden.EDOEN = Order.shipping.receiver_address.state.name;
                orden.PSTLZ = Order.shipping.receiver_address.zip_code;
                orden.PAISE = Order.shipping.receiver_address.country.name == "México" ? "Mexico" : Order.shipping.receiver_address.country.name;
                orden.MAILC = Order.buyer.email;
                orden.TELNU = Order.shipping.receiver_address.receiver_phone;
                orden.MATNR = OrderItem.item.seller_sku;
                orden.SHIPID = Order.shipping.id;
                orden.SALE_FEE = OrderItem.sale_fee;
                orden.LIST_COST = Order.shipping.shipping_option.list_cost;
                orden.STATUS = Order.status;
                ordenes.Add(orden);
            }
            return ordenes;
        }
    }
}
