using Business.Helper;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.PaymentValidation
{
    public class PaymentValidationApi : IPaymentValidationApi
    {
        private readonly HttpClient http;

        public PaymentValidationApi(HttpClient http)
        {
            this.http = http;
        }
        public async Task<bool> CheckPayment(Document document)
        {
            return true;
            var valid = await http.GetFromJsonAsync<bool>(RouteHelper.PaymentApi);
            return valid;
        }
    }
}
