using GestionaT.Application.Interfaces.Reports;
using GestionaT.Domain.Entities;
using RazorLight;
using HtmlRendererCore.PdfSharp;

namespace GestionaT.Infraestructure.Reports
{
    public class ReportServiceRazorLight : IReportService
    {
        string rootPath = Path.Combine(AppContext.BaseDirectory, "Reports", "Templates");
        private const string reportSalePath = "SaleReport";

        private readonly RazorLightEngine _engine;

        public ReportServiceRazorLight()
        {
            _engine = new RazorLightEngineBuilder()
                .UseFileSystemProject(rootPath)
                .UseMemoryCachingProvider()
                .Build();
        }
        public async Task<byte[]> GenerateSaleReportPdfAsync(List<Sale> data)
        {
            string htmlContent = await _engine.CompileRenderAsync(reportSalePath, data);

            // Renderizar HTML en PDF (puedes ajustar ancho/alto si quieres)
            var pdf = PdfGenerator.GeneratePdf(htmlContent, PdfSharpCore.PageSize.A4);

            using (MemoryStream ms = new())
            {
                pdf.Save(ms);
                return ms.ToArray();
            }
        }
    }
}
