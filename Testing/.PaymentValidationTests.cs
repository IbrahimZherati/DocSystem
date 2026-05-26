using Xunit;
using FluentAssertions;
using Business.Services.PaymentValidation;
using DataAccess.Entities;
using System.Net.Http;
using System.Threading.Tasks;

namespace Testing
{
    public class PaymentValidationTests
    {
        [Fact]
        public async Task CheckPayment_ShouldReturnTrue()
        {
            // 1. Arrange (التجهيز)
            var httpClient = new HttpClient();
            var service = new PaymentValidationApi(httpClient);
            var document = new Document();

            // 2. Act (التنفيذ)
            var result = await service.CheckPayment(document);

            // 3. Assert (التأكيد)
            result.Should().BeTrue();
        }
    }
}