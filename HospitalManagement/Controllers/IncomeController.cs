using HospitalManagement.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace HospitalManagement.Controllers
{
    public class IncomeController : Controller
    {
        private static readonly HttpClient client;

        static IncomeController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44361/api/incomedata");

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
        // GET: Income/List
        public ActionResult List()
        {
            
            string url = "listincome";
            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<IncomeDto> Income = response.Content.ReadAsAsync<IEnumerable<IncomeDto>>().Result;
            
            return View(Income);
        }


        // GET: Income/Details/5
        public ActionResult Details(int id)
        {
            
            string url = "findincome/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            Debug.WriteLine("The response code is");
            Debug.WriteLine(response.StatusCode);

            IncomeDto selectedincome = response.Content.ReadAsAsync<IncomeDto>().Result;
            Debug.WriteLine("List of Income: ");
            Debug.WriteLine(selectedincome.income_id);
            return View(selectedincome);
        }


        public ActionResult Error()
        {

            return View();
        }
        // GET: Income/New
        [Authorize]
        public ActionResult New()
        {
            
            string url = "listorders";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<IncomeDto> NewIncome = response.Content.ReadAsAsync<IEnumerable<IncomeDto>>().Result;
            return View(NewIncome);
        }

        // POST: Income/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Income income)
        {
            GetApplicationCookie();//get token credentials
            Debug.WriteLine("the json payload is:");
            Debug.WriteLine(income.income_id);
           
            string url = "addincome";

            JavaScriptSerializer jss = new JavaScriptSerializer();
            string jsonpayload = jss.Serialize(income);

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

        // GET: Income/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {

            //the existing order information
            string url = "findincome/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            IncomeDto selectedincome = response.Content.ReadAsAsync<IncomeDto>().Result;
            return View(selectedincome);
        }

        // POST: Income/Update/5
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Income income)
        {
            GetApplicationCookie();//get token credentials
            string url = "UpdateIncome/" + id;

            JavaScriptSerializer jss = new JavaScriptSerializer();
            string jsonpayload = jss.Serialize(income);

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

        // GET: Income/DeleteConfirm/5
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "findincome/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            IncomeDto selectedincome = response.Content.ReadAsAsync<IncomeDto>().Result;
            return View(selectedincome);
        }

        // POST: Income/Delete/5
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
