using Microsoft.AspNetCore.Mvc;
using Cymulative1_KunalThakare.Models;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Cymulative1_KunalThakare.Controllers
{
    public class TeacherPageController : Controller
    {
        private readonly SchoolDbContext _context;

        public TeacherPageController(SchoolDbContext context)
        {
            _context = context;
        }

        List<Teacher> teachersList = new List<Teacher>();

        public IActionResult List(DateTime? startDate, DateTime? endDate)
        {
            using (var connection = _context.AccessDatabase())
            {
                connection.Open();
                var command = connection.CreateCommand();

                if (startDate.HasValue && endDate.HasValue)
                {
                    command.CommandText = "SELECT * FROM teachers WHERE hiredate BETWEEN @startDate AND @endDate";
                    command.Parameters.AddWithValue("@startDate", startDate.Value);
                    command.Parameters.AddWithValue("@endDate", endDate.Value);
                }
                else
                {
                    command.CommandText = "SELECT * FROM teachers";
                }

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Teacher teacher = new Teacher
                        {
                            TeacherId = Convert.ToInt32(reader["teacherid"]),
                            TeacherFName = reader["teacherfname"].ToString(),
                            TeacherLName = reader["teacherlname"].ToString(),
                            EmployeeNumber = reader["employeenumber"].ToString(),
                            HireDate = Convert.ToDateTime(reader["hiredate"]),
                            Salary = Convert.ToDecimal(reader["salary"])
                        };

                        teachersList.Add(teacher);
                    }
                }
            }

            return View(teachersList); //Returns teachers list to the List.cshtml view
        }

        public IActionResult Show(int? id)
        {
            if (!id.HasValue)
            {
                return View();
            }

            TeacherViewModel teacherViewModel = new TeacherViewModel();
            Teacher teacher = null;

            using (var connection = _context.AccessDatabase())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM teachers WHERE teacherid = @id";
                command.Parameters.AddWithValue("@id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        teacher = new Teacher
                        {
                            TeacherId = Convert.ToInt32(reader["teacherid"]),
                            TeacherFName = reader["teacherfname"].ToString(),
                            TeacherLName = reader["teacherlname"].ToString(),
                            EmployeeNumber = reader["employeenumber"].ToString(),
                            HireDate = Convert.ToDateTime(reader["hiredate"]),
                            Salary = Convert.ToDecimal(reader["salary"])
                        };
                    }
                    else
                    {
                        //This will redirect to an error page(Error.cshtml)
                        return View("Error", new ErrorViewModel
                        {
                            RequestId = id.ToString(),
                        }
                        );
                    }

                    teacherViewModel.Teacher = teacher;
                }

                var courseCommand = connection.CreateCommand();
                courseCommand.CommandText = "SELECT * FROM courses WHERE teacherid = @id";
                courseCommand.Parameters.AddWithValue("@id", id);


                teacherViewModel.Courses = new List<Course>();

                using (var reader = courseCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        teacherViewModel.Courses.Add(new Course()
                        {
                            CourseId = Convert.ToInt32(reader["Courseid"]),
                            CourseCode = reader["coursecode"].ToString(),
                            StartDate = Convert.ToDateTime(reader["startdate"]),
                            FinishDate = Convert.ToDateTime(reader["finishdate"]),
                            CourseName = reader["coursename"].ToString()
                        });
                    }
                }


            }

            return View(teacherViewModel); // Pass the teacher(according to id) to Show.cshtml
        }

        public IActionResult DeleteConfirm()
        {
            return View();
        }


        [HttpPost]
        public IActionResult DeleteButtonClick(int TeacherId)
        {
            using (var connection = _context.AccessDatabase())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM teachers WHERE teacherid = @id";
                command.Parameters.AddWithValue("@id", TeacherId);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    //This will redirect to an error page(Error.cshtml)
                    return View("Error", new ErrorViewModel
                    {
                        RequestId = TeacherId.ToString(),
                    }
                    );
                }
            }

            return RedirectToAction("DeleteConfirm");
        }

        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddTeacher(Teacher newTeacher)
        {
            if (newTeacher.TeacherLName == "" || newTeacher.TeacherFName == "" 
                || newTeacher.HireDate > DateTime.Today
                || !Regex.IsMatch(newTeacher.EmployeeNumber, @"^T\d{3}$")
                || teachersList.Any(t => t.EmployeeNumber == newTeacher.EmployeeNumber))
            {
                //This will redirect to an error page(Error.cshtml)
                return View("Error", new ErrorViewModel
                {
                    RequestId = newTeacher.TeacherId.ToString(),
                }
                );
            }


            if (ModelState.IsValid)
            {
                using (var connection = _context.AccessDatabase())
                {
                    //if(_context.Teachers.ToList().Any(t => t.EmployeeNumber == newTeacher.EmployeeNumber))
                    //{
                    //    //This will redirect to an error page(Error.cshtml)
                    //    return View("Error", new ErrorViewModel
                    //    {
                    //        RequestId = newTeacher.TeacherId.ToString(),
                    //    }
                    //    );
                    //}

                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "INSERT INTO teachers (teacherfname, teacherlname, employeenumber, hiredate, salary) VALUES (@fname, @lname, @empNum, @hireDate, @salary)";
                    command.Parameters.AddWithValue("@fname", newTeacher.TeacherFName);
                    command.Parameters.AddWithValue("@lname", newTeacher.TeacherLName);
                    command.Parameters.AddWithValue("@empNum", newTeacher.EmployeeNumber);
                    command.Parameters.AddWithValue("@hireDate", newTeacher.HireDate);
                    command.Parameters.AddWithValue("@salary", newTeacher.Salary);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        return RedirectToAction("New");
                    }
                    else
                    {
                        //This will redirect to an error page(Error.cshtml)
                        return View("Error", new ErrorViewModel
                        {
                            RequestId = newTeacher.TeacherId.ToString(),
                        }
                        );
                    }
                }
            }

            return RedirectToAction("New");
        }

        //Cumulative3


            /// <summary>
            /// Displays Edit.cshtml with optional teacher data.
            /// </summary>
            [HttpGet]
            public IActionResult Edit()
            {
                return View();
            }

        /// <summary>
        /// Searches for a teacher by ID and loads their data in Edit.cshtml.
        /// </summary>
        [HttpGet]
        public IActionResult SearchTeacher(int TeacherId)
        {
            Teacher teacher = null;

            using (var connection = _context.AccessDatabase())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM teachers WHERE teacherid = @id";
                command.Parameters.AddWithValue("@id", TeacherId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        teacher = new Teacher
                        {
                            TeacherId = Convert.ToInt32(reader["teacherid"]),
                            TeacherFName = reader["teacherfname"].ToString(),
                            TeacherLName = reader["teacherlname"].ToString(),
                            EmployeeNumber = reader["employeenumber"].ToString(),
                            HireDate = Convert.ToDateTime(reader["hiredate"]),
                            Salary = Convert.ToDecimal(reader["salary"])
                        };
                    }
                }
            }

            if (teacher == null)
            {
                TempData["ErrorMessage"] = "Teacher not found.";
                return RedirectToAction("Edit");
            }

            return View("Edit", teacher);
        }

        /// <summary>
        /// Updates teacher details after editing.
        /// </summary>
        [HttpPost]
        public IActionResult UpdateTeacher(Teacher updatedTeacher)
        {
            using (var connection = _context.AccessDatabase())
            {
                connection.Open();

                // Validate if teacher exists
                var checkCommand = connection.CreateCommand();
                checkCommand.CommandText = "SELECT COUNT(*) FROM teachers WHERE teacherid = @id";
                checkCommand.Parameters.AddWithValue("@id", updatedTeacher.TeacherId);
                int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                if (count == 0)
                {
                    TempData["ErrorMessage"] = "Teacher not found.";
                    return RedirectToAction("Edit");
                }

                // Validation
                if (string.IsNullOrWhiteSpace(updatedTeacher.TeacherFName) || string.IsNullOrWhiteSpace(updatedTeacher.TeacherLName))
                {
                    TempData["ErrorMessage"] = "Teacher name cannot be empty.";
                    return View("Edit", updatedTeacher);
                }
                if (updatedTeacher.HireDate > DateTime.Today)
                {
                    TempData["ErrorMessage"] = "Hire date cannot be in the future.";
                    return View("Edit", updatedTeacher);
                }
                if (updatedTeacher.Salary < 0)
                {
                    TempData["ErrorMessage"] = "Salary must be greater than or equal to 0.";
                    return View("Edit", updatedTeacher);
                }

                // Update record manually
                var updateCommand = connection.CreateCommand();
                updateCommand.CommandText = @"
            UPDATE teachers SET 
                teacherfname = @FirstName,
                teacherlname = @LastName,
                employeenumber = @EmployeeNumber,
                hiredate = @HireDate,
                salary = @Salary
            WHERE teacherid = @id";

                updateCommand.Parameters.AddWithValue("@FirstName", updatedTeacher.TeacherFName);
                updateCommand.Parameters.AddWithValue("@LastName", updatedTeacher.TeacherLName);
                updateCommand.Parameters.AddWithValue("@EmployeeNumber", updatedTeacher.EmployeeNumber);
                updateCommand.Parameters.AddWithValue("@HireDate", updatedTeacher.HireDate);
                updateCommand.Parameters.AddWithValue("@Salary", updatedTeacher.Salary);
                updateCommand.Parameters.AddWithValue("@id", updatedTeacher.TeacherId);

                updateCommand.ExecuteNonQuery();
            }

            TempData["SuccessMessage"] = "Teacher updated successfully.";
            return RedirectToAction("Edit");
        }
        //Cumulative3
    }
}