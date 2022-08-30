using API.Entities;
using System.Collections.Generic;

namespace API.Services.Interfaces
{
    public interface IExportDataService
    {
        public byte[] ExportRentedItem();
    }
}
