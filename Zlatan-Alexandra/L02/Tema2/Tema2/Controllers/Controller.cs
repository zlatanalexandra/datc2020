using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Tema2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Controller : ControllerBase
    {
        [HttpGet]
        public IEnumerable<Student> GetStudents()
        {
            return StudentRepo.getAll();
        }

        [HttpGet("{id}")]
        public Student GetStudentsbyId([FromRoute] int id)
        {
            return StudentRepo.getById(id);
        }

        [HttpPost]
        public string CreateStudent([FromBody] Student student)
        {
            try
            {
                StudentRepo.insert(student);
                return "Created!";
            }
            catch (System.Exception error)
            {
                return "Error: " + error.Message;
                throw;
            }
        }

        [HttpPut("{id}")]
        public string UpdateStudent([FromRoute] int id, [FromBody] Student student)
        {
            Student updatedStudent = StudentRepo.update(id, student);
            return updatedStudent.ToString();
        }

        [HttpDelete("{id}")]
        public string DeleteStudentById([FromRoute] int id)
        {
            StudentRepo.deleteById(id);
            return "Deleted!";
        }
    }
}
