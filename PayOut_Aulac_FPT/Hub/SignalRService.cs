using Microsoft.AspNetCore.SignalR;
using PayOut_Aulac_FPT.DTO.ConnectPayment;

namespace PayOut_Aulac_FPT.Hub
{
    public class SignalRService : Microsoft.AspNetCore.SignalR.Hub
    {
        //public async Task StartConnect(object? strConnect)
        //{
        //    await Clients.All.SendAsync(strConnect);
        //}

        public async Task JoinRoom(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        }

        public async Task LeaveRoom(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }

        public async Task SendMessageToRoom(string roomName, object? strResult)
        {
            await Clients.Group(roomName).SendAsync("ReceiveMessage", strResult);
        }
    }
}
