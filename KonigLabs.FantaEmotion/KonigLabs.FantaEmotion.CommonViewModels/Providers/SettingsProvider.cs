using System.IO;
using System.Xml.Serialization;
using KonigLabs.FantaEmotion.CommonViewModels.ViewModels.Settings;
using KonigLabs.FantaEmotion.SDKData.Enums;

namespace KonigLabs.FantaEmotion.CommonViewModels.Providers
{
    public class SettingsProvider
    {
        

        public virtual CameraSettingsDto GetCameraSettings()
        {
            CameraSettingsDto settings = null;
            if (File.Exists("CameraPhotoSettings.xml"))
            {
                using (var fs = File.OpenRead("CameraPhotoSettings.xml"))
                {
                    settings = (CameraSettingsDto)new XmlSerializer(typeof(CameraSettingsDto)).Deserialize(fs);
                }
            }
            else
            {
                settings = new CameraSettingsDto
                {
                    SelectedAeMode = AEMode.Manual,
                    SelectedAvValue = ApertureValue.AV_8,
                    SelectedIsoSensitivity = CameraISOSensitivity.ISO_400,
                    SelectedWhiteBalance = WhiteBalance.Daylight,
                    SelectedShutterSpeed = ShutterSpeed.TV_200
                };
                using (var fs = File.Open("CameraPhotoSettings.xml", FileMode.OpenOrCreate))
                {
                    new XmlSerializer(typeof(CameraSettingsDto)).Serialize(fs, settings);
                }
            }
            return settings;
        }

    }
}
