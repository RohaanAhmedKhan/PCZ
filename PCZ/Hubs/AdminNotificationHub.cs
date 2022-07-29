using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace PCZ.Hubs {
    public class AdminNotificationHub : Hub {

        public void Hello() {

            Clients.All.hello();

        }

        public static void SendSwalNotification(string type, string title, string message)
        {

            var context = GlobalHost.ConnectionManager.GetHubContext<AdminNotificationHub>();
            context.Clients.All.displayMessage(type, message);

        }
    }
}