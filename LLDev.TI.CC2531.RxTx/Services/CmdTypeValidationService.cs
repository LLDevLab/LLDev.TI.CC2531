using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Services;

internal interface ICmdTypeValidationService
{
    bool IsResponseOrCallback(ZToolCmdType cmdType);
}

internal sealed class CmdTypeValidationService : ICmdTypeValidationService
{
    public bool IsResponseOrCallback(ZToolCmdType cmdType) => (ushort)cmdType > (ushort)ZToolCmdType.SysResetReq;
}
