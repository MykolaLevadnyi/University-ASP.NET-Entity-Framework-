using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace University_MVC_
{
    public partial class Groups
    {
        public Groups()
        {
            Shedule = new HashSet<Shedule>();
            Students = new HashSet<Students>();
        }

        public long Id { get; set; }
        [Display(Name = "Назва групи")]
        [Required(ErrorMessage = "Поле не може бути порожнім")]
        public string Name { get; set; }
        
        public long? ClassPrId { get; set; }
        [Display(Name = "Кафедра")]
        [Required(ErrorMessage = "Поле не може бути порожнім")]
        public long DepartmentId { get; set; }

        [Display(Name = "Староста")]
        public virtual Students ClassPr { get; set; }
        [Display(Name = "Кафедра")]
        [Required(ErrorMessage = "Поле не може бути порожнім")]
        public virtual Deparments Department { get; set; }
        public virtual ICollection<Shedule> Shedule { get; set; }
        public virtual ICollection<Students> Students { get; set; }
    }
}
