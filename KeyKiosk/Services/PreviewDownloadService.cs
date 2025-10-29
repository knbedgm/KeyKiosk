using KeyKiosk.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace KeyKiosk.Services;

public class PreviewDownloadService
{
    [Inject] private PDFService PDFService { get; set; }
    private readonly IJSRuntime _jsRuntime;

    public PreviewDownloadService(PDFService pdfService, IJSRuntime jsRuntime)
    {
        PDFService = pdfService;
        _jsRuntime = jsRuntime;
    }

    #region Preview

    public async Task PreviewEfficiencyReportAsync(List<WorkOrder> workOrders, List<WorkOrderTaskTemplate> templates, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var pdfBytes = PDFService.GenerateEfficiencyReport(workOrders, templates, startDate, endDate);

        var base64 = Convert.ToBase64String(pdfBytes);

        // Inject JS function directly here
        var js = @"
            window.openPdfPreview = (base64) => {
                const pdfDataUri = ""data:application/pdf;base64,"" + base64;
                const win = window.open();
                win.document.write(
                    ""<iframe src='"" + pdfDataUri + ""' "" +
                    ""frameborder='0' style='width:100%;height:100%;'></iframe>""
                );
            };
        ";
        await _jsRuntime.InvokeVoidAsync("eval", js);

        // Call JS function to open in new tab
        await _jsRuntime.InvokeVoidAsync("openPdfPreview", base64);
    }

    public async Task PreviewWorkOrdersReportAsync(List<WorkOrder> workOrders, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var pdfBytes = PDFService.GenerateWorkOrdersReport(workOrders, startDate, endDate);

        var base64 = Convert.ToBase64String(pdfBytes);

        // Inject JS function directly here
        var js = @"
            window.openPdfPreview = (base64) => {
                const pdfDataUri = ""data:application/pdf;base64,"" + base64;
                const win = window.open();
                win.document.write(
                    ""<iframe src='"" + pdfDataUri + ""' "" +
                    ""frameborder='0' style='width:100%;height:100%;'></iframe>""
                );
            };
        ";
        await _jsRuntime.InvokeVoidAsync("eval", js);

        // Call JS function to open in new tab
        await _jsRuntime.InvokeVoidAsync("openPdfPreview", base64);
    }

    public async Task PreviewCustomerHistoryReportAsync(List<WorkOrder> workOrders, string customerName)
    {
        var pdfBytes = PDFService.GenerateCustomerHistoryReport(workOrders, customerName);

        var base64 = Convert.ToBase64String(pdfBytes);

        // Inject JS function directly here
        var js = @"
            window.openPdfPreview = (base64) => {
                const pdfDataUri = ""data:application/pdf;base64,"" + base64;
                const win = window.open();
                win.document.write(
                    ""<iframe src='"" + pdfDataUri + ""' "" +
                    ""frameborder='0' style='width:100%;height:100%;'></iframe>""
                );
            };
        ";
        await _jsRuntime.InvokeVoidAsync("eval", js);

        // Call JS function to open in new tab
        await _jsRuntime.InvokeVoidAsync("openPdfPreview", base64);
    }

    public async Task PreviewVehicleHistoryReportAsync(List<WorkOrder> workOrders, string vehicleLicensePlate)
    {
        var pdfBytes = PDFService.GenerateVehicleHistoryReport(workOrders, vehicleLicensePlate);

        var base64 = Convert.ToBase64String(pdfBytes);

        // Inject JS function directly here
        var js = @"
            window.openPdfPreview = (base64) => {
                const pdfDataUri = ""data:application/pdf;base64,"" + base64;
                const win = window.open();
                win.document.write(
                    ""<iframe src='"" + pdfDataUri + ""' "" +
                    ""frameborder='0' style='width:100%;height:100%;'></iframe>""
                );
            };
        ";
        await _jsRuntime.InvokeVoidAsync("eval", js);

        // Call JS function to open in new tab
        await _jsRuntime.InvokeVoidAsync("openPdfPreview", base64);
    }

    public async Task PreviewTopTasksReportAsync(List<WorkOrderTask> tasks, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var pdfBytes = PDFService.GenerateTopTasksReport(tasks, startDate, endDate);

        var base64 = Convert.ToBase64String(pdfBytes);

        // Inject JS function directly here
        var js = @"
            window.openPdfPreview = (base64) => {
                const pdfDataUri = ""data:application/pdf;base64,"" + base64;
                const win = window.open();
                win.document.write(
                    ""<iframe src='"" + pdfDataUri + ""' "" +
                    ""frameborder='0' style='width:100%;height:100%;'></iframe>""
                );
            };
        ";
        await _jsRuntime.InvokeVoidAsync("eval", js);

        // Call JS function to open in new tab
        await _jsRuntime.InvokeVoidAsync("openPdfPreview", base64);
    }

