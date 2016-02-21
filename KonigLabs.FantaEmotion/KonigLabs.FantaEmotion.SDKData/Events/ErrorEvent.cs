using KonigLabs.FantaEmotion.Common.Extensions;
using KonigLabs.FantaEmotion.SDKData.Enums;

namespace KonigLabs.FantaEmotion.SDKData.Events
{
    public class ErrorEvent : CameraEventBase
    {
        public ErrorEvent(ReturnValue returnValue)
        {
            ErrorCode = returnValue;
        }

        public ReturnValue ErrorCode { get; private set; }
        public override string Message
        {
            get { return string.Format("При работе камеры возникла ошибка: {0}", ErrorCode.GetDescription()); }
        }

        public override CameraEventType EventType
        {
            get { return CameraEventType.Error; }
        }
    }
}
