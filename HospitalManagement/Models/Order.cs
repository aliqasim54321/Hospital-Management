using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HospitalManagement.Models
{
    public class Order
    {
        [Key]
        public int Order_id { get; set; }
        //An order belongs to one patient
        //An patient can have many orders
        [ForeignKey("Patient")]
        public int PatientId { get; set; }
        public string Category { get; set; }

        public int Total_Price { get; set; }
        public virtual Patient Patient { get; set; }
    }
    public class OrderDto
    {
        public int Order_id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string Category { get; set; }
        public int Total_Price { get; set; }


    }
}