    public async Task PreviewPartsExpenseReportAsync(List<WorkOrderPart> parts, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var pdfBytes = PDFService.GeneratePartsUsageExpenseReport(parts, startDate, endDate);

        var base64 = Convert.ToBase64String(pdfBytes);

        // Inject JS function directly here
        var js = @"
            window.openPdfPreview = (base64) => {
                const pdfDataUri = ""data:application/pdf;base64,"" + base64;
                const win = window.open();
                win.document.write(
                    ""<iframe src='"" + pdfDataUri + ""' "" +
                    ""frameborder='0' style='width:100%;height:100%;'></iframe>""
                );
            };
        ";
        await _jsRuntime.InvokeVoidAsync("eval", js);

        // Call JS function to open in new tab
        await _jsRuntime.InvokeVoidAsync("openPdfPreview", base64);
    }

    public async Task PreviewRevenueReportAsync(List<WorkOrder> workOrders, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var pdfBytes = PDFService.GenerateRevenueReport(workOrders, startDate, endDate);

        var base64 = Convert.ToBase64String(pdfBytes);

        // Inject JS function directly here
        var js = @"
            window.openPdfPreview = (base64) => {
                const pdfDataUri = ""data:application/pdf;base64,"" + base64;
                const win = window.open();
                win.document.write(
                    ""<iframe src='"" + pdfDataUri + ""' "" +
                    ""frameborder='0' style='width:100%;height:100%;'></iframe>""
                );
            };
        ";
        await _jsRuntime.InvokeVoidAsync("eval", js);

        // Call JS function to open in new tab
        await _jsRuntime.InvokeVoidAsync("openPdfPreview", base64);
    }

    public async Task PreviewMechanicTodoAsync(WorkOrder workOrder)
    {
        var pdfBytes = PDFService.GenerateWorkOrderWithTasksPDF(workOrder);

        var base64 = Convert.ToBase64String(pdfBytes);

        var js = @"
            window.openPdfPreview = (base64) => {
                const pdfDataUri = ""data:application/pdf;base64,"" + base64;
                const win = window.open();
                win.document.write(
                    ""<iframe src='"" + pdfDataUri + ""' "" +
                    ""frameborder='0' style='width:100%;height:100%;'></iframe>""
                );
            };
        ";
        await _jsRuntime.InvokeVoidAsync("eval", js);

        await _jsRuntime.InvokeVoidAsync("openPdfPreview", base64);
    }

    public async Task PreviewMechanicProductivityReportAsync(List<WorkOrderLogEvent> workOrderLogs, DateTimeOffset startDate, DateTimeOffset endDate, string mechName)
    {
        var pdfBytes = PDFService.GenerateMechanicProductivityReport(workOrderLogs, startDate, endDate, mechName);

        var base64 = Convert.ToBase64String(pdfBytes);

        // Inject JS function directly here
        var js = @"
            window.openPdfPreview = (base64) => {
                const pdfDataUri = ""data:application/pdf;base64,"" + base64;
                const win = window.open();
                win.document.write(
                    ""<iframe src='"" + pdfDataUri + ""' "" +
                    ""frameborder='0' style='width:100%;height:100%;'></iframe>""
                );
            };
        ";
        await _jsRuntime.InvokeVoidAsync("eval", js);

        // Call JS function to open in new tab
        await _jsRuntime.InvokeVoidAsync("openPdfPreview", base64);
    }


    #endregion



    #region Download

    public async Task DownloadEfficiencyReportAsync(List<WorkOrder> workOrders, List<WorkOrderTaskTemplate> templates, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var pdfBytes = PDFService.GenerateEfficiencyReport(workOrders, templates, startDate, endDate);

        var base64 = Convert.ToBase64String(pdfBytes);

        var js = @"
            window.downloadFileFromBytes = (filename, base64) => {
                const link = document.createElement('a');
                link.href = 'data:application/pdf;base64,' + base64;
                link.download = filename;
                link.click();
            };
        ";
        await _jsRuntime.InvokeVoidAsync("eval", js);

        await _jsRuntime.InvokeVoidAsync("downloadFileFromBytes", "report.pdf", base64);
    }

    public async Task DownloadWorkOrdersReportAsync(List<WorkOrder> workOrders, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var pdfBytes = PDFService.GenerateWorkOrdersReport(workOrders, startDate, endDate);

        var base64 = Convert.ToBase64String(pdfBytes);

        var js = @"
            window.downloadFileFromBytes = (filename, base64) => {
                const link = document.createElement('a');
                link.href = 'data:application/pdf;base64,' + base64;
                link.download = filename;
                link.click();
            };
        ";
        await _jsRuntime.InvokeVoidAsync("eval", js);

        await _jsRuntime.InvokeVoidAsync("downloadFileFromBytes", "report.pdf", base64);
    }

