namespace GClaims.BuildingBlocks.Core.Messages.CommonMessages.IntegrationEvents
{
    /// <summary>
    /// 
    ///   É um evento utiizado para notificar e interagir com as ações relacionadas as regras de negoicio
    ///
    ///   Ex: Após inserir um Pacinete na base que esta em um modulo "X", eu preciso notificar a a secretaria e está em um modulo "Y". 
    ///
    ///   Esse evento é usado para integrar e comunicar entre modulos; 
    ///
    /// </summary>
    public abstract class IntegrationEvent : Event, IIntegrationEvent
    {
    }
}