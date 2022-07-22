using FluentValidation.Results;

namespace GClaims.BuildingBlocks.Core.Messages
{
    public abstract class Message : IMessage
    {
        private dynamic _aggregatedI;

        public string MessageType
        {
            get { return GetType().Name; }
        }

        public dynamic AggregateId
        {
            get
            {
                if (_aggregatedI == null)
                {
                    return "No AggregateId";
                }

                return _aggregatedI;
            }
            set { _aggregatedI = value; }
        }

        public DateTime Timestamp { get; set; }

        public object Data { get; set; }

        public ValidationResult ValidationResult { get; set; }

        protected Message()
        {
            ValidationResult = new ValidationResult();
        }
    }
}