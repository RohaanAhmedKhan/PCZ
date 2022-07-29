using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCZ.ViewModels
{
    public class MessagesViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public System.DateTime RecievedTime { get; set; }
        public bool IsRead { get; set; }

        public string ReplyMessage { get; set; }
    }
}