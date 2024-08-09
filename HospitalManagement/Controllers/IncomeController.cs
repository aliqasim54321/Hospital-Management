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
        public ActionResult New()
        {
            
            string url = "listorders";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<IncomeDto> NewIncome = response.Content.ReadAsAsync<IEnumerable<IncomeDto>>().Result;
            return View(NewIncome);
        }

        // POST: Income/Create
        [HttpPost]
        public ActionResult Create(Income income)
        {
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
        public ActionResult Update(int id, Income income)
        {

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

        public ActionResult DeleteConfirm(int id)
        {
            string url = "findincome/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            IncomeDto selectedincome = response.Content.ReadAsAsync<IncomeDto>().Result;
            return View(selectedincome);
        }

        // POST: Income/Delete/5
        [HttpPost]

        public ActionResult Delete(int id)
        {

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