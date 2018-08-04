namespace CN_Presentation.ViewModel.Controls
{
    public class RateChangedSubsriber
    {
        public delegate void RateChangedHandler(int selectValue);

        public event RateChangedHandler RateChangedEvent;

        public RateChangedSubsriber(RateChangedHandler handler)
        {
            RateChangedEvent += handler;
        }

        public virtual void OnRateChangedEvent(int selectValue)
        {
            RateChangedEvent?.Invoke(selectValue);
        }
    }
}