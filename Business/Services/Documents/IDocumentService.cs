using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Documents
{
    public interface IDocumentService
    {
        Task<byte[]> Print(int Id);
        Task Archive(Document document);
        Task<bool> CheckValid(string refNumber);

        Task Remove(int Id);
        Task<List<Document>> GetDocuments();
    }
}
