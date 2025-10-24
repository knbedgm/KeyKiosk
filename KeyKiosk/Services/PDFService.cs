using KeyKiosk.Data;
using KeyKiosk.Migrations;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;

namespace KeyKiosk.Services;

public class PDFService
{
    public byte[] GenerateWorkOrderWithTasksPDF(WorkOrder workOrder)
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

    public byte[] GenerateEfficiencyReport(List<WorkOrder> workOrderList, 
                                            List<WorkOrderTaskTemplate> templateList, 
                                            DateTimeOffset startDate,
                                            DateTimeOffset endDate)
    {
        if (workOrderList == null || workOrderList.Count() == 0)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);

                    page.Header().Text(text =>
                    {
                        text.AlignCenter();
                        text.Span("There were no work orders between").FontSize(30).Bold();
                        text.EmptyLine();
                        text.EmptyLine();
                        text.Span($"{startDate.Date:MMMM dd, yyyy}").FontSize(30).Bold();
                        text.EmptyLine();
                        text.Span("and").FontSize(30).Bold();
                        text.EmptyLine();
                        text.Span($"{endDate.Date:MMMM dd, yyyy}").FontSize(30).Bold();
                    });
                });
            });
            return document.GeneratePdf();
        }
        else
        {
            int totalTheoreticalDays = 0;
            int totalActualDays = 0;
            List<EfficiencyReportData> efficiencyList = new List<EfficiencyReportData>();
            EfficiencyReportData tempWorkOrderData = new EfficiencyReportData();
            EfficiencyReportTaskList tempTaskListData = new EfficiencyReportTaskList();

            foreach (WorkOrder workOrder in workOrderList)
            {
                tempWorkOrderData = new EfficiencyReportData();
                tempWorkOrderData.VehiclePlate = workOrder.VehiclePlate;
                tempWorkOrderData.WorkOrderDate = workOrder.StartDate;

                foreach (WorkOrderTask task in workOrder.Tasks)
                {
                    totalActualDays += task.DaysForCompletion;

                    tempTaskListData = new EfficiencyReportTaskList();
                    tempTaskListData.TaskTitle = task.Title;
                    tempTaskListData.ActualDays = task.DaysForCompletion;

                    foreach (WorkOrderTaskTemplate template in templateList)
                    {
                        if (task.Title == template.TaskTitle)
                        {
                            totalTheoreticalDays += template.ExpectedDaysForCompletion;
                            tempTaskListData.ExpectedDays = template.ExpectedDaysForCompletion;
                        }
                    }

                    tempWorkOrderData.TasksList.Add(tempTaskListData);
                }

                efficiencyList.Add(tempWorkOrderData);
            }

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);

                    page.Header().Text(text =>
                    {
                        text.AlignCenter();
                        text.Span("Efficiency Report").FontSize(30).Bold();
                        text.EmptyLine();
                        text.EmptyLine();
                        text.Span($"{startDate.Date:MMMM dd, yyyy} to {endDate.Date:MMMM dd, yyyy}").FontSize(20).Bold();
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        col.Item().PaddingBottom(10).Text($"The total expected time for tasks is {totalTheoreticalDays}");
                        col.Item().PaddingBottom(10).Text($"The total actual time for tasks is {totalActualDays}");
                        col.Item().Text($"The efficiency is %{Math.Round(((float)totalTheoreticalDays / (float)totalActualDays) * 100, 2)}");

                        foreach (EfficiencyReportData item in efficiencyList)
                        {
                            col.Item().PaddingTop(40);
                            if (item.WorkOrderDate != null)
                            {
                                col.Item().PaddingBottom(30).Text($"Tasks for: {item.VehiclePlate}                        Date: {item.WorkOrderDate:MMMM dd, yyyy}").Bold();
                            }
                            else { 
                                col.Item().PaddingBottom(30).Text($"Tasks for: {item.VehiclePlate}").Bold();
                            }

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
                                    header.Cell().Element(CellStyle).Text("Task");
                                    header.Cell().Element(CellStyle).Text("Expected");
                                    header.Cell().Element(CellStyle).Text("Actual");

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
                                foreach (EfficiencyReportTaskList task in item.TasksList)
                                {
                                    table.Cell().Element(CellStyle).Text(task.TaskTitle);
                                    table.Cell().Element(CellStyle).Text(task.ExpectedDays.ToString());
                                    table.Cell().Element(CellStyle).Text(task.ActualDays.ToString());

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
                        }
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
}

public class EfficiencyReportTaskList()
{
    public string TaskTitle { get; set; } = "";
    public int ExpectedDays { get; set; }
    public int ActualDays { get; set; }
}

public class EfficiencyReportData()
{
    public string VehiclePlate { get; set; } = "";
    public DateTimeOffset? WorkOrderDate { get; set; } = new DateTimeOffset();
    public List<EfficiencyReportTaskList> TasksList { get; set; } = new List<EfficiencyReportTaskList>();
}