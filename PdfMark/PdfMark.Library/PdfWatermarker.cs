using PdfSharp.Drawing;
using PdfSharp.Pdf.IO;
using System;
using System.IO;

namespace PdfMark.Library
{
    public class PdfWatermarker
    {
        public XFont WatermarkFont { get; private set; } = new XFont("Arial", 55);
        public XFont WatermarkFont_Small { get; private set; } = new XFont("Arial", 20);
        public XBrush WatermarkBrush { get; private set; } = new XSolidBrush(XColor.FromArgb(128, 255, 0, 0));
        public XStringFormat Format { get; private set; } = new XStringFormat() { Alignment = XStringAlignment.Near, LineAlignment = XLineAlignment.Near };

        public void AddWatermark_Batch(string pdfDirectoryPath, string watermark, string destinationDirectoryPath, Action<float, string> setProgressRatio)
        {
            Directory.CreateDirectory(destinationDirectoryPath);
            var filePaths = Directory.GetFiles(pdfDirectoryPath, "*.pdf");
            for (var i = 0; i < filePaths.Length; i++)
            {
                var filePath = filePaths[i];
                var copyFilePath = Path.Combine(destinationDirectoryPath, filePath.Replace(pdfDirectoryPath, "").TrimStart('\\'));

                setProgressRatio(i * 1.0f / filePaths.Length, $"Watermarking {Path.GetFileName(filePath)}");
                File.Copy(filePath, copyFilePath, overwrite: true);
                AddWatermark(copyFilePath, watermark);
            }

            setProgressRatio(1, $"Done");
        }

        public void AddWatermark(string pdfFilePath, string watermark)
        {
            // Add Watermark
            using (var doc = PdfReader.Open(pdfFilePath, PdfDocumentOpenMode.Modify))
            {
                foreach (var page in doc.Pages)
                {
                    var gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Prepend);

                    var font = WatermarkFont;
                    var size = gfx.MeasureString(watermark, font);

                    if (size.Width > page.Width * 0.7)
                    {
                        font = WatermarkFont_Small;
                        size = gfx.MeasureString(watermark, font);
                    }

                    gfx.DrawString(watermark, font, WatermarkBrush, new XPoint((page.Width - size.Width) / 2, 0.0), Format);
                }

                doc.Save(pdfFilePath);
                doc.Close();
            }
        }
    }
}
