using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Diagnostics;
using HospitalManagement.Models;
using System.Web.Script.Serialization;

namespace HospitalManagement.Controllers
{
    public class OrderController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        
        static OrderController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44361/api/orderdata/");

        }

        /// <summary>
        /// Grabs the authentication cookie sent to this controller.
        /// For proper WebAPI authentication, you can send a post request with login credentials to the WebAPI and log the access token from the response. The controller already knows this token, so we're just passing it up the chain.
        /// 
        /// Here is a descriptive article which walks through the process of setting up authorization/authentication directly.
        /// https://docs.microsoft.com/en-us/aspnet/web-api/overview/security/individual-accounts-in-web-api
        /// </summary>
        private void GetApplicationCookie()
        {
            string token = "";
            //HTTP client is set up to be reused, otherwise it will exhaust server resources.
            //This is a bit dangerous because a previously authenticated cookie could be cached for
            //a follow-up request from someone else. Reset cookies in HTTP client before grabbing a new one.
            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            //collect token as it is submitted to the controller
            //use it to pass along to the WebAPI.
            Debug.WriteLine("Token Submitted is : " + token);
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }
        // GET: Order/List
        public ActionResult List()
        {
            //objective:communictae with our order data api to retrive a list of orders
            //curl https://localhost:44361/api/orderdata/listorders  
           
            string url = "listorders";
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is");
           // Debug.WriteLine(response.StatusCode);

            IEnumerable<OrderDto> orders = response.Content.ReadAsAsync<IEnumerable<OrderDto>>().Result;
            //Debug.WriteLine("Number of orders received :");
            //Debug.WriteLine(orders.Count());
            return View(orders);
        }

        // GET: Order/Details/5
        public ActionResult Details(int id)
        {
            //objective:communictae with our order data api to retrive one of the  order
            //curl https://localhost:44361/api/orderdata/findorder/{id}
            
            string url = "findorder/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            Debug.WriteLine("The response code is");
            Debug.WriteLine(response.StatusCode);

            OrderDto selectedorder = response.Content.ReadAsAsync<OrderDto>().Result;
            Debug.WriteLine("Number of orders received :");
            Debug.WriteLine(selectedorder.Order_id);
            return View(selectedorder);
        }
        public ActionResult Error()
        {

            return View();
        }
        // GET: Order/New
        [Authorize]
        public ActionResult New()
        {
            //information about all orders in systam
            //GET api/orderdata/listorders
            string url = "listorders";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<OrderDto> OrderOptions = response.Content.ReadAsAsync<IEnumerable<OrderDto>>().Result;
            return View(OrderOptions);
        }

        // POST: Order/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Order order)
        {
            GetApplicationCookie();//get token credentials
            Debug.WriteLine("the json payload is:");
            Debug.WriteLine(order.Order_id);
            //objective:add a new order into our system using the API
            //curl -H "Content-Type:application/json" -d @order.json https://localhost:44362/api/orderdata/addorder
            string url = "addorder";

            
            string jsonpayload = jss.Serialize(order);

            Debug.WriteLine(jsonpayload);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            client.PostAsync(url, content);

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error"); ;
            }
        }

        // GET: Order/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
           
            //the existing order information
            string url = "findorder/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            OrderDto selectedorder = response.Content.ReadAsAsync<OrderDto>().Result;
            return View(selectedorder);
        }

        // POST: Order/Update/5
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Order order)
        {
            GetApplicationCookie();//get token credentials
            string url = "updateOrder/" + id;

            
            string jsonpayload = jss.Serialize(order);

            Debug.WriteLine(jsonpayload);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error"); ;
            }
        }

        // GET: Order/DeleteConfirm/5
        [Authorize]

        public ActionResult DeleteConfirm(int id)
        {
            string url = "findorder/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            OrderDto selectedorder = response.Content.ReadAsAsync<OrderDto>().Result;
            return View(selectedorder);
        }

        // POST: Order/Delete/5
        [HttpPost]
        [Authorize]

        public ActionResult Delete(int id)
        {
            GetApplicationCookie();//get token credentials
            string url = "deleteorder/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
    }
}
