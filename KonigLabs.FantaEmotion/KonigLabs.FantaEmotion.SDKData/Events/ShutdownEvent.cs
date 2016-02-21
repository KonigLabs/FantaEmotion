namespace KonigLabs.FantaEmotion.SDKData.Events
{
    public class ShutdownEvent : CameraEventBase
    {
        public override string Message
        {
            get { return "Работа камеры была аварийно завершена"; }
        }

        public override CameraEventType EventType
        {
            get { return CameraEventType.Shutdown; }
        }
    }
}
