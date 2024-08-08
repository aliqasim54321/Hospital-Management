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
using System.Diagnostics;

namespace HospitalManagement.Controllers
{
    public class PatientDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

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