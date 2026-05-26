using Business.Services.Students;
using DataAccess.Entities;
using NSubstitute;

namespace Testing;

public class StudentTests
{
    private readonly StudentService _studentService;
    private readonly IRepo<Student> _studentRepoMock = Substitute.For<IRepo<Student>>();

    public StudentTests()
    {
        _studentService = new StudentService(_studentRepoMock);
    }   
    
    [Fact]
    public async Task GetStudents_ShouldReturnAllStudents_WhenStudentsExist()
    {
        // Arrange
        var expectedStudents = new List<Student>
        {
            new() { Id = 1, Name = "Edmon", Major = "Computer Science" },
            new() { Id = 2, Name = "Sarah", Major = "Mathematics" }
        };
        _studentRepoMock.GetAllAsync().Returns(expectedStudents);

        // Act
        var students = await _studentService.GetStudents();

        // Assert
        await _studentRepoMock.Received(1).GetAllAsync();
        Assert.Equal(expectedStudents.Count, students.Count);
        Assert.Equal(expectedStudents, students);
    }
}