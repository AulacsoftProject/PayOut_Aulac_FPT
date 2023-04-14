using PayOut_Aulac_FPT.DTO.ConnectPayment;

namespace PayOut_Aulac_FPT.Hub
{
    public interface ISignalRService
    {
        Task StartConnect(QRCodeCreateResponse? strConnect);
    }
}
