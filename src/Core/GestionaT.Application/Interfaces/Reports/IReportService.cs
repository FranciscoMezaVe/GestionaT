using GestionaT.Domain.Entities;

namespace GestionaT.Application.Interfaces.Reports
{
    public interface IReportService
    {
        Task<byte[]> GenerateSaleReportPdfAsync(List<Sale> data);
    }
}
