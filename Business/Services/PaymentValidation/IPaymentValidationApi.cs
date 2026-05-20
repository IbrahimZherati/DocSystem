using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.PaymentValidation
{
    public interface IPaymentValidationApi
    {
        Task<bool> CheckPayment(Document document);
    }
}
