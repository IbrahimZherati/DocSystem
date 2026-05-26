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
            // إنشاء الـ Substitutes (النسخ الوهمية)
            _documentReportMock = Substitute.For<IDocumentReport>();
            _documentRepoMock = Substitute.For<IRepo<Document>>();
            _paymentValidationApiMock = Substitute.For<IPaymentValidationApi>();

            // حقن الاعتماديات مباشرة
            _documentService = new DocumentService(
                _documentReportMock,
                _documentRepoMock,
                _paymentValidationApiMock
            );
        }

        // =======================================================
        // 1. اختبار دالة الـ Remove
        // =======================================================
        [Fact]
        public async Task Remove_WhenCalled_ShouldGetDocumentRemoveItAndSave()
        {
            // Arrange (التجهيز)
            int documentId = 10;
            var fakeDocument = new Document { Id = documentId };

            // نُخبر الـ Repo الوهمي عندما يُستدعى بـ GetByIdAsync أن يُعيد المستند الوهمي
            _documentRepoMock.GetByIdAsync(documentId).Returns(Task.FromResult(fakeDocument));

            // Act (التنفيذ)
            await _documentService.Remove(documentId);

            // Assert (التحقق باستخدام NSubstitute)
            // نتحقق أن الـ Repo قام فعلياً باستدعاء الـ Remove للمستند الصحيح مرة واحدة
            _documentRepoMock.Received(1).Remove(fakeDocument);

            // ونتحقق أنه تم حفظ التغييرات في قاعدة البيانات مرة واحدة
            await _documentRepoMock.Received(1).SaveAsync();
        }

        // =======================================================
        // 2. اختبار دالة الـ CheckValid (في حالة وجود المستند)
        // =======================================================
        [Fact]
        public async Task CheckValid_WhenRefNumberExists_ShouldReturnTrue()
        {
            // Arrange
            string existingRef = "QR-ABC-123";

            // نُحاكي دالة ExistsAsync بحيث إذا استقبلت أي تعبير (Expression) تُرجع true
            // استخدمنا Arg.Any لأن الدوال التي تستقبل Lambda مثل (d => d.RefNumber == refNumber) نختبرها هكذا
            _documentRepoMock.ExistsAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Document, bool>>>())
                             .Returns(Task.FromResult(true));

            // Act
            bool result = await _documentService.CheckValid(existingRef);

            // Assert
            Assert.True(result); // نضمن أن النتيجة الراجعة هي true فعلاً
        }

        // =======================================================
        // 3. اختبار دالة الـ CheckValid (في حالة عدم وجود المستند)
        // =======================================================
        [Fact]
        public async Task CheckValid_WhenRefNumberDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            string nonExistingRef = "QR-XYZ-999";

            _documentRepoMock.ExistsAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Document, bool>>>())
                             .Returns(Task.FromResult(false));

            // Act
            bool result = await _documentService.CheckValid(nonExistingRef);

            // Assert
            Assert.False(result); // نضمن أن النتيجة الراجعة هي false
        }
    }
}