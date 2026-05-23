using DataAccess.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using NSubstitute;
using Xunit;



public class RepoTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly Repo<Document> _repo;

    public RepoTests()
    {
        // Use an In-Memory Database for reliable EF Core behavior
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("DBTest")
            .Options;

        _context = new AppDbContext(options);
        _repo = new Repo<Document>(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddEntityToDatabase()
    {
        // Arrange
        var entity = new Document { Id = 1, DocumentName = "Test item" , RefNumber = Guid.NewGuid().ToString() };

        // Act
        await _repo.AddAsync(entity);
        await _repo.SaveAsync();

        // Assert
        var result = await _context.Set<Document>().FindAsync(1);
        result.Should().NotBeNull();
        result!.DocumentName.Should().Be("Test item");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectEntity()
    {
        // Arrange
        var entity = new Document { Id = 2, DocumentName = "Target", RefNumber = Guid.NewGuid().ToString() };
        await _context.Set<Document>().AddAsync(entity);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repo.GetByIdAsync(2);

        // Assert
        result.Should().NotBeNull();
        result!.DocumentName.Should().Be("Target");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        // Arrange
        var entities = new List<Document>
        {
            new() { Id = 3, DocumentName = "Item 1" , RefNumber = Guid.NewGuid().ToString()},
            new() { Id = 4, DocumentName = "Item 2" , RefNumber = Guid.NewGuid().ToString()}
        };
        await _context.Set<Document>().AddRangeAsync(entities);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repo.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(e => e.DocumentName == "Item 1");
    }

    [Fact]
    public async Task Update_ShouldModifyExistingEntity()
    {
        // Arrange
        var entity = new Document { Id = 5, DocumentName = "Original" , RefNumber = Guid.NewGuid().ToString() };
        await _context.Set<Document>().AddAsync(entity);
        await _context.SaveChangesAsync();

        // Act
        entity.DocumentName = "Updated";
        _repo.Update(entity);
        await _repo.SaveAsync();

        // Assert
        var result = await _context.Set<Document>().FindAsync(5);
        result!.DocumentName.Should().Be("Updated");
    }

    [Fact]
    public async Task Remove_ShouldDeleteEntity()
    {
        // Arrange
        var entity = new Document { Id = 6, DocumentName = "To Delete" , RefNumber = Guid.NewGuid().ToString() };
        await _context.Set<Document>().AddAsync(entity);
        await _context.SaveChangesAsync();

        // Act
        _repo.Remove(entity);
        await _repo.SaveAsync();

        // Assert
        var result = await _context.Set<Document>().FindAsync(6);
        result.Should().BeNull();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenConditionMatches()
    {
        // Arrange
        var entity = new Document { Id = 7, DocumentName = "SearchMe" , RefNumber = Guid.NewGuid().ToString() };
        await _context.Set<Document>().AddAsync(entity);
        await _context.SaveChangesAsync();

        // Act
        var exists = await _repo.ExistsAsync(x => x.DocumentName == "SearchMe");

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task AddRangeAsync_ShouldAddMultipleEntities()
    {
        // Arrange
        var entities = new List<Document>
        {
            new() { Id = 8, DocumentName = "Batch 1" , RefNumber = Guid.NewGuid().ToString() },
            new() { Id = 9, DocumentName = "Batch 2" , RefNumber = Guid.NewGuid().ToString()}
        };

        // Act
        await _repo.AddRangeAsync(entities);
        await _repo.SaveAsync();

        // Assert
        var count = await _context.Set<Document>().CountAsync();
        count.Should().Be(2);
    }

    [Fact]
    public async Task RemoveRange_ShouldDeleteMultipleEntities()
    {
        // Arrange
        var entities = new List<Document>
        {
            new() { Id = 10, DocumentName = "Del 1" , RefNumber = Guid.NewGuid().ToString() },
            new() { Id = 11, DocumentName = "Del 2" , RefNumber = Guid.NewGuid().ToString()}
        };
        await _context.Set<Document>().AddRangeAsync(entities);
        await _context.SaveChangesAsync();

        // Act
        _repo.RemoveRange(entities);
        await _repo.SaveAsync();

        // Assert
        var count = await _context.Set<Document>().CountAsync();
        count.Should().Be(0);
    }

    [Fact]
    public async Task SaveAsync_ShouldSaveChanges()
    {
        //Arrange
        var newDocument = new Document
        {
            Id = 1,
            DocumentName = "Doc Test",
            RefNumber = Guid.NewGuid().ToString()
        };
        await _context.Documents.AddAsync(newDocument);
        //Act
        var firstResult = await _context.Documents
       .AsNoTracking()  // ← Ignores tracked entities
       .FirstOrDefaultAsync(x => x.Id == 1);
        await _repo.SaveAsync();
        var secondResult = await _context.Documents.FindAsync(1);

        //Assert
        firstResult.Should().BeNull();
        secondResult.Should().NotBeNull();
    }

    [Fact]
    public async Task GetQuery_ShouldReturnQuerable()
    {
        //Arrange

        //Act
        var query = _repo.GetQuery();
        //Assert
        query.Should().BeAssignableTo<IQueryable<Document>>();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
