using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalManagement.Models
{
    public class Income
    {
        [Key]
        public int income_id { get; set; }

        public string pay_type { get; set; }

        //1 record of income is from one order
        [ForeignKey("Order")]
        public int Order_id { get; set; }
        public virtual Order Order { get; set; }

        public DateTime income_date { get; set; }

    }

    public class IncomeDto
    {
        public int income_id { get; set; }
        public string pay_type { get; set; }
        public string category { get; set; }
        public int Order_id { get; set; }
        public DateTime income_date { get; set; }


    }
}