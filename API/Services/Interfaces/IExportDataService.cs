using API.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services.Interfaces
{
    public interface IExportDataService
    {
        Task<byte[]> ExportAccounts();
        byte[] ExportBills();
        byte[] ExportMembers();
        byte[] ExportRentedItem();
        byte[] ExportRentRequests();
        byte[] ExportReports();
        byte[] ExportUnRentRequests();
        byte[] ExportVehicles();
    }
}
