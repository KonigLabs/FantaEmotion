namespace KonigLabs.FantaEmotion.PatternProcessing
{
    public class TimerData
    {
        public TimerData(int tick, bool ready = false)
        {
            Tick = tick;
            Ready = ready;
        }

        public int Tick { get; private set; }

        public bool Ready { get; private set; }
    }
}
