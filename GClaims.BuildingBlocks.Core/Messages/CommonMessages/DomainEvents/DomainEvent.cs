namespace GClaims.BuildingBlocks.Core.Messages.CommonMessages.DomainEvents
{
    /// <summary>
    /// É um tipo especifica de evento que serve pra notificar um contexto delimitado (handler) pra falar que alguma coisa aconteceu,
    /// que houve uma mudança de estado no servidor;
    /// </summary>
    public abstract class DomainEvent : Message, IDomainEvent
    {
        public DateTime Timestamp { get; private set; }

        protected DomainEvent(Guid aggregateId)
        {
            AggregateId = aggregateId;
            Timestamp = DateTime.Now;
        }
    }
}