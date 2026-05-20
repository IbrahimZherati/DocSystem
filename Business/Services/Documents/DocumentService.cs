using Business.Services.PaymentValidation;
using Business.Services.Report;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Documents
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentReport documentReport;
        private readonly IRepo<Document> documentRepo;
        private readonly IPaymentValidationApi paymentValidationApi;

        public DocumentService(IDocumentReport documentReport,
            IRepo<Document> documentRepo,
            IPaymentValidationApi paymentValidationApi)
        {
            this.documentReport = documentReport;
            this.documentRepo = documentRepo;
            this.paymentValidationApi = paymentValidationApi;
        }
        public async Task Archive(Document document)
        {
            if(!await paymentValidationApi.CheckPayment(document))
            {
                throw new Exception("the payment not complete");
            }
            document.RefNumber = Guid.NewGuid().ToString();
            document.GenerateQRCode();
            await documentRepo.AddAsync(document);
            await documentRepo.SaveAsync();
        }

        public async Task<bool> CheckValid(string refNumber)
        {
            return await documentRepo.ExistsAsync(d => d.RefNumber == refNumber);
        }

        public async Task<List<Document>> GetDocuments()
        {
            return await documentRepo.GetQuery()
                .Include(d => d.Student).ToListAsync();
        }

        public async Task<byte[]> Print(int Id)
        {
            var document = await documentRepo.GetQuery()
                .Include(d => d.Student)
                .Include(d => d.DocumentProperties)
                .FirstOrDefaultAsync(d => d.Id == Id);
            if (document == null)
            {
                throw new Exception("document not found");
            }
            var file = await documentReport.Report(document);
            return file;
        }

        public async Task Remove(int Id)
        {
            var document = await documentRepo.GetByIdAsync(Id);
           documentRepo.Remove(document);
            await documentRepo.SaveAsync();
        }
    }
}
