using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using HospitalManagement.Models;

namespace HospitalManagement.Controllers
{
    public class PatientDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// This method lists the number of Patients in the database
        /// </summary>
        /// <returns>An array of Patients objects</returns>
        /// <example>
        /// GET: api/PatientsData/ListDepartments =>
        ///  using command prompt 
        ///curl https://localhost:44361/api/DepartmentData/ListDepartments
        /// {  "PatientId": 9, "PatientName": "Ali Ashraf", "PatientPhone": "1234567890",  "Staff Department": Human Resourse }


        // GET: api/PatientData/ListPatients
        [HttpGet]
           public IEnumerable<PatientDto> ListPatients()
        {
            List<Patient> Patients = db.Patients.ToList();
            List<PatientDto> PatientDtos = new List<PatientDto>();

            Patients.ForEach(p => PatientDtos.Add(new PatientDto()
            {
                PatientId = p.PatientId,
                PatientName = p.PatientName,
                PatientEmail= p.PatientEmail,
                PatientPhone= p.PatientPhone,
                StaffName= p.Staff.StaffDepartment, 

            }));
            return PatientDtos;
        }

        /// <summary>
        /// This method provides/fetch the information about a particular Patient from the database
        /// </summary>
        /// <param name="id"> id refres to the PatientId of an Patient whose information is requested</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: A Patient in the system matching up to the  Patient ID primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example> 
        /// GET: api/PatientData/FindDepartment/9=> [{  "PatientId": 9, "PatientName": "Ali Ashraf", "PatientPhone": "1234567890",  "Staff Department": Human Resourse }]
        ///  OR using command prompt
        ///  curl https://localhost:44361/api/DepartmentData/FindDepartment/9 =>
        /// [{ "PatientId": 9, "PatientName": "Ali Ashraf", "PatientPhone": "1234567890",  "Staff Department": Human Resourse }]
        /// </example>

        // GET: api/PatientData/FindPatient/5
        [ResponseType(typeof(Patient))]
        [HttpGet]
        public IHttpActionResult FindPatient(int id)
        {
            Patient Patient = db.Patients.Find(id);
            if (Patient == null)
            {
                return NotFound();
            }
            PatientDto PatientDto = new PatientDto()
            {
                PatientId = Patient.PatientId,
                PatientName = Patient.PatientName,
                PatientEmail = Patient.PatientEmail,
                PatientPhone = Patient.PatientPhone,
                StaffName = Patient.Staff.StaffDepartment,
            };
            Debug.WriteLine(PatientDto.ToString());


            return Ok(PatientDto);
        }

        /// <summary>
        /// This method updates the infomation about the current patient in the database
        /// </summary>
        /// <param name="id"> The id of an patient whose information needs to be updated</param>
        /// <param name="department">JSON FORM DATA of an patient </param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>>curl -d @Patient.json -H "Content-type:application/json"  https://localhost:44355/api/PatientData/UpdatePatient/1 => updates the informatio of an Patient with PatientId =9 with the updated informtion listed in the Patientnt.json file
        /// POST: api/PatientData/UpdatePatient/5
        /// FORM DATA:  Patient JASON Object
        /// </example>
       
        // POST: api/PatientData/UpdatePatient/5
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdatePatient(int id, Patient patient)
        {
            Debug.WriteLine("I have reached the update animal method");
            if (!ModelState.IsValid)
            {
                Debug.WriteLine("Model State is not valid");
                return BadRequest(ModelState);
            }

            if (id != patient.PatientId)
            {
                Debug.WriteLine("ID mismatch");
                Debug.WriteLine("Get parameter"+id);
                Debug.WriteLine("POST parameter" + patient.PatientId);
                Debug.WriteLine("POST parameter" + patient.PatientName);
                Debug.WriteLine("POST parameter" + patient.PatientEmail);
                Debug.WriteLine("POST parameter" + patient.PatientPhone);
               
                return BadRequest();
            }

            db.Entry(patient).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatientExists(id))
                {
                    Debug.WriteLine("Patient not found");
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            Debug.WriteLine("None of the condition triggered");
            return StatusCode(HttpStatusCode.NoContent);
        }
        /// <summary>
        /// This method adds the new patient into the database
        /// </summary>
        /// <param name="patient"> JSON FORM DATA of an patient</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: patient ID, patient Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>>curl -d @patient.json -H "Content-type:application/json"  https://localhost:44361/api/patientData/Addpatient => adds the new  patient object listed in the patient.json file  into the database
        ///  POST: api/patientData/Addpatient
        /// FORM DATA: patient JSON Object
        /// </example>

        // POST: api/PatientData/AddPatient
        [ResponseType(typeof(Patient))]
        [HttpPost]
        public IHttpActionResult AddPatient(Patient patient)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Patients.Add(patient);
            db.SaveChanges();

            return Ok();
        }

        // POST: api/PatientData/DeleteAnimal/5
        /// <summary>
        /// This method deletes the specific Patient from the database by providing the id of an Patient as a parameter 
        /// </summary>
        /// <param name="id">The id of an Patient to be deleted</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example> Post: api/PatientData/DeletePatient/5  => deletes the Patient from the database having id = 5
        /// FORM DATA: (empty)
        /// curl -d ""  https://localhost:44361/api/PatientData/DeletePatient/5 =>deletes the Patient from the database having id = 8
        /// </example>
        
        [ResponseType(typeof(Patient))]
        [HttpPost]
        public IHttpActionResult DeletePatient(int id)
        {
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return NotFound();
            }

            db.Patients.Remove(patient);
            db.SaveChanges();

            return Ok(patient);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PatientExists(int id)
        {
            return db.Patients.Count(e => e.PatientId == id) > 0;
        }
    }
}