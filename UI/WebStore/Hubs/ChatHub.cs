﻿using Microsoft.AspNetCore.SignalR;

namespace WebStore.Hubs;

public class ChatHub : Hub
{
    public async Task SendMessage(string Message) => await Clients.Others.SendAsync("MessageToClient", Message);
}
