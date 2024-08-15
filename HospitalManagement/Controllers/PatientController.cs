using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Diagnostics;
using HospitalManagement.Models;
using System.Web.Script.Serialization;

///<summary>
///Making a Database for the Patient to make an appoinmetn
///Have used the JavaScriptSerializer for passing the data from json formt to the sttring 
///TO reduce the code the uri contain the api to be called as an function 
///</summary>
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
        /// <summary>
        /// This method communicate with the Patient data api and get the list of Patients and show them on the webpage 
        /// </summary>
        /// <returns>
        /// Returns  a view with the list of Patients
        /// </returns>
        /// <example>  GET: Patient/List => List View (with the list of Patients)
        /// </example>

        // GET: Patient/List
        public ActionResult List()
        {
            //objective : communicate with our patient data api to retrieve a  list of patient
            //curl https://localhost:44361/api/PatientsData/ListPatients

            string url = "ListPatients";
            HttpResponseMessage response = client.GetAsync(url).Result;
            Debug.WriteLine("Number of Patient arrived");
            Debug.WriteLine("The response code is");
           Debug.WriteLine(response.StatusCode);

            IEnumerable<PatientDto> patients = response.Content.ReadAsAsync<IEnumerable<PatientDto>>().Result;
             
            return View(patients);
        }
        /// <summary>
        /// This method communicate with the FindPatient method in the Patient data api , get the infomartion about the particular  Patient and show it on the webpage 
        /// </summary>
        /// <param name="id">The id of a Patient whose information  is requested </param>
        /// <returns>
        /// Returns  a view with the information about a particular Patient
        /// </returns>
        /// <example>  GET: Patient/Details/5 => Details View( The details of a requested  Patient with the Patient  id of 5)
        /// </example>
        
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

        ///GET: Patient/New => New View => (this webpage gives a form with an empty input fields where new Patient's information can be filled)

        // GET: Patient/New
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// This method communicate with the AddPatient method in the Patient data api , pass on the new Patient's infomation to this method to create a new  Patient in the database
        /// </summary>
        /// <param name="Patient">The Patient object with new  Patient's information </param>
        /// <returns>
        /// if the information is processed successfully redirects to the List View 
        /// else directs to the Error View page 
        /// </returns>
        /// <example>  POST: Patient/Create
        /// FORM DATA: Patient JASON Object
        /// </example>
        

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

        /// <summary>
        /// This method communicates with the PatientData api ,  fetch the  stored informtaion of a  Patient and  directs  it to the edit page  where it can be updated
        /// </summary>
        /// <param name="id"> The id of a Patient </param>
        /// <returns>
        ///  GET: Patient/Edit/5 => dircets it to the Edit View with the previous information of an Patient showing on the page 
        /// </returns>
       
        // GET: Patient/Edit/5
        public ActionResult Edit(int id)
        {
            string url = "findpatient/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            PatientDto SelectedPatient = response.Content.ReadAsAsync<PatientDto>().Result;

            return View(SelectedPatient);

        }


        /// <summary>
        /// This method communicates with Updatepatient  method in the patientData api, pass on the updatd information about an  patient and update it in the database
        /// </summary>
        /// <param name="id"> The id of a patient </param>
        ///<param name="patient"> The patient object with the updated information </param>
        ///<returns>
        ///if the informtion is processed succssfully redircts to the List View 
        ///else it  directs to the Error View
        /// </returns>
        /// <example>
        ///  POST: patient/Update/5   - > update the patient with patient id of 5 with the updated information by communicating wih the Updatepatient method in the patientData Api and redirects it to the List view 
        /// FORM DATA - patient JASON  Object
        /// </example>
        /// 


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

        /// <summary>
        /// This method communicates with the PatientData Api and redirects to the DeleteConfirm View where it confirms with user  before deleting a department
        /// </summary>
        /// <param name="id">The id of  Patient which  is requested to be deleted from te database</param>
        /// <returns>
        ///  Directs to DeleteConfrim View  prompting user to confirm the deletion of a Patient
        /// </returns>
        /// <example>
        /// GET: Patient/DeleteConfirm/5 - >   Directs to DeleteConfrim View prompting to confirm the deletion of Patient with department id of 5
        /// </example>
       

        // GET: Patient/DeleteConfirm/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "findpatient/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            PatientDto selectedpatient = response.Content.ReadAsAsync<PatientDto>().Result;


            return View(selectedpatient);

        }

        /// <summary>
        /// This method communicates with the Deletepatient mehtod in the  patient Data Api and  Deletes the particular  patient from the database
        /// </summary>
        /// <param name="id">The id of a patient to be deleted </param>
        /// <returns>
        ///if the informtion is processed succssfully redircts to the List View 
        ///else it  directs to the Error View
        /// </returns>
        /// <example>
        /// POST: patient/Delete/5 => deletes a patient with patient id 5 by communicating  with the Deletepatient method in the patient data api and redirects to the List View  
        /// </example>
       

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
