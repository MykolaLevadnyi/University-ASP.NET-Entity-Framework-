using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace University_MVC_
{
    public partial class Shedule
    {
        public Shedule()
        {
            LessonToschedule = new HashSet<LessonToschedule>();
        }

        public long Id { get; set; }
        [Display(Name = "День")]
        [Required(ErrorMessage = "Поле не може бути порожнім")]
        public long DayId { get; set; }
        [Display(Name = "Група")]
        [Required(ErrorMessage = "Поле не може бути порожнім")]
        public long GroupId { get; set; }

        public virtual Days Day { get; set; }
        public virtual Groups Group { get; set; }
        public virtual ICollection<LessonToschedule> LessonToschedule { get; set; }
    }
}
