using NSubstitute;
using Xunit;
using System;
using System.Threading.Tasks;
using DataAccess.Entities;
using Business.Services.Documents;
using Business.Services.Report;
using Business.Services.PaymentValidation;

namespace Business.Tests
{
    public class DocumentServiceTests
    {
        private readonly IDocumentReport _documentReportMock;
        private readonly IRepo<Document> _documentRepoMock;
        private readonly IPaymentValidationApi _paymentValidationApiMock;
        private readonly DocumentService _documentService;

        public DocumentServiceTests()
        {
            // Initialize our mock dependencies using NSubstitute
            _documentReportMock = Substitute.For<IDocumentReport>();
            _documentRepoMock = Substitute.For<IRepo<Document>>();
            _paymentValidationApiMock = Substitute.For<IPaymentValidationApi>();

            // Inject the mocked dependencies into the real service
            _documentService = new DocumentService(
                _documentReportMock,
                _documentRepoMock,
                _paymentValidationApiMock
            );
        }

        [Fact]
        public async Task Remove_WhenDocumentExists_ShouldRetrieveFromRepoRemoveItAndSaveChanges()
        {
            // ==========================================
            // 1. ARRANGE (Setup the realistic scenario)
            // ==========================================
            int targetDocumentId = 42; // A realistic ID example

            // This represents the actual document that exists in your system
            var existingDocument = new Document
            {
                Id = targetDocumentId,
                RefNumber = "REF-2026-XYZ",
                //Title = "Archived_University_Agreement"
            };

            // Scenario Setup: When the service asks the repo for this ID, return our existing document
            _documentRepoMock.GetByIdAsync(targetDocumentId).Returns(Task.FromResult(existingDocument));

            // ==========================================
            // 2. ACT (Execute the actual service method)
            // ==========================================
            await _documentService.Remove(targetDocumentId);

            // ==========================================
            // 3. ASSERT (Verify the mock interactions)
            // ==========================================

            // Assert A: Verify that GetByIdAsync was actually called with the exact ID passed
            await _documentRepoMock.Received(1).GetByIdAsync(targetDocumentId);

            // Assert B: Verify that the exact document object retrieved was passed into the Remove method
            // This proves it wasn't just "any" document removed, but the specific one from the database list
            _documentRepoMock.Received(1).Remove(existingDocument);

            // Assert C: Verify that the unit of work/changes were saved to the database context
            await _documentRepoMock.Received(1).SaveAsync();
        }
    }
}