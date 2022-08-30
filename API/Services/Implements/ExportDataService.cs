using API.Data;
using API.Entities;
using API.Services.Interfaces;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace API.Services.Implements
{
    public class ExportDataService : IExportDataService
    {
        private readonly BuildingContext _context;

        public ExportDataService(BuildingContext context)
        {
            _context = context;
        }
        public byte[] ExportRentedItem()
        {
            var items = _context.Items
                .Include(i => i.Type)
                .Include(i => i.Renter)
                .ToList();
            using (var workBook = new XLWorkbook())
            {
                var worksheet = workBook.Worksheets.Add();
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Id";
                worksheet.Cell(currentRow, 2).Value = "Type";
                worksheet.Cell(currentRow, 3).Value = "Location";
                worksheet.Cell(currentRow, 4).Value = "Price";
                worksheet.Cell(currentRow, 5).Value = "Renter";
                worksheet.Cell(currentRow, 6).Value = "RenterId";
                worksheet.Cell(currentRow, 7).Value = "RentedDate";
                worksheet.Cell(currentRow, 8).Value = "NumberOfParents";

                foreach (var item in items)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.Id;
                    worksheet.Cell(currentRow, 2).Value = item.Type.Name;
                    worksheet.Cell(currentRow, 3).Value = item.Location;
                    worksheet.Cell(currentRow, 4).Value = item.Price;
                    worksheet.Cell(currentRow, 5).Value = item.Renter != null ? item.Renter.UserName : "";
                    worksheet.Cell(currentRow, 6).Value = item.Renter != null ? item.Renter.Id : "";
                    worksheet.Cell(currentRow, 7).Value = item.Renter != null ? item.RentedDate : "";
                    worksheet.Cell(currentRow, 8).Value = item.Renter != null ? item.Renter.NumberOfParent : "";
                }

                using (var stream = new MemoryStream())
                {
                    workBook.SaveAs(stream);
                    var content = stream.ToArray();
                    return content;
                }
            }
        }
    }
}
