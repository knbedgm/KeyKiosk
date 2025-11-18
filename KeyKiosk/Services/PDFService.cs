using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using KeyKiosk.Data;
using KeyKiosk.Migrations;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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
                page.Header().Text($"{workOrder.VehiclePlate} (#{workOrder.Id})").FontSize(30).Bold().AlignCenter();

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
                            columns.RelativeColumn(5);
                            columns.RelativeColumn(10);
                            columns.RelativeColumn(3);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Task");
                            header.Cell().Element(CellStyle).Text("Details");
                            header.Cell().Element(CellStyle).Text("Status");

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
                            table.Cell().Element(CellStyle).Text(ConvertTaskStatus(task.Status));

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

    public static string ConvertTaskStatus(WorkOrderTaskStatusType enumStatus)
    {
        if (enumStatus == WorkOrderTaskStatusType.Created)
        {
            return "Todo";
        }
        else if (enumStatus == WorkOrderTaskStatusType.WorkStarted)
        {
            return "Started";
        }
        else if (enumStatus == WorkOrderTaskStatusType.WorkFinished)
        {
            return "Finished";
        }
        else
        {
            return "Error";
        }
    }

    public byte[] GenerateWorkOrdersReport(List<WorkOrder> workOrderList,
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
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);

                    page.Header().Text(text =>
                    {
                        text.AlignCenter();
                        text.Span("Work Orders Report").FontSize(30).Bold();
                        text.EmptyLine();
                        text.EmptyLine();
                        text.Span($"{startDate.Date:MMMM dd, yyyy} - {endDate.Date:MMMM dd, yyyy}").FontSize(20).Bold();
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        col.Item().Text($"Number of work orders: {workOrderList.Count}");

                        col.Item().PaddingTop(40).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("ID");
                                header.Cell().Element(CellStyle).Text("Vehicle Plate");
                                header.Cell().Element(CellStyle).Text("Start Date");
                                header.Cell().Element(CellStyle).Text("Status");

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
                            foreach (WorkOrder item in workOrderList)
                            {
                                table.Cell().Element(CellStyle).Text(item.Id.ToString());
                                table.Cell().Element(CellStyle).Text(item.VehiclePlate);
                                table.Cell().Element(CellStyle).Text($"{item.StartDate:MMMM dd, yyyy}");
                                table.Cell().Element(CellStyle).Text(ConvertWorkOrderStatus(item.Status));

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

    public static string ConvertWorkOrderStatus(WorkOrderStatusType enumStatus)
    {
        if (enumStatus == WorkOrderStatusType.Created)
        {
            return "To do";
        }
        else if (enumStatus == WorkOrderStatusType.WorkStarted)
        {
            return "Started";
        }
        else if (enumStatus == WorkOrderStatusType.WorkFinished)
        {
            return "Finished";
        }
        else if (enumStatus == WorkOrderStatusType.Closed)
        {
            return "Closed";
        }
        else
        {
            return "Error";
        }
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
                        text.Span($"{startDate.Date:MMMM dd, yyyy}").FontSize(25).Bold();
                        text.EmptyLine();
                        text.Span("and").FontSize(25).Bold();
                        text.EmptyLine();
                        text.Span($"{endDate.Date:MMMM dd, yyyy}").FontSize(25).Bold();
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
                    totalActualDays += task.HoursForCompletion;

                    tempTaskListData = new EfficiencyReportTaskList();
                    tempTaskListData.TaskTitle = task.Title;
                    tempTaskListData.ActualDays = task.HoursForCompletion;

                    foreach (WorkOrderTaskTemplate template in templateList)
                    {
                        if (task.Title == template.TaskTitle)
                        {
                            totalTheoreticalDays += template.ExpectedHoursForCompletion;
                            tempTaskListData.ExpectedDays = template.ExpectedHoursForCompletion;
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
                        text.Span($"{startDate.Date:MMMM dd, yyyy} - {endDate.Date:MMMM dd, yyyy}").FontSize(20).Bold();
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        col.Item().PaddingBottom(10).Text($"The total expected time for tasks is {totalTheoreticalDays}");
                        col.Item().PaddingBottom(10).Text($"The total actual time for tasks is {totalActualDays}");
                        col.Item().Text($"The efficiency is {Math.Round(((float)totalTheoreticalDays / (float)totalActualDays) * 100, 2)}%");

                        foreach (EfficiencyReportData item in efficiencyList)
                        {
                            col.Item().PaddingTop(40);
                            if (item.WorkOrderDate != null)
                            {
                                col.Item().PaddingBottom(15).Text($"Tasks for: {item.VehiclePlate}                        Date: {item.WorkOrderDate:MMMM dd, yyyy}").Bold();
                            }
                            else
                            {
                                col.Item().PaddingBottom(15).Text($"Tasks for: {item.VehiclePlate}").Bold();
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

    public byte[] GenerateCustomerHistoryReport(List<WorkOrder> workOrderList, string customerName)
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
                        text.Span($"There are no work orders for customer").FontSize(25).Bold();
                        text.EmptyLine();
                        text.EmptyLine();
                        text.Span(customerName).FontSize(30).Bold();
                    });
                });
            });
            return document.GeneratePdf();
        }
        else
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);

                    page.Header().Text(text =>
                    {
                        text.AlignCenter();
                        text.Span("Customer History Report").FontSize(30).Bold();
                        text.EmptyLine();
                        text.EmptyLine();
                        text.Span(customerName).FontSize(25).Bold();
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        col.Item().Text($"Number of work orders: {workOrderList.Count}");

                        col.Item().PaddingTop(40).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(3);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("ID");
                                header.Cell().Element(CellStyle).Text("Vehicle Plate");
                                header.Cell().Element(CellStyle).Text("Model");
                                header.Cell().Element(CellStyle).Text("Date");

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
                            foreach (WorkOrder item in workOrderList)
                            {
                                table.Cell().Element(CellStyle).Text(item.Id.ToString());
                                table.Cell().Element(CellStyle).Text(item.VehiclePlate);
                                table.Cell().Element(CellStyle).Text(item.Details);
                                table.Cell().Element(CellStyle).Text($"{item.StartDate:MMMM dd, yyyy}");

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

    public byte[] GenerateVehicleHistoryReport(List<WorkOrder> workOrderList, string vehiclePlate)
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
                        text.Span($"There are no work orders for vehicle").FontSize(25).Bold();
                        text.EmptyLine();
                        text.EmptyLine();
                        text.Span(vehiclePlate).FontSize(30).Bold();
                    });
                });
            });
            return document.GeneratePdf();
        }
        else
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);

                    page.Header().Text(text =>
                    {
                        text.AlignCenter();
                        text.Span("Vehicle History Report").FontSize(30).Bold();
                        text.EmptyLine();
                        text.EmptyLine();
                        text.Span(vehiclePlate).FontSize(25).Bold();
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        col.Item().Text($"Number of work orders: {workOrderList.Count}");

                        col.Item().PaddingTop(40).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(3);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("ID");
                                header.Cell().Element(CellStyle).Text("Customer");
                                header.Cell().Element(CellStyle).Text("Details");
                                header.Cell().Element(CellStyle).Text("Date");

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
                            foreach (WorkOrder item in workOrderList)
                            {
                                table.Cell().Element(CellStyle).Text(item.Id.ToString());
                                table.Cell().Element(CellStyle).Text(item.CustomerName);
                                table.Cell().Element(CellStyle).Text(item.Details);
                                table.Cell().Element(CellStyle).Text($"{item.StartDate:MMMM dd, yyyy}");

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

    public byte[] GenerateTopTasksReport(List<WorkOrderTask> taskList, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        if (taskList == null || taskList.Count() == 0)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);

                    page.Header().Text(text =>
                    {
                        text.AlignCenter();
                        text.Span("There were no tasks between").FontSize(30).Bold();
                        text.EmptyLine();
                        text.EmptyLine();
                        text.Span($"{startDate.Date:MMMM dd, yyyy}").FontSize(25).Bold();
                        text.EmptyLine();
                        text.Span("and").FontSize(25).Bold();
                        text.EmptyLine();
                        text.Span($"{endDate.Date:MMMM dd, yyyy}").FontSize(25).Bold();
                    });
                });
            });
            return document.GeneratePdf();
        }
        else
        {
            List<TaskReportData> taskReportData = new List<TaskReportData>();
            TaskReportData tempTaskData = new TaskReportData();

            foreach (WorkOrderTask task in taskList)
            {
                if (!taskReportData.Any(t => t.TaskTitle == task.Title))
                {
                    tempTaskData.TaskTitle = task.Title;
                    tempTaskData.Count = 1;
                    taskReportData.Add(tempTaskData);
                    tempTaskData = new TaskReportData();
                }
                else
                {
                    taskReportData.Find(t => t.TaskTitle == task.Title).Count++;
                }
            }

            taskReportData.Sort((t1, t2) => t1.Count.CompareTo(t2.Count));
            taskReportData.Reverse();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);

                    page.Header().Text(text =>
                    {
                        text.AlignCenter();
                        text.Span("Popular Tasks Report").FontSize(30).Bold();
                        text.EmptyLine();
                        text.EmptyLine();
                        text.Span($"{startDate.Date:MMMM dd, yyyy} - {endDate.Date:MMMM dd, yyyy}").FontSize(20).Bold();
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
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
                                header.Cell().Element(CellStyle).Text("Count");

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
                            foreach (var task in taskReportData)
                            {
                                table.Cell().Element(CellStyle).Text(task.TaskTitle);
                                table.Cell().Element(CellStyle).Text(task.Count.ToString());

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

    public byte[] GeneratePartsUsageExpenseReport(List<WorkOrderPart> partList, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        if (partList == null || partList.Count() == 0)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);

                    page.Header().Text(text =>
                    {
                        text.AlignCenter();
                        text.Span("No parts ordered between").FontSize(30).Bold();
                        text.EmptyLine();
                        text.EmptyLine();
                        text.Span($"{startDate.Date:MMMM dd, yyyy}").FontSize(25).Bold();
                        text.EmptyLine();
                        text.Span("and").FontSize(25).Bold();
                        text.EmptyLine();
                        text.Span($"{endDate.Date:MMMM dd, yyyy}").FontSize(25).Bold();
                    });
                });
            });
            return document.GeneratePdf();
        }
        else
        {
            List<PartReportData> partReportData = new List<PartReportData>();
            PartReportData tempPartData = new PartReportData();

            foreach (WorkOrderPart part in partList)
            {
                if (!partReportData.Any(p => p.PartName == part.PartName))
                {
                    tempPartData.PartName = part.PartName;
                    tempPartData.Count = 1;
                    tempPartData.Cost = part.CostCents;
                    partReportData.Add(tempPartData);
                    tempPartData = new PartReportData();
                }
                else
                {
                    partReportData.Find(t => t.PartName == part.PartName).Count++;
                    partReportData.Find(t => t.PartName == part.PartName).Cost+=part.CostCents;
                }
            }

            partReportData.Sort((p1, p2) => p1.Count.CompareTo(p2.Count));
            partReportData.Reverse();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);

                    page.Header().Text(text =>
                    {
                        text.AlignCenter();
                        text.Span("Parts Usage and Expense Report").FontSize(30).Bold();
                        text.EmptyLine();
                        text.EmptyLine();
                        text.Span($"{startDate.Date:MMMM dd, yyyy} - {endDate.Date:MMMM dd, yyyy}").FontSize(20).Bold();
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Part");
                                header.Cell().Element(CellStyle).Text("Count");
                                header.Cell().Element(CellStyle).Text("Cost");
                                header.Cell().Element(CellStyle).Text("Cost Per Unit");

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
                            foreach (var part in partReportData)
                            {
                                table.Cell().Element(CellStyle).Text(part.PartName);
                                table.Cell().Element(CellStyle).Text(part.Count.ToString());
                                table.Cell().Element(CellStyle).Text($"{((float)part.Cost/100).ToString("F2")}");
                                table.Cell().Element(CellStyle).Text((((float)part.Cost / 100)/part.Count).ToString("F2"));

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

    public byte[] GenerateRevenueReport(List<WorkOrder> workOrderList, DateTimeOffset startDate, DateTimeOffset endDate)
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
                        text.Span("No revenue between").FontSize(30).Bold();
                        text.EmptyLine();
                        text.EmptyLine();
                        text.Span($"{startDate.Date:MMMM dd, yyyy}").FontSize(25).Bold();
                        text.EmptyLine();
                        text.Span("and").FontSize(25).Bold();
                        text.EmptyLine();
                        text.Span($"{endDate.Date:MMMM dd, yyyy}").FontSize(25).Bold();
                    });
                });
            });
            return document.GeneratePdf();
        }
        else
        {
            float totalRevenue = 0;

            foreach (WorkOrder workOrder in workOrderList)
            {
                totalRevenue += workOrder.TotalCostCents;
            }

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);

                    page.Header().Text(text =>
                    {
                        text.AlignCenter();
                        text.Span("Revenue Report").FontSize(30).Bold();
                        text.EmptyLine();
                        text.EmptyLine();
                        text.Span($"{startDate.Date:MMMM dd, yyyy} - {endDate.Date:MMMM dd, yyyy}").FontSize(20).Bold();
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        col.Item().Text($"Total Revenue: ${(totalRevenue/100).ToString("F2")}");

                        col.Item().PaddingTop(40).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(3);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Work Order ID");
                                header.Cell().Element(CellStyle).Text("Date");
                                header.Cell().Element(CellStyle).Text("Revenue $");

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
                            foreach (WorkOrder item in workOrderList)
                            {
                                table.Cell().Element(CellStyle).Text(item.Id.ToString());
                                table.Cell().Element(CellStyle).Text($"{item.StartDate:MMMM dd, yyyy}");
                                table.Cell().Element(CellStyle).Text((item.TotalCostCents / 100).ToString("F2"));

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

    public byte[] GenerateMechanicProductivityReport(List<WorkOrderLogEvent> workOrderLogList, DateTimeOffset startDate, DateTimeOffset endDate, string mechName)
    {
        if (workOrderLogList == null || workOrderLogList.Count() == 0)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);

                    page.Header().Text(text =>
                    {
                        text.AlignCenter();
                        text.Span("No work orders completed by").FontSize(30).Bold();
                        text.EmptyLine();
                        text.EmptyLine();
                        text.Span(mechName).FontSize(25).Bold();
                        text.EmptyLine();
                        text.Span("between").FontSize(25).Bold();
                        text.EmptyLine();
                        text.Span($"{startDate.Date:MMMM dd, yyyy}").FontSize(25).Bold();
                        text.EmptyLine();
                        text.Span("and").FontSize(25).Bold();
                        text.EmptyLine();
                        text.Span($"{endDate.Date:MMMM dd, yyyy}").FontSize(25).Bold();
                    });
                });
            });
            return document.GeneratePdf();
        }
        else
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);

                    page.Header().Text(text =>
                    {
                        text.AlignCenter();
                        text.Span("Mechanic Productivity Report").FontSize(30).Bold();
                        text.EmptyLine();
                        text.EmptyLine();
                        text.Span($"{startDate.Date:MMMM dd, yyyy} - {endDate.Date:MMMM dd, yyyy}").FontSize(20).Bold();
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        col.Item().Text($"Work orders completed: {workOrderLogList.Count}");

                        col.Item().PaddingTop(40).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(3);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("WorkOrder ID");
                                header.Cell().Element(CellStyle).Text("Date");
                                header.Cell().Element(CellStyle).Text("Work Type");

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
                            foreach (WorkOrderLogEvent item in workOrderLogList)
                            {
                                table.Cell().Element(CellStyle).Text(item.workOrder.Id.ToString());
                                table.Cell().Element(CellStyle).Text($"{item.DateTime:MMMM dd, yyyy}");
                                table.Cell().Element(CellStyle).Text(item.EventType.ToString());

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

public class TaskReportData()
{
    public string TaskTitle { get; set; } = "";
    public int Count { get; set; }
}

public class PartReportData()
{
    public string PartName { get; set; } = "";
    public int Count { get; set; }
    public int Cost { get; set; }
}