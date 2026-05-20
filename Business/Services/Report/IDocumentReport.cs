using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Report
{
    public interface IDocumentReport
    {
        Task<byte[]?> Report(Document document);
    }
}
