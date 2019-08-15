using PdfMark.Library;
using System;
using System.IO;

namespace PdfMark.ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // args = new string[] { @"C:\_ToolsDataCache\DwgToPdf\636947379013796758", "test" };

            if (args.Length != 2)
            {
                Console.WriteLine("Required args are: \"FOLDER_PATH\" \"WATERMARK\"");
                return;
            }

            var pdfDirectoryPath = args[0];
            var watermark = args[1];

            var destinationDirectoryPath = Path.Combine(pdfDirectoryPath, "watermarked");
            var cursorTop = Console.CursorTop;

            new PdfWatermarker().AddWatermark_Batch(pdfDirectoryPath, watermark, destinationDirectoryPath, (ratio, text) =>
            {
                Console.SetCursorPosition(0, cursorTop);
                Console.Write($"{(int)(ratio * 100)}% {text}");
            });

            Console.SetCursorPosition(0, cursorTop);
            Console.WriteLine($"{100}%");
        }
    }
}
