using Microsoft.AspNetCore.Mvc;
using Cymulative1_KunalThakare.Models;
using System.Diagnostics;

namespace Cymulative1_KunalThakare.Controllers
{
    public class StudentPageController : Controller
    {
        private readonly SchoolDbContext _context;

        public StudentPageController(SchoolDbContext context)
        {
            _context = context;
        }

        public IActionResult List(DateTime? startDate, DateTime? endDate)
        {
            List<Student> StudentsList = new List<Student>();

            using (var connection = _context.AccessDatabase())
            {
                connection.Open();
                var command = connection.CreateCommand();

                command.CommandText = "SELECT * FROM students";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Student Student = new Student
                        {
                            StudentId = Convert.ToInt32(reader["Studentid"]),
                            StudentFName = reader["studentfname"].ToString(),
                            StudentLName = reader["studentlname"].ToString(),
                            StudentNumber = reader["studentnumber"].ToString(),
                            EnrolDate = Convert.ToDateTime(reader["enroldate"]),
                        };

                        StudentsList.Add(Student);
                    }
                }
            }

            return View(StudentsList); //Returns Students list to the List.cshtml view
        }
    }
}