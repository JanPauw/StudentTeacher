using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StudentTeacher.Models;

namespace STxUnitTesting
{
    public class UnitTest1 : IDisposable
    {
        private readonly XISD_POEContext _context;

        public UnitTest1()
        {
            var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<XISD_POEContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .UseInternalServiceProvider(serviceProvider)
            .Options;

            _context = new XISD_POEContext(options);
            _context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetCampus_ReturnsCorrectType()
        {
            var campuses = await _context.Campuses.ToListAsync();
            var model = Assert.IsAssignableFrom<IEnumerable<Campus>>(campuses);
        }

        [Fact]
        public async Task GetCommentaries_ReturnsCorrectType()
        {
            var commentaries = await _context.Commentaries.ToListAsync();
            var model = Assert.IsAssignableFrom<IEnumerable<Commentary>>(commentaries);
        }

        [Fact]
        public async Task GetExecutions_ReturnsCorrectType()
        {
            var execuotions = await _context.Executions.ToListAsync();
            var model = Assert.IsAssignableFrom<IEnumerable<Execution>>(execuotions);
        }

        [Fact]
        public async Task GetGradings_ReturnsCorrectType()
        {
            var gradings = await _context.Gradings.ToListAsync();
            var model = Assert.IsAssignableFrom<IEnumerable<Grading>>(gradings);
        }

        [Fact]
        public async Task GetLecturers_ReturnsCorrectType()
        {
            var lecturers = await _context.Lecturers.ToListAsync();
            var model = Assert.IsAssignableFrom<IEnumerable<Lecturer>>(lecturers);
        }

        [Fact]
        public async Task GetOverall_ReturnsCorrectType()
        {
            var overalls = await _context.Overalls.ToListAsync();
            var model = Assert.IsAssignableFrom<IEnumerable<Overall>>(overalls);
        }

        [Fact]
        public async Task GetPlanning_ReturnsCorrectType()
        {
            var plannings = await _context.Plannings.ToListAsync();
            var model = Assert.IsAssignableFrom<IEnumerable<Planning>>(plannings);
        }

        [Fact]
        public async Task GetSchools_ReturnsCorrectType()
        {
            var schools = await _context.Schools.ToListAsync();
            var model = Assert.IsAssignableFrom<IEnumerable<School>>(schools);
        }

        [Fact]
        public async Task GetStudents_ReturnsCorrectType()
        {
            var students = await _context.Students.ToListAsync();
            var model = Assert.IsAssignableFrom<IEnumerable<Student>>(students);
        }

        [Fact]
        public async Task GetStudentSchools_ReturnsCorrectType()
        {
            var studentSchools = await _context.StudentSchools.ToListAsync();
            var model = Assert.IsAssignableFrom<IEnumerable<StudentSchool>>(studentSchools);
        }

        [Fact]
        public async Task GetSubjects_ReturnsCorrectType()
        {
            var subjects = await _context.Subjects.ToListAsync();
            var model = Assert.IsAssignableFrom<IEnumerable<Subject>>(subjects);
        }

        [Fact]
        public async Task GetTeachers_ReturnsCorrectType()
        {
            var teachers = await _context.Teachers.ToListAsync();
            var model = Assert.IsAssignableFrom<IEnumerable<Teacher>>(teachers);
        }

        [Fact]
        public async Task GetUsers_ReturnsCorrectType()
        {
            var users = await _context.Users.ToListAsync();
            var model = Assert.IsAssignableFrom<IEnumerable<User>>(users);
        }
    }
}