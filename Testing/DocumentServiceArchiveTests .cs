using Business.Services.Documents;
using Business.Services.PaymentValidation;
using Business.Services.Report;
using DataAccess;
using DataAccess.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Testing.Services
{
    public class DocumentServiceArchiveTests : IDisposable
    {
        private readonly DbContextOptions<AppDbContext> _options;
        private AppDbContext _context;
        private IRepo<Document> _documentRepo;
        private IDocumentReport _documentReportMock;
        private IPaymentValidationApi _paymentValidationApiMock;
        private DocumentService _documentService;

        public DocumentServiceArchiveTests()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(_options);
            _documentRepo = new Repo<Document>(_context);
            _documentReportMock = Substitute.For<IDocumentReport>();
            _paymentValidationApiMock = Substitute.For<IPaymentValidationApi>();

            _documentService = new DocumentService(
                _documentReportMock,
                _documentRepo,
                _paymentValidationApiMock
            );
        }

        [Fact]
        public async Task Archive_WhenPaymentIsComplete_ShouldUpdateRefNumberAndSaveDocument()
        {
            // Arrange
            var document = new Document
            {
                Id = 1,
                DocumentName = "Test Document",
                RefNumber = "OLD-REF-123",
                Student = new Student
                {
                    Id = 10,
                    Name = "John Doe",
                    Major = "Computer Science"
                },
                DocumentProperties = new List<DocumentProperty>()
            };

            _paymentValidationApiMock.CheckPayment(document).Returns(Task.FromResult(true));

            // Act
            await _documentService.Archive(document);

            // Assert
            // Verify RefNumber changed to a new GUID
            document.RefNumber.Should().NotBe("OLD-REF-123");
            Guid.Parse(document.RefNumber).Should().NotBeEmpty();

            // Verify document saved to database
            var savedDocument = await _context.Documents.FirstOrDefaultAsync(d => d.Id == 1);
            savedDocument.Should().NotBeNull();
            savedDocument.RefNumber.Should().Be(document.RefNumber);

            // Verify CheckPayment was called
            await _paymentValidationApiMock.Received(1).CheckPayment(document);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}