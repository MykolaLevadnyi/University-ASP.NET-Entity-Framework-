using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace University_MVC_
{
    public partial class Deparments
    {
        public Deparments()
        {
            Groups = new HashSet<Groups>();
        }

        public long Id { get; set; }
        [Display(Name = "Кафедра")]
        [Required(ErrorMessage = "Поле не може бути порожнім")]
        public string Name { get; set; }

        public virtual ICollection<Groups> Groups { get; set; }
    }
}
