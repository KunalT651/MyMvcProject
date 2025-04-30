namespace Cymulative1_KunalThakare.Models
{
    //This model properties holds the column values from Course table in SchoolDb. 
    public class Course
    {
        public int CourseId { get; set; }
        public string CourseCode { get; set; }
        public long TeacherId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public string CourseName { get; set; }
    }
}
