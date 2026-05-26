using Business.Services.Documents;
using Business.Services.PaymentValidation;
using Business.Services.Report;
using DataAccess;
using DataAccess.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Testing.Services
{
	public class DocumentServiceTests : IDisposable
	{
		private readonly IRepo<Document> _repo;

		// كتابة المسار الكامل الصريح للواجهة لقطع الشك باليقين ومجابهة الخطأ
		private readonly IDocumentReport _documentReportMock = Substitute.For<IDocumentReport>();
		private readonly IPaymentValidationApi paymentValidationApi = Substitute.For<IPaymentValidationApi>();

		private readonly AppDbContext _context;

		private readonly DocumentService _documentService;

		public DocumentServiceTests()
		{

			// Use an In-Memory Database for reliable EF Core behavior
			var options = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase("DBTest2")
				.Options;

			_context = new AppDbContext(options);
			_repo = new Repo<Document>(_context);
			// بناء الخدمة بالترتيب الصحيح المعتمد في مشروعكم
			_documentService = new DocumentService(_documentReportMock, _repo, paymentValidationApi);
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
				Student = new Student { Id = 10, Name = "2", Major = "3" },
				DocumentProperties = new List<DocumentProperty>()
			};
			await _repo.AddAsync(fakeDocument);
			await _repo.SaveAsync();

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

			// 2. Act
			Func<Task> act = async () => await _documentService.Print(nonExistingId);

			// 3. Assert
			await act.Should().ThrowAsync<Exception>()
				.WithMessage("document not found");
		}

		public void Dispose()
		{
			_context.Database.EnsureDeleted();
			_context.Dispose();
		}
	}
}