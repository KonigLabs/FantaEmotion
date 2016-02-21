using System;

namespace KonigLabs.FantaEmotion.CommonViewModels.ViewModels.Settings
{
    public class AppSettingsDto
    {
        public bool ShowPrinterOnStartup { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }

        public string HashTag { get; set; }

        public string PrinterName { get; set; }
    }
}
