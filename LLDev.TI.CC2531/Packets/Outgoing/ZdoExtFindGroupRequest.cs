using LLDev.TI.CC2531.Enums;

namespace LLDev.TI.CC2531.Packets.Outgoing;
internal sealed class ZdoExtFindGroupRequest : OutgoingPacket, IOutgoingPacket
{
    /// <summary>
    /// Endpoint to look for
    /// </summary>
    public byte Endpoint { get; }
    public ushort GroupId { get; }

    protected override byte[] Data { get; }

    public ZdoExtFindGroupRequest(byte endpoint, ushort groupId) :
        base(ZToolCmdType.ZdoExtFindGroupReq, 3)
    {
        Endpoint = endpoint;
        GroupId = groupId;

        Data = [Endpoint, .. BitConverter.GetBytes(GroupId)];
    }
}
