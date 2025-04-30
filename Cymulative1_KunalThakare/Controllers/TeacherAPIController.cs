using Microsoft.AspNetCore.Mvc;
using Cymulative1_KunalThakare.Models;

namespace Cymulative1_KunalThakare.Controllers
{
    [ApiController]
    [Route("api/Teacher")]
    public class TeacherAPIController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        public TeacherAPIController(SchoolDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET: api/Teacher/ListAllTeachers
        /// Retrieves a list of all teachers in the database.
        /// Example Request: GET /api/Teacher/ListAllTeachers
        /// Response: Returns list of all teachers with their details.
        /// </summary>
        /// <returns>List of all teachers</returns>
        [HttpGet]
        [Route("ListAllTeachers")]
        public List<Teacher> ListAllTeachers()
        {
            List<Teacher> TeacherList = new List<Teacher>();

            using (var connection = _context.AccessDatabase())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM teachers";

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

                        TeacherList.Add(teacher);
                    }
                }
            }

            return TeacherList;
        }

        /// <summary>
        /// GET: api/Teacher/{id}
        /// Retrieves details of a specific teacher by ID.
        /// Example Request: GET /api/Teacher/5
        /// Response: teacher details if found, otherwise error..
        /// </summary>
        /// <param name="id">Teacher ID</param>
        /// <returns>Teacher details</returns>
        [HttpGet("{id}")]
        public IActionResult GetTeacher(int id)
        {
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
                        Teacher teacher = new Teacher
                        {
                            TeacherId = Convert.ToInt32(reader["teacherid"]),
                            TeacherFName = reader["teacherfname"].ToString(),
                            TeacherLName = reader["teacherlname"].ToString(),
                            EmployeeNumber = reader["employeenumber"].ToString(),
                            HireDate = Convert.ToDateTime(reader["hiredate"]),
                            Salary = Convert.ToDecimal(reader["salary"])
                        };

                        return Ok(teacher);
                    }
                }
            }

            return NotFound("Teacher details not found.");
        }

        /// <summary>
        /// POST: api/Teacher
        /// Adds a new teacher to the database.
        /// **Example Request:** 
        /// POST /api/Teacher
        /// Content-Type: application/json
        /// {
        ///   "TeacherFName": "K",
        ///   "TeacherLName": "T",
        ///   "EmployeeNumber": "T123",
        ///   "HireDate": "2025-04-04",
        ///   "Salary": 58000.00
        /// }
        /// Response: 200 OK or 400 Bad Request if validation fails.
        /// </summary>
        /// <param name="newTeacher">New Teacher object</param>
        [HttpPost]
        public IActionResult AddTeacher([FromBody] Teacher newTeacher)
        {
            if (newTeacher == null)
            {
                return BadRequest("Invalid teacher data.");
            }

            using (var connection = _context.AccessDatabase())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                INSERT INTO teachers (teacherfname, teacherlname, employeenumber, hiredate, salary)
                VALUES (@FirstName, @LastName, @EmployeeNumber, @HireDate, @Salary)";

                command.Parameters.AddWithValue("@FirstName", newTeacher.TeacherFName);
                command.Parameters.AddWithValue("@LastName", newTeacher.TeacherLName);
                command.Parameters.AddWithValue("@EmployeeNumber", newTeacher.EmployeeNumber);
                command.Parameters.AddWithValue("@HireDate", newTeacher.HireDate);
                command.Parameters.AddWithValue("@Salary", newTeacher.Salary);

                command.ExecuteNonQuery();
            }

            return Ok("Teacher added successfully.");
        }

        /// <summary>
        /// DELETE: api/Teacher/{id}
        /// Deletes a teacher from the database by ID.
        /// Example Request: DELETE /api/Teacher/5
        /// Response: 200 OK if deletion successful, error if the teacher does not exist.
        /// </summary>
        /// <param name="id">Teacher ID</param>
        [HttpDelete("{id}")]
        public IActionResult DeleteTeacher(int id)
        {
            using (var connection = _context.AccessDatabase())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM teachers WHERE teacherid = @id";
                command.Parameters.AddWithValue("@id", id);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    return NotFound("Teacher not found.");
                }
            }

            return Ok("Teacher with ID " + id + " deleted successfully.");
        }

        //Cumulative3

        /// <summary>
        /// PUT: api/Teacher/{id}
        /// Updates an existing teacher's details.
        /// **Example Request:** 
        /// PUT /api/Teacher/5
        /// Content-Type: application/json
        /// {
        ///   "TeacherFName": "UpdatedName",
        ///   "TeacherLName": "UpdatedLastName",
        ///   "EmployeeNumber": "T123",
        ///   "HireDate": "2024-01-01",
        ///   "Salary": 60000.00
        /// }
        /// **Response:** 200 OK if updated, 404 if teacher not found, 400 if validation fails.
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult UpdateTeacher(int id, [FromBody] Teacher updatedTeacher)
        {
            if (updatedTeacher == null)
            {
                return BadRequest("Invalid teacher data.");
            }

            using (var connection = _context.AccessDatabase())
            {
                connection.Open();

                // Check if teacher exists
                var checkCommand = connection.CreateCommand();
                checkCommand.CommandText = "SELECT COUNT(*) FROM teachers WHERE teacherid = @id";
                checkCommand.Parameters.AddWithValue("@id", id);
                int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                if (count == 0) // No teacher found
                {
                    return NotFound("Teacher not found.");
                }

                // Validate on the server side
                if (string.IsNullOrWhiteSpace(updatedTeacher.TeacherFName) || string.IsNullOrWhiteSpace(updatedTeacher.TeacherLName))
                {
                    return BadRequest("Teacher name cannot be empty.");
                }
                if (updatedTeacher.HireDate > DateTime.Today)
                {
                    return BadRequest("Hire date cannot be in the future.");
                }
                if (updatedTeacher.Salary < 0)
                {
                    return BadRequest("Salary must be greater than or equal to 0.");
                }

                // Proceed with the update
                var command = connection.CreateCommand();
                command.CommandText = @"UPDATE teachers 
                                SET teacherfname = @FirstName, 
                                    teacherlname = @LastName, 
                                    employeenumber = @EmployeeNumber, 
                                    hiredate = @HireDate, 
                                    salary = @Salary 
                                WHERE teacherid = @id";

                command.Parameters.AddWithValue("@FirstName", updatedTeacher.TeacherFName);
                command.Parameters.AddWithValue("@LastName", updatedTeacher.TeacherLName);
                command.Parameters.AddWithValue("@EmployeeNumber", updatedTeacher.EmployeeNumber);
                command.Parameters.AddWithValue("@HireDate", updatedTeacher.HireDate);
                command.Parameters.AddWithValue("@Salary", updatedTeacher.Salary);
                command.Parameters.AddWithValue("@id", id);

                command.ExecuteNonQuery();
            }

            return Ok($"Teacher with ID {id} updated successfully.");
        }

        //Cumulative3
    }
}