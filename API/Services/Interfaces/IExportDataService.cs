using API.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services.Interfaces
{
    public interface IExportDataService
    {
        Task<byte[]> ExportAccounts(DateTime from, DateTime to);
        byte[] ExportBills(DateTime from, DateTime to);
        byte[] ExportMembers(DateTime from, DateTime to);
        byte[] ExportRentedItem(DateTime from, DateTime to);
        byte[] ExportRentRequests(DateTime from, DateTime to);
        byte[] ExportReports(DateTime from, DateTime to);
        byte[] ExportUnRentRequests(DateTime from, DateTime to);
        byte[] ExportVehicles(DateTime from, DateTime to);
    }
}
