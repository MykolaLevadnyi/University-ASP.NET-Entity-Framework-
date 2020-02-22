using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace University_MVC_
{
    public partial class LessonToschedule
    {
        public long Id { get; set; }
        //[Required(ErrorMessage = "Поле не може бути порожнім")]
        public long ScheduleId { get; set; }
        [Display(Name = "Назва Предмету")]
        [Required(ErrorMessage = "Поле не може бути порожнім")]
        public long LessonId { get; set; }
        [Display(Name = "Номер пари")]
        [Required(ErrorMessage = "Поле не може бути порожнім")]
        public int? Num { get; set; }


        public virtual Lessons Lesson { get; set; }
        public virtual Shedule Schedule { get; set; }
    }
}
