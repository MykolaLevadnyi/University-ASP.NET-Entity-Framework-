using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace University_MVC_
{
    public partial class Days
    {
        public Days()
        {
            Shedule = new HashSet<Shedule>();
        }

        public long Id { get; set; }
        [Display(Name = "День Неділі")]
        public string Name { get; set; }

        public virtual ICollection<Shedule> Shedule { get; set; }
    }
}
