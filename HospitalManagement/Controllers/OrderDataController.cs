using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using HospitalManagement.Models;
using System.Diagnostics;
namespace HospitalManagement.Controllers
{
    public class OrderDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/OrderData/ListOrders
        [HttpGet]
        public IEnumerable<OrderDto> ListOrders()
        {
            List<Order> Orders = db.Orders.ToList();
            List<OrderDto> OrderDtos = new List<OrderDto>();

            Orders.ForEach(a => OrderDtos.Add(new OrderDto()
            {
                Order_id = a.Order_id,
                PatientName = a.Patient.PatientName,
                Category = a.Category,
                Total_Price = a.Total_Price
            }));
            return OrderDtos;
        }

        // GET: api/OrderData/FindOrder/5
        [ResponseType(typeof(OrderDto))]
        [HttpGet]
        public IHttpActionResult FindOrder(int id)
        {
            Order Order = db.Orders.Find(id);
            OrderDto OrderDto = new OrderDto()
            {
                Order_id = Order.Order_id,
                PatientName = Order.Patient.PatientName,
                Category = Order.Category,
                Total_Price = Order.Total_Price

            };
            if (Order == null)
            {
                return NotFound();
            }

            return Ok(OrderDto);
        }

        // POST: api/OrderData/UpdateOrder/5
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateOrder(int id, Order order)
        {
            Debug.WriteLine("I have reached the update order method");
            if (!ModelState.IsValid)
            {
                Debug.WriteLine("Model State is invalid");
                return BadRequest(ModelState);
            }

            if (id != order.Order_id)
            {
                Debug.WriteLine("ID Mismatch");
                Debug.WriteLine("GET parameter" + id);
                Debug.WriteLine("POST parameter" +order.Order_id);
                return BadRequest();
            }

            db.Entry(order).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    Debug.WriteLine("Order not found");
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

        // POST: api/OrderData/AddOrder
        [ResponseType(typeof(Order))]
        [HttpPost]
        public IHttpActionResult AddOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Orders.Add(order);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = order.Order_id }, order);
        }

        // POST: api/OrderData/DeleteOrder/5
        [ResponseType(typeof(Order))]
        [HttpPost]
        public IHttpActionResult DeleteOrder(int id)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            db.Orders.Remove(order);
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

        private bool OrderExists(int id)
        {
            return db.Orders.Count(e => e.Order_id == id) > 0;
        }
    }
}