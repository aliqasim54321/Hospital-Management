﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HospitalManagement.Models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        public string PatientName { get; set; }

        public string PatientEmail { get; set; }

        public int PatientPhone { get; set; }

        // A patient belongs to one Staff Department 
        // A Staff Department can be there with many patient

        [ForeignKey("Staff")]

        public int StaffId { get; set; }
        public virtual Staff Staff { get; set; }


    }

    public class PatientDto
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string PatientEmail { get; set; }
        public int PatientPhone { get; set; }
        public int StaffId { get; set; }
        public string StaffName { get; set; }
    }
}