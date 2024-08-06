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
        
        static OrderController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44361/api/");

        }
        // GET: Order/List
        public ActionResult List()
        {
            //objective:communictae with our order data api to retrive a list of orders
            //curl https://localhost:44361/api/orderdata/listorders  
           
            string url = "orderdata/listorders";
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
            
            string url = "orderdata/findorder/" + id;
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
        public ActionResult New()
        {
            //information about all orders in systam
            //GET api/orderdata/listorders
            string url = "orderdata/listorders";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<OrderDto> OrderOptions = response.Content.ReadAsAsync<IEnumerable<OrderDto>>().Result;
            return View(OrderOptions);
        }

        // POST: Order/Create
        [HttpPost]
        public ActionResult Create(Order order)
        {
            Debug.WriteLine("the json payload is:");
            Debug.WriteLine(order.Order_id);
            //objective:add a new order into our system using the API
            //curl -H "Content-Type:application/json" -d @order.json https://localhost:44362/api/orderdata/addorder
            string url = "orderdata/addorder";

            JavaScriptSerializer jss = new JavaScriptSerializer();
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
        public ActionResult Update(int id, Order order)
        {
            
            string url = "orderdata/UpdateOrder/" + id;

            JavaScriptSerializer jss = new JavaScriptSerializer();
            string jsonpayload = jss.Serialize(order);

            Debug.WriteLine(jsonpayload);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

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

        // GET: Order/DeleteConfirm/5
       
        public ActionResult DeleteConfirm(int id)
        {
            string url = "orderdata/findorder/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            OrderDto selectedorder = response.Content.ReadAsAsync<OrderDto>().Result;
            return View(selectedorder);
        }

        // POST: Order/Delete/5
        [HttpPost]
        
        public ActionResult Delete(int id)
        {
            
            string url = "orderdata/deleteorder/" + id;
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
