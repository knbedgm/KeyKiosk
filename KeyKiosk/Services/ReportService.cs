using KeyKiosk.Data;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;

namespace KeyKiosk.Services;

public class ReportService
{
    public byte[] GenerateReport(WorkOrder workOrder)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(50);
                page.Header().Text("My Report").FontSize(20).Bold().AlignCenter();

                page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                {
                    col.Item().Text($"Name: {workOrder.CustomerName}");
                    col.Item().Text($"Vehicle Plate: {workOrder.VehiclePlate}");
                    col.Item().Text($"Start Date: {workOrder.StartDate:yyyy-MM-dd}");
                    col.Item().Text($"End Date: {workOrder.EndDate:yyyy-MM-dd}");
                    col.Item().Text($"Status: {workOrder.Status}");
                    col.Item().Text("Items:");
                    
                    col.Spacing(10);

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Task").Bold();
                            header.Cell().Text("Details").Bold();
                            header.Cell().Text("Price").Bold();
                        });

                        foreach (var task in workOrder.Tasks)
                        {
                            table.Cell().Text(task.Title);
                            table.Cell().Text(task.Details);
                            table.Cell().Text($"${task.CostCents:F2}");
                        }
                    });
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Page ");
                    x.CurrentPageNumber();
                });
            });
        });

        return document.GeneratePdf();
    }
}
