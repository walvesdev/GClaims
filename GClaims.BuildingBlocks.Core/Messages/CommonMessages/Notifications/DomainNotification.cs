namespace GClaims.BuildingBlocks.Core.Messages.CommonMessages.Notifications
{
    /// <summary>
    /// As notificações de dominio elas notificam algum problema encontrado atraves da regra de negocio que invalida aquele processo q o usuario esta tenteando faz;
    /// Ela traz um lista de problemas encontrado;
    /// </summary>
    public class DomainNotification : Message, IDomainNotification
    {
        public DateTime Timestamp { get; private set; }
        public Guid DomainNotificationId { get; private set; }
        public string Key { get; private set; }
        public string Value { get; private set; }
        public int Version { get; private set; }

        public DomainNotification(string key, string value)
        {
            Timestamp = DateTime.Now;
            DomainNotificationId = Guid.NewGuid();
            Version = 1;
            Key = key;
            Value = value;
        }
    }
}