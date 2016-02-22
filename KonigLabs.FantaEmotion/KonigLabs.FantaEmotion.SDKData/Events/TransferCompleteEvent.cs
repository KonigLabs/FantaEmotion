using KonigLabs.FantaEmotion.Common.Extensions;
using KonigLabs.FantaEmotion.SDKData.Enums;

namespace KonigLabs.FantaEmotion.SDKData.Events
{
    public class TransferCompleteEvent : CameraEventBase
    {
        public TransferCompleteEvent(ReturnValue returnValue = ReturnValue.Ok)
        {
            ErrorCode = returnValue;
        }

        public ReturnValue ErrorCode { get; private set; }

        public override string Message
        {
            get
            {
                return ErrorCode == ReturnValue.Ok
                    ? "Файл успешно сохранён на компьютере."
                    : $"При копировании файла возникла ошибка: {ErrorCode.GetDescription()}";
            }
        }

        public override CameraEventType EventType
        {
            get { return CameraEventType.Transfer; }
        }
    }
}
