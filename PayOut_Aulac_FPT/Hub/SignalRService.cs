using Microsoft.AspNetCore.SignalR;
using PayOut_Aulac_FPT.DTO.ConnectPayment;

namespace PayOut_Aulac_FPT.Hub
{
    public class SignalRService : Hub<ISignalRService>
    {
        public async Task StartConnect(QRCodeCreateResponse? strConnect)
        {
            await Clients.All.StartConnect(strConnect);
        }
    }
}
