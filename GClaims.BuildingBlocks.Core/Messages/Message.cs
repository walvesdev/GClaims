using FluentValidation.Results;

namespace GClaims.BuildingBlocks.Core.Messages
{
    public abstract class Message : IMessage
    {
        public string MessageType
        {
            get
            {
                return GetType().Name;
            }
        }

        public dynamic AggregateId { get; set; }
        public DateTime Timestamp { get; set; }
        public object Data { get; set; }
        public ValidationResult ValidationResult { get; set; }
    }
}