using QRCoder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
namespace DataAccess.Entities
{
    public class Document
    {
        public int Id { get;  set; }

        [JsonPropertyName("documentName")]
        public string? DocumentName { get; set; }

        [JsonPropertyName("studentId")]
        public int? StudentId { get; set; }

        [JsonPropertyName("documentProperties")]
        public ICollection<DocumentProperty> DocumentProperties { get; set; } = new List<DocumentProperty>();

        [JsonPropertyName("refNumber")]
        public string RefNumber { get;  set; } = null!;
        public string? QR { get;  set; }

        public virtual Student? Student { get; set; }



        public void GenerateQRCode()
        {
            using var qrCodeData = QRCodeGenerator.GenerateQrCode(RefNumber, QRCodeGenerator.ECCLevel.Q);
            using var svgRenderer = new SvgQRCode(qrCodeData);
            string svg = new(svgRenderer.GetGraphic());
            QR = svg;
        }

    }
}
