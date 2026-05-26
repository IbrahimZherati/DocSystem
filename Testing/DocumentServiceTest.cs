using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using NSubstitute;
using FluentAssertions;
using DataAccess;
using DataAccess.Entities;

namespace Testing.Services
{
    public class DocumentServiceTests
    {
        private readonly IRepo<Document> _documentRepoMock = Substitute.For<IRepo<Document>>();
        
        // كتابة المسار الكامل الصريح للواجهة لقطع الشك باليقين ومجابهة الخطأ
        private readonly Business.Services.Documents.IDocumentReport _documentReportMock = Substitute.For<Business.Services.Documents.IDocumentReport>();
        
        private readonly Business.Services.Documents.DocumentService _documentService;

        public DocumentServiceTests()
        {
            // بناء الخدمة بالترتيب الصحيح المعتمد في مشروعكم
            _documentService = new Business.Services.Documents.DocumentService(_documentReportMock, _documentRepoMock);
        }

        [Fact]
        public async Task Print_WhenDocumentExists_ShouldReturnByteArray()
        {
            // 1. Arrange
            var documentId = 1;
            var expectedBytes = new byte[] { 0x20, 0x26, 0x42, 0x46 };

            var fakeDocument = new Document
            {
                Id = documentId,
                DocumentName = "Graduation Certificate",
                RefNumber = "REF-2026-XYZ",
                Student = new Student { Id = 10 }, 
                DocumentProperties = new List<DocumentProperty>()
            };

            var fakeQuery = new List<Document> { fakeDocument }.AsQueryable();
            _documentRepoMock.GetQuery().Returns(fakeQuery);
            _documentReportMock.Report(fakeDocument).Returns(Task.FromResult(expectedBytes));

            // 2. Act
            var result = await _documentService.Print(documentId);

            // 3. Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedBytes);
        }

        [Fact]
        public async Task Print_WhenDocumentDoesNotExist_ShouldThrowException()
        {
            // 1. Arrange
            var nonExistingId = 99;
            var emptyQuery = new List<Document>().AsQueryable();
            _documentRepoMock.GetQuery().Returns(emptyQuery);

            // 2. Act
            Func<Task> act = async () => await _documentService.Print(nonExistingId);

            // 3. Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("document not found");
        }
    }
}