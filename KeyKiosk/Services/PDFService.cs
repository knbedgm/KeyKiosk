using KeyKiosk.Data;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;

namespace KeyKiosk.Services;

public class PDFService
{
    public byte[] GenerateReport(WorkOrder workOrder)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(50);
                page.Header().Text(workOrder.VehiclePlate).FontSize(30).Bold().AlignCenter();

                page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                {
                    if (workOrder.Details != null && workOrder.Details.Length > 0)
                    {
                        col.Item().PaddingBottom(10).Text($"Important Details").FontSize(20).Bold();
                        col.Item().PaddingBottom(40).Text(workOrder.Details);
                    }
                    
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Task");
                            header.Cell().Element(CellStyle).Text("Details");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container
                                    .Background(Colors.Blue.Darken2)
                                    .DefaultTextStyle(x => x.FontColor(Colors.White).Bold())
                                    .PaddingVertical(8)
                                    .PaddingHorizontal(16);
                            }
                        });

                        int tableCellColourIndex = 0;
                        foreach (var task in workOrder.Tasks)
                        {
                            table.Cell().Element(CellStyle).Text(task.Title);
                            table.Cell().Element(CellStyle).Text(task.Details);

                            IContainer CellStyle(IContainer container)
                            {
                                var backgroundColor = tableCellColourIndex % 2 == 0
                                    ? Colors.Blue.Lighten5
                                    : Colors.Blue.Lighten4;

                                return container
                                    .Background(backgroundColor)
                                    .PaddingVertical(8)
                                    .PaddingHorizontal(16);
                            }

                            tableCellColourIndex++;
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
