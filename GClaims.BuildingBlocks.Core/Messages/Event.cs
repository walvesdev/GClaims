namespace GClaims.BuildingBlocks.Core.Messages
{
    public abstract class Event : Message, IEvent
    {
        public DateTime Timestamp { get; private set; }

        protected Event()
        {
            Timestamp = DateTime.Now;
        }
    }
}