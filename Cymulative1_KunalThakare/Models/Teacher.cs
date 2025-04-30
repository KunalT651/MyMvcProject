namespace Cymulative1_KunalThakare.Models
{
    //This model properties holds the column values from Teacher table in SchoolDb. 
    public class Teacher
    {
        public int TeacherId { get; set; }
        public string TeacherFName { get; set; }
        public string TeacherLName { get; set; }
        public string EmployeeNumber { get; set; }
        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }
    }
}
