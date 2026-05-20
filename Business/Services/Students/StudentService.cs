using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Students
{
    public class StudentService : IStudentService
    {
        private readonly IRepo<Student> studentRepo;

        public StudentService(IRepo<Student> studentRepo)
        {
            this.studentRepo = studentRepo;
        }

        public async Task<List<Student>> GetStudents()
        { 
            return await studentRepo.GetAllAsync();
        }
    }
}
