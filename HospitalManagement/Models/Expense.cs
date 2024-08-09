using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalManagement.Models
{
    public class Expense
    {
        [Key]
        public int expense_id { get; set; }

        public string exppay_type { get; set; }

        //1 record of expense is for 1 staff for 1 month
        [ForeignKey("Staff")]
        public int StaffId { get; set; }
        public virtual Staff Staff { get; set; }

        public DateTime expense_date { get; set; }

    }
}