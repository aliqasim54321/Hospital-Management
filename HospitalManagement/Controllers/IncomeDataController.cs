using HospitalManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace HospitalManagement.Controllers
{
    public class IncomeDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns list of all income
        /// </summary>
        /// <returns></returns>

        // GET: Income/List
        [HttpGet]
        public IEnumerable<IncomeDto> ListIncome()
        {
            List<Income> Income = db.Income.ToList();
            List<IncomeDto> IncomeDtos = new List<IncomeDto>();

            Income.ForEach(a => IncomeDtos.Add(new IncomeDto()
            {
                income_id = a.income_id,
                pay_type = a.pay_type,
                category = a.Order.Category,
                Order_id = a.Order_id,
                income_date = a.income_date
            }));
            return IncomeDtos;
        }


        /// <summary>
        /// Returns and Income Detail that match the given Id to Income Id
        /// </summary>
        /// <param name="id">Primary Key of the Income</param>
        /// <returns></returns>

        [ResponseType(typeof(IncomeDto))]
        [HttpGet]
        public IHttpActionResult FindIncome(int id)
        {
            Income Income = db.Income.Find(id);
            IncomeDto IncomeDto = new IncomeDto()
            {
                income_id = Income.income_id,
                pay_type = Income.pay_type,
                category = Income.Order.Category,
                Order_id = Income.Order_id,
                income_date = Income.income_date

            };
            if (Income == null)
            {
                return NotFound();
            }

            return Ok(IncomeDto);
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="income"></param>
        /// <returns></returns>

        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult UpdateIncome(int id, Income income)
        {
            Debug.WriteLine("I have reached the update income method");
            if (!ModelState.IsValid)
            {
                Debug.WriteLine("Model State is invalid");
                return BadRequest(ModelState);
            }

            if (id != income.income_id)
            {
                Debug.WriteLine("ID Mismatch");
                Debug.WriteLine("GET parameter" + id);
                Debug.WriteLine("POST parameter" + income.income_id);
                return BadRequest();
            }

            db.Entry(income).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IncomeExists(id))
                {
                    Debug.WriteLine("Income not found");
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            Debug.WriteLine("None of the conditions trigerred");
            return StatusCode(HttpStatusCode.NoContent);
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="income"></param>
        /// <returns></returns>

        [ResponseType(typeof(Income))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult AddIncome(Income income)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Income.Add(income);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = income.income_id }, income);
        }







        [ResponseType(typeof(Income))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult DeleteIncome(int id)
        {
            Income income = db.Income.Find(id);
            if (income == null)
            {
                return NotFound();
            }

            db.Income.Remove(income);
            db.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool IncomeExists(int id)
        {
            return db.Income.Count(e => e.income_id == id) > 0;
        }
    }
}
