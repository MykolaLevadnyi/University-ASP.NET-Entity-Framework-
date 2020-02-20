using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace University_MVC_
{
    public partial class Lessons
    {
        public Lessons()
        {
            LessonToschedule = new HashSet<LessonToschedule>();
        }

        public long Id { get; set; }
        [Display(Name = "Ім'я Предмету")]
        [Required(ErrorMessage = "Поле не може бути порожнім")]
        public string Name { get; set; }

        public virtual ICollection<LessonToschedule> LessonToschedule { get; set; }
    }
}
