using Microsoft.AspNetCore.SignalR;
using NotepadOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotepadOnline
{
    public class DisconnectHub : Hub
    {
        public async Task Disconnect(IQueryable<User> users, User user)
        {
            await Clients.All.SendAsync("Send", users, user);
        }

        //public async Task Disconnect(string message)
        //{
        //   await Clients.All.SendAsync("Send", message);
        //}
    }
}
