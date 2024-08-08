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
    public class PatientController : Controller
    {

        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static PatientController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44361/api/PatientData/");
        }


        // GET: Patient/List
        public ActionResult List()
        {
        
            string url = "ListPatients";
            HttpResponseMessage response = client.GetAsync(url).Result;
            Debug.WriteLine("Number of Patient arrived");
            Debug.WriteLine("The response code is");
           Debug.WriteLine(response.StatusCode);

            IEnumerable<PatientDto> patients = response.Content.ReadAsAsync<IEnumerable<PatientDto>>().Result;
             
            return View(patients);
        }

        // GET: Patient/Details/5
        public ActionResult Details(int id)
        {

            //objective : communicate with our patient data api to retrieve one of patient
            //curl https://localhost:44361/api/PatientsData/findPatient/{id}
            string url = "findPatient/"+id;
            HttpResponseMessage response = client.GetAsync(url).Result;
              

            PatientDto selectedpatient = response.Content.ReadAsAsync<PatientDto>().Result;
     
            return View(selectedpatient);
        }

        public ActionResult Error()
        {
            return View();
        }

        // GET: Patient/New
        public ActionResult New()
        {
            return View();
        }

        // POST: Patient/Create
        [HttpPost]
        public ActionResult Create(PatientDto patient)
        {
            Debug.WriteLine("The jsonpayloade is:");
            //Debug.WriteLine(patient.patient);

            //Add new patient into our database using API
            //curl -H "Content-Type:application/json" -d @patient.json https://localhost:44361/api/PatientsData/addpatient

            string url = "addpatient";

          
            string jsonpayload = jss.Serialize(patient);

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
                return RedirectToAction("Error");
            }
        }

        // GET: Patient/Edit/5
        public ActionResult Edit(int id)
        {
            string url = "findpatient/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            PatientDto SelectedPatient = response.Content.ReadAsAsync<PatientDto>().Result;

            return View(SelectedPatient);

        }

        // POST: Patient/Update/5
        [HttpPost]
        public ActionResult Update(int id, PatientDto patient)
        {
                           
                //serialize into JSON
                //Send the request to the API

                string url = "UpdatePatient/" + id;


                string jsonpayload = jss.Serialize(patient);
                Debug.WriteLine(jsonpayload);

                HttpContent content = new StringContent(jsonpayload);
                content.Headers.ContentType.MediaType = "application/json";

                //POST: api/PatientData/UpdatePatient/{id}
                //Header : Content-Type: application/json
                HttpResponseMessage response = client.PostAsync(url, content).Result;




                return RedirectToAction("Details/" + id);
            
        }

        // GET: Patient/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "findpatient/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            PatientDto selectedpatient = response.Content.ReadAsAsync<PatientDto>().Result;


            return View(selectedpatient);

        }

        // POST: Patient/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string url = "deletepatient/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;


            return RedirectToAction("List");

        }
    }
}
