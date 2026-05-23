using DataAccess.Entities;
using FluentAssertions;
using QRCoder;
using Svg;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using ZXing.Windows.Compatibility;
namespace Testing.DataAccess.Entities.Documents
{
    public class DocumentTests
    {


        [Fact]
        public async Task GenerateQRCode_ShouldGenerateQR()
        {

            //Arrange
            var refNum = Guid.NewGuid().ToString();
            var document = new Document
            {
                Id = 1,
                DocumentName = "Doc Test",
                RefNumber = refNum
            };

            //Act
            document.GenerateQRCode();
            var svgDocument = SvgDocument.FromSvg<SvgDocument>(document.QR);
            using var bitmap = svgDocument.Draw(300, 300);

            var reader = new BarcodeReader();
            var result = reader.Decode(bitmap).Text;

            //Assert 
            result.Should().Be(refNum);
        }
    }
}
