using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using OfficeOpenXml;
using System;
using OfficeOpenXml.Style;
using Microsoft.AspNetCore.Hosting;
using FPTBook.Models;
using FPTBook.Areas.Identity.Data;
using FPTBook.Utils;
using Microsoft.EntityFrameworkCore;

namespace FPTBook.Controllers
{
    public class EPPlusController : ControllerBase
    {

        private readonly FPTBookIdentityDbContext _context;
        private readonly IWebHostEnvironment hostEnvironment;

        public EPPlusController(FPTBookIdentityDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            hostEnvironment = environment;
        }

        [HttpPost]
        public async Task<IActionResult> ExportV2(CancellationToken cancellationToken)
        {
            // query data from database
            await Task.Yield();
            var fPTContext = _context.Book.Include(b => b.Author).Include(b => b.Category).Include(b => b.Publisher);
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("Sheet1");

                // simple way
                workSheet.Cells.LoadFromCollection(await fPTContext.ToListAsync(), true);

                package.Save();
            }
            stream.Position = 0;
            string excelName = $"UserList-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

            return File(stream, "application/octet-stream", excelName);
        }
    }
}
