//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CRMnew.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Notes
    {
        public int Id { get; set; }

        [Display(Name = "Tre��")]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }

        [Display(Name = "Usuni�to")]
        public bool IsDeleted { get; set; }

        [Display(Name = "Firma")]
        public int CompanyId { get; set; }

        [Display(Name = "U�ytkownik")]
        public string UserId { get; set; }

        [Display(Name = "Czas")]
        public System.DateTime EntryTime { get; set; }
    
        public virtual AspNetUsers AspNetUsers { get; set; }
        public virtual Companies Companies { get; set; }
    }
}