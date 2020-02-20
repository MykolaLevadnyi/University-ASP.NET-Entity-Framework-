using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace University_MVC_
{
    public partial class Students
    {
        public Students()
        {
            Groups = new HashSet<Groups>();
        }

        public long Id { get; set; }
        [Display(Name = "Ім'я")]
        [Required(ErrorMessage = "Поле не може бути порожнім")]
        public string Name { get; set; }
        [Display(Name = "Прізвище")]
        [Required(ErrorMessage = "Поле не може бути порожнім")]
        public string Surname { get; set; }
        [Display(Name = "Стать")]
        [Required(ErrorMessage = "Поле не може бути порожнім")]
        public string Gender { get; set; }
        [Display(Name = "День народження")]
        [Required(ErrorMessage = "Поле не може бути порожнім")]
        public DateTime? Birthday { get; set; }
        [Display(Name = "Назва групи")]
        [Required(ErrorMessage = "Поле не може бути порожнім")]
        public long? GroupId { get; set; }
        [Display(Name = "Назва групи")]
        [Required(ErrorMessage = "Поле не може бути порожнім")]
        public virtual Groups Group { get; set; }
        public virtual ICollection<Groups> Groups { get; set; }
    }
}
