using API.Payloads.Requests;
using System;

namespace API.Services.Interfaces
{
    public interface IPdfService
    {
        byte[] GeneratePDF(PdfBillRequest request);
        byte[] ExportBills(DateTime from, DateTime to);
        byte[] ExportMembers(DateTime from, DateTime to);
        byte[] ExportRentedItem(DateTime from, DateTime to);
        byte[] ExportRentRequests(DateTime from, DateTime to);
        byte[] ExportReports(DateTime from, DateTime to);
        byte[] ExportUnRentRequests(DateTime from, DateTime to);
        byte[] ExportVehicles(DateTime from, DateTime to);
    }
}