    public async Task DownloadCustomerHistoryReportAsync(List<WorkOrder> workOrders, string customerName)
    {
        var pdfBytes = PDFService.GenerateCustomerHistoryReport(workOrders, customerName);

        var base64 = Convert.ToBase64String(pdfBytes);

        var js = @"
            window.downloadFileFromBytes = (filename, base64) => {
                const link = document.createElement('a');
                link.href = 'data:application/pdf;base64,' + base64;
                link.download = filename;
                link.click();
            };
        ";
        await _jsRuntime.InvokeVoidAsync("eval", js);

        await _jsRuntime.InvokeVoidAsync("downloadFileFromBytes", "report.pdf", base64);
    }

    public async Task DownloadVehicleHistoryReportAsync(List<WorkOrder> workOrders, string vehicleLicensePlate)
    {
        var pdfBytes = PDFService.GenerateVehicleHistoryReport(workOrders, vehicleLicensePlate);

        var base64 = Convert.ToBase64String(pdfBytes);

        var js = @"
            window.downloadFileFromBytes = (filename, base64) => {
                const link = document.createElement('a');
                link.href = 'data:application/pdf;base64,' + base64;
                link.download = filename;
                link.click();
            };
        ";
        await _jsRuntime.InvokeVoidAsync("eval", js);

        await _jsRuntime.InvokeVoidAsync("downloadFileFromBytes", "report.pdf", base64);
    }

    public async Task DownloadTopTasksReportAsync(List<WorkOrderTask> tasks, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var pdfBytes = PDFService.GenerateTopTasksReport(tasks, startDate, endDate);

        var base64 = Convert.ToBase64String(pdfBytes);

        var js = @"
            window.downloadFileFromBytes = (filename, base64) => {
                const link = document.createElement('a');
                link.href = 'data:application/pdf;base64,' + base64;
                link.download = filename;
                link.click();
            };
        ";
        await _jsRuntime.InvokeVoidAsync("eval", js);

        await _jsRuntime.InvokeVoidAsync("downloadFileFromBytes", "report.pdf", base64);
    }

    public async Task DownloadPartsExpenseReportAsync(List<WorkOrderPart> parts, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var pdfBytes = PDFService.GeneratePartsUsageExpenseReport(parts, startDate, endDate);

        var base64 = Convert.ToBase64String(pdfBytes);

        var js = @"
            window.downloadFileFromBytes = (filename, base64) => {
                const link = document.createElement('a');
                link.href = 'data:application/pdf;base64,' + base64;
                link.download = filename;
                link.click();
            };
        ";
        await _jsRuntime.InvokeVoidAsync("eval", js);

        await _jsRuntime.InvokeVoidAsync("downloadFileFromBytes", "report.pdf", base64);
    }

    public async Task DownloadRevenueReportAsync(List<WorkOrder> workOrders, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var pdfBytes = PDFService.GenerateRevenueReport(workOrders, startDate, endDate);

        var base64 = Convert.ToBase64String(pdfBytes);

        var js = @"
            window.downloadFileFromBytes = (filename, base64) => {
                const link = document.createElement('a');
                link.href = 'data:application/pdf;base64,' + base64;
                link.download = filename;
                link.click();
            };
        ";
        await _jsRuntime.InvokeVoidAsync("eval", js);

        await _jsRuntime.InvokeVoidAsync("downloadFileFromBytes", "report.pdf", base64);
    }

    public async Task DownloadMechanicTodoAsync(WorkOrder workOrder)
    {
        var pdfBytes = PDFService.GenerateWorkOrderWithTasksPDF(workOrder);

        var base64 = Convert.ToBase64String(pdfBytes);

        var js = @"
            window.downloadFileFromBytes = (filename, base64) => {
                const link = document.createElement('a');
                link.href = 'data:application/pdf;base64,' + base64;
                link.download = filename;
                link.click();
            };
        ";
        await _jsRuntime.InvokeVoidAsync("eval", js);

        await _jsRuntime.InvokeVoidAsync("downloadFileFromBytes", "report.pdf", base64);
    }

    public async Task DownloadMechanicProductivityReportAsync(List<WorkOrderLogEvent> workOrderLogs, DateTimeOffset startDate, DateTimeOffset endDate, string mechName)
    {
        var pdfBytes = PDFService.GenerateMechanicProductivityReport(workOrderLogs, startDate, endDate, mechName);

        var base64 = Convert.ToBase64String(pdfBytes);

        var js = @"
            window.downloadFileFromBytes = (filename, base64) => {
                const link = document.createElement('a');
                link.href = 'data:application/pdf;base64,' + base64;
                link.download = filename;
                link.click();
            };
        ";
        await _jsRuntime.InvokeVoidAsync("eval", js);

        await _jsRuntime.InvokeVoidAsync("downloadFileFromBytes", "report.pdf", base64);
    }

    #endregion
}
