using System;
using KonigLabs.FantaEmotion.SDKData.Enums;

namespace KonigLabs.FantaEmotion.ViewModel.Settings
{
    [Serializable]
    public class CameraSettingsDto
    {
        public ExposureCompensation SelectedCompensation { get; set; }

        public WhiteBalance SelectedWhiteBalance { get; set; }

        public CameraISOSensitivity SelectedIsoSensitivity { get; set; }

        public ShutterSpeed SelectedShutterSpeed { get; set; }

        public ApertureValue SelectedAvValue { get; set; }
    }
}
