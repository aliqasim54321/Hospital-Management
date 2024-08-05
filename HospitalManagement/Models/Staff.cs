using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public class Staff
    {
        [Key]
        public int StaffId { get; set; }

        public string StaffName {get; set; }

        public string StaffDepartment {  get; set; }
    }
}