using System.Text.Json.Serialization;
using FluentValidation.Results;
using MediatR;

namespace GClaims.BuildingBlocks.Core.Messages
{
    public interface IMessage
    {
        public string MessageType { get; }

        public dynamic AggregateId { get; set; }

        public DateTime Timestamp { get; set; }

        public object Data { get; set; }

        [JsonIgnore]
        public ValidationResult ValidationResult { get; set; }
    }

    public interface IMessageValidator
    {
        public Task<bool> Validate(IBaseRequest request);

    }
}