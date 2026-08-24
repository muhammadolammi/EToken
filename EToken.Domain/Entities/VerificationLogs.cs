

namespace EToken.Domain.Entities;

public class VerificationLogs(Guid id, Guid deviceId, string cIF, string actionType, string result, string ipAddress, DateTimeOffset createddAt)
{
    public Guid Id { get; set; } = id;
    public Guid DeviceId { get; set; } = deviceId;
    public string CIF { get; set; } = cIF;
    public string ActionType { get; set; } = actionType;
    public string Result { get; set; } = result;
    public string IpAddress { get; set; } = ipAddress;

    public DateTimeOffset CreateddAt { get; set; } = createddAt;

} 