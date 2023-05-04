using Microsoft.AspNetCore.SignalR;
using PayOut_Aulac_FPT.DTO.ConnectPayment;

namespace PayOut_Aulac_FPT.Hub
{
    public class SignalRService : Hub<ISignalRService>
    {
        public async Task StartConnect(object? strConnect)
        {
            await Clients.All.StartConnect(strConnect);
        }

        public async Task SendRoom(string roomName, object? strResult)
        {
            await Clients.Group(roomName).SendRoom(roomName, strResult);
        }

        //public async Task JoinRoom(string roomName, ResultInfo? strResult)
        //{
        //    await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        //    await Clients.Group(roomName).SendRoom(roomName, strResult);
        //}

        //public async Task LeaveRoom(string roomName)
        //{
        //    await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        //    await Clients.Group(roomName).SendAsync("LeaveRoom", $"{Context.ConnectionId} has left the room {roomName}.");
        //}

        //public async Task SendMessage(string roomName, string message)
        //{
        //    await Clients.Group(roomName).SendAsync("ReceiveMessage", $"{Context.ConnectionId}: {message}");
        //}
    }
}
