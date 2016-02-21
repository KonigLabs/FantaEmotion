using System;

namespace KonigLabs.FantaEmotion.SDKData.Events
{
    public abstract class CameraEventBase : EventArgs
    {
        public abstract string Message { get; }

        public abstract CameraEventType EventType { get; }
    }
}
