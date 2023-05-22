using PayOut_Aulac_FPT.DTO.ConnectPayment;

namespace PayOut_Aulac_FPT.Hub
{
    public interface ISignalRService
    {
        Task StartConnect(object? strConnect);
        Task SendMessageToRoom(string? roomName, object? strResult);
        Task JoinRoom(string roomName);
        Task LeaveRoom(string roomName);
    }
}
