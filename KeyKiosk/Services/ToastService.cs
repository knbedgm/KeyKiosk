using System;

namespace KeyKiosk.Services
{
    public class ToastService
    {
        public event Action<string, string>? OnShow;
        public event Action? OnHide;

        public void ShowToast(string message, string type = "info")
        {
            OnShow?.Invoke(message, type);
        }

        public void HideToast()
        {
            OnHide?.Invoke();
        }
    }
}
