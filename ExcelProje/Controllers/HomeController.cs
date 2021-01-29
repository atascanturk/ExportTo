using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ExcelProje.Models;
using OfficeOpenXml;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Data;
using FastMember;

namespace ExcelProje.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult PdfGetir()
        {
            DataTable dataTable = new DataTable();

            dataTable.Load(ObjectReader.Create(new List<Musteri>
            {
                new Musteri{Id=1,Ad="Yavuz"},
                new Musteri{Id=2,Ad="Ayse"}
            }));


            string fileName = Guid.NewGuid() + ".pdf";

            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/documents/" + fileName);

            var stream = new FileStream(path, FileMode.Create);



            Document document = new Document(PageSize.A4, 25f, 25f, 25f, 25f);

            PdfWriter.GetInstance(document, stream);

            document.Open();

            //Paragraph paragraph = new Paragraph("Yavuz Selim KAHRAMAN");

            PdfPTable pdfPTable = new PdfPTable(dataTable.Columns.Count);

            //pdfPTable.AddCell("Ad");
            //pdfPTable.AddCell("Soyad");

            //pdfPTable.AddCell("Yavuz");
            //pdfPTable.AddCell("Kahraman");


            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                pdfPTable.AddCell(dataTable.Columns[i].ColumnName);
            }


            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    pdfPTable.AddCell(dataTable.Rows[i][j].ToString());
                }
            }



            document.Add(pdfPTable);

            document.Close();
            return File("/documents/" + fileName, "application/pdf", fileName);
        }


        public IActionResult ExcelGetir()
        {
            ExcelPackage excelPackage = new ExcelPackage();

            var excelBlank = excelPackage.Workbook.Worksheets.Add("Calisma1");

            //excelBlank.Cells[1, 1].Value = "Ad";
            //excelBlank.Cells[1, 2].Value = "Soyad";

            //excelBlank.Cells[2, 1].Value = "Yavuz";
            //excelBlank.Cells[2, 2].Value = "KAHRAMAN";


            excelBlank.Cells["A1"].LoadFromCollection(new List<Musteri>
            {
                new Musteri{Id=1,Ad="Yavuz"},
                new Musteri{Id=2,Ad="Ayse"}
            }, true, OfficeOpenXml.Table.TableStyles.Light15);


            var bytes = excelPackage.GetAsByteArray();

            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Guid.NewGuid() + "" + ".xlsx");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    class Musteri
    {
        public int Id { get; set; }
        public string Ad { get; set; }
    }
}
