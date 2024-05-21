using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;

namespace Cts.PDF.Common
{
    class PDFHelper
    {
        static void Main(string[] args)
        {
            //// Example usage of the functions
            //try
            //{
            //   foreach(string pdfPath in Directory.GetFiles(@"C:\Projects\Cts.PDF.Common\Cts.PDF.Common\TestFolder\"))
            //    {
            //        ExtractPages(pdfPath, pdfPath + "_splited.pdf", new List<int> { 1, 2, 3, 4, 5, 6 });
            //    }


            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"An error occurred: {ex.Message}");
            //}
        }

        public static void CombinePDFs(List<string> fileNames, string outputFilePath)
        {
            if (fileNames == null || fileNames.Count == 0)
                throw new ArgumentException("File names list cannot be null or empty");

            using (FileStream stream = new FileStream(outputFilePath, FileMode.Create))
            {
                Document document = new Document();
                PdfCopy pdf = new PdfCopy(document, stream);
                PdfReader reader = null;

                try
                {
                    document.Open();
                    foreach (string file in fileNames)
                    {
                        if (!File.Exists(file))
                            throw new FileNotFoundException($"File {file} not found");

                        reader = new PdfReader(file);
                        for (int i = 1; i <= reader.NumberOfPages; i++)
                        {
                            pdf.AddPage(pdf.GetImportedPage(reader, i));
                        }
                        pdf.FreeReader(reader);
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error while combining PDFs: " + ex.Message);
                }
                finally
                {
                    if (document != null)
                    {
                        document.Close();
                    }
                    if (reader != null)
                    {
                        reader.Close();
                    }
                }
            }
        }

        public static void SplitPDF(string inputFilePath, string outputFilePrefix, int pagesPerSplit)
        {
            if (!File.Exists(inputFilePath))
                throw new FileNotFoundException("Input file not found");

            if (pagesPerSplit < 1)
                throw new ArgumentException("Pages per split must be at least 1");

            PdfReader reader = new PdfReader(inputFilePath);
            int totalPages = reader.NumberOfPages;
            int splitIndex = 1;

            try
            {
                for (int page = 1; page <= totalPages; page += pagesPerSplit)
                {
                    Document document = new Document();
                    string outputFilePath = $"{outputFilePrefix}{splitIndex}.pdf";
                    using (FileStream fs = new FileStream(outputFilePath, FileMode.Create))
                    {
                        PdfCopy copy = new PdfCopy(document, fs);
                        document.Open();
                        for (int i = page; i < page + pagesPerSplit && i <= totalPages; i++)
                        {
                            copy.AddPage(copy.GetImportedPage(reader, i));
                        }
                        document.Close();
                    }
                    splitIndex++;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while splitting PDF: " + ex.Message);
            }
            finally
            {
                reader.Close();
            }
        }

        public static int CountPages(string inputFilePath)
        {
            if (!File.Exists(inputFilePath))
                throw new FileNotFoundException("Input file not found");

            PdfReader reader = new PdfReader(inputFilePath);
            int totalPages = reader.NumberOfPages;
            reader.Close();
            return totalPages;
        }

        public static void ExtractPages(string inputFilePath, string outputFilePath, List<int> pageNumbers)
        {
            if (!File.Exists(inputFilePath))
                throw new FileNotFoundException("Input file not found");

            if (pageNumbers == null || pageNumbers.Count == 0)
                throw new ArgumentException("Page numbers list cannot be null or empty");

            PdfReader reader = new PdfReader(inputFilePath);

            try
            {
                Document document = new Document();
                using (FileStream fs = new FileStream(outputFilePath, FileMode.Create))
                {
                    PdfCopy copy = new PdfCopy(document, fs);
                    document.Open();

                    foreach (int pageNumber in pageNumbers)
                    {
                        if (pageNumber < 1 || pageNumber > reader.NumberOfPages)
                        {
                            continue;
                            //throw new ArgumentOutOfRangeException($"Invalid page number: {pageNumber}");
                        }
                        else
                        {
                            copy.AddPage(copy.GetImportedPage(reader, pageNumber));
                        }
                            

                        
                    }
                    document.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while extracting pages: " + ex.Message);
            }
            finally
            {
                reader.Close();
            }
        }
    }
}
