namespace KeyKiosk.Services
{
    public interface IPhysicalDrawerController
    {
        abstract Task Open(int id);

        abstract Task OpenAll();
    }
}