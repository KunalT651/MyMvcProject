namespace Cymulative1_KunalThakare.Models
{
    //This model properties holds the column values from Student table in SchoolDb. 
    public class Student
    {
        public int StudentId { get; set; }
        public string StudentFName { get; set; }
        public string StudentLName { get; set; }
        public string StudentNumber { get; set; }
        public DateTime EnrolDate { get; set; }
    }
}
