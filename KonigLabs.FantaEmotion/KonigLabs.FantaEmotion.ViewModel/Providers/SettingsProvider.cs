using System.IO;
using System.Xml.Serialization;
using KonigLabs.FantaEmotion.SDKData.Enums;
using KonigLabs.FantaEmotion.ViewModel.Settings;

namespace KonigLabs.FantaEmotion.ViewModel.Providers
{
    public class SettingsProvider
    {
        public CameraSettingsDto GetCameraSettings()
        {
            CameraSettingsDto settings = null;
            if (!File.Exists("CameraPhotoSettings.xml"))
            {
                settings = new CameraSettingsDto
                {
                    //NOT SUPPORTED on EOS 1100D
                    //SelectedAeMode = AEMode.Manual,
                    SelectedAvValue = ApertureValue.AV_8,
                    SelectedIsoSensitivity = CameraISOSensitivity.ISO_400,
                    SelectedWhiteBalance = WhiteBalance.Daylight,
                    SelectedShutterSpeed = ShutterSpeed.TV_200,
                    SelectedCompensation = ExposureCompensation.Zero
                };
                using (var file = File.Create("CameraPhotoSettings.xml"))
                {
                    new XmlSerializer(settings.GetType()).Serialize(file, settings);
                    file.Close();
                }

            }
            else {
                using (var file = File.OpenRead("CameraPhotoSettings.xml"))
                {
                    settings = (CameraSettingsDto)new XmlSerializer(typeof(CameraSettingsDto)).Deserialize(file);
                    file.Close();
                }
            }
            return settings;
        }
    }
}
