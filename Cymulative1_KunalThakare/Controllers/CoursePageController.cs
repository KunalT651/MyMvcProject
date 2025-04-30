using Microsoft.AspNetCore.Mvc;
using Cymulative1_KunalThakare.Models;
using System.Diagnostics;

namespace Cymulative1_KunalThakare.Controllers
{
    public class CoursePageController : Controller
    {
        private readonly SchoolDbContext _context;

        public CoursePageController(SchoolDbContext context)
        {
            _context = context;
        }

        public IActionResult List(DateTime? startDate, DateTime? endDate)
        {
            List<Course> CoursesList = new List<Course>();

            using (var connection = _context.AccessDatabase())
            {
                connection.Open();
                var command = connection.CreateCommand();

                command.CommandText = "SELECT * FROM Courses";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Course Course = new Course
                        {
                            CourseId = Convert.ToInt32(reader["Courseid"]),
                            CourseCode = reader["coursecode"].ToString(),
                            TeacherId = Convert.ToInt32(reader["teacherid"]),
                            StartDate = Convert.ToDateTime(reader["startdate"]),
                            FinishDate = Convert.ToDateTime(reader["finishdate"]),
                            CourseName = reader["coursename"].ToString()
                        };

                        CoursesList.Add(Course);
                    }
                }
            }

            return View(CoursesList); //Returns Courses list to the List.cshtml view
        }
    }
}