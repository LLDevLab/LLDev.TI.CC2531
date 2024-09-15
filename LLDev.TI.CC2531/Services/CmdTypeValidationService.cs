using LLDev.TI.CC2531.Enums;

namespace LLDev.TI.CC2531.Services;

internal interface ICmdTypeValidationService
{
    bool IsResponseOrCallback(ZToolCmdType cmdType);
}

internal sealed class CmdTypeValidationService : ICmdTypeValidationService
{
    public bool IsResponseOrCallback(ZToolCmdType cmdType) => (ushort)cmdType > (ushort)ZToolCmdType.SysResetReq;
}
