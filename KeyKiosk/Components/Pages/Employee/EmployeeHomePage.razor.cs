using System.Xml.Linq;
using KeyKiosk.Data;
using KeyKiosk.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace KeyKiosk.Components.Pages.Employee;

public partial class EmployeeHomePage
{
    private List<WorkOrder>? workOrders;
    private WorkOrder? editWorkOrder;

    [Inject] private ReportService ReportService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;


    protected override async Task OnInitializedAsync()
    {
        workOrders = await Db.WorkOrders.Include(w => w.Tasks).ToListAsync();
    }

    private async Task GeneratePdfAsync(WorkOrder workOrder)
    {
        var pdfBytes = ReportService.GenerateReport(workOrder);

        // Download the PDF in the browser
        var base64 = Convert.ToBase64String(pdfBytes);

        // Inject JS function directly here
        var js = @"
            window.downloadFileFromBytes = (filename, base64) => {
                const link = document.createElement('a');
                link.href = 'data:application/pdf;base64,' + base64;
                link.download = filename;
                link.click();
            };
        ";
        await JSRuntime.InvokeVoidAsync("eval", js);

        await JSRuntime.InvokeVoidAsync("downloadFileFromBytes", "report.pdf", base64);
    }

    private async Task PreviewPdfAsync(WorkOrder workOrder)
    {
        var pdfBytes = ReportService.GenerateReport(workOrder);

        // Convert PDF bytes to base64 string
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
        await JSRuntime.InvokeVoidAsync("eval", js);

        // Call JS function to open in new tab
        await JSRuntime.InvokeVoidAsync("openPdfPreview", base64);
    }

    private void NewWorkOrder()
    {
        editWorkOrder = new WorkOrder
        {
            CustomerName = "",
            Details = "",
            VehiclePlate = "",
            StartDate = DateTimeOffset.Now,
            EndDate = DateTimeOffset.Now.AddDays(1),
            Status = WorkOrderStatusType.Created,
            Tasks = new List<WorkOrderTask>()
        };
    }

    private async Task EditWorkOrder(int id)
    {
        editWorkOrder = await Db.WorkOrders
            .Include(w => w.Tasks)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    private async Task SaveWorkOrder()
    {
        if (editWorkOrder!.Id == 0)
        {
            Db.WorkOrders.Add(editWorkOrder);
        }
        else
        {
            Db.WorkOrders.Update(editWorkOrder);
        }

        await Db.SaveChangesAsync();
        workOrders = await Db.WorkOrders.Include(w => w.Tasks).ToListAsync();
        editWorkOrder = null;
    }

    private async Task DeleteWorkOrder(int id)
    {
        var wo = await Db.WorkOrders.FindAsync(id);
        if (wo != null)
        {
            Db.WorkOrders.Remove(wo);
            await Db.SaveChangesAsync();
            workOrders = await Db.WorkOrders.Include(w => w.Tasks).ToListAsync();
        }
    }

    private void CancelEdit()
    {
        editWorkOrder = null;
    }
}
