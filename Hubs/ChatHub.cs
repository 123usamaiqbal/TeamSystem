using Microsoft.AspNetCore.SignalR;
using TeamManageSystem.Data;
using TeamManageSystem.Models.Account;

namespace TeamManageSystem.Hubs
{
    public class ChatHub : Hub
    {
        private readonly TeamManageContext _dbContext;

        public ChatHub(TeamManageContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async System.Threading.Tasks.Task SendMessage(string user, string message)
        {
            var chatMessage = new ChatMessage
            {
                Sendername = user,
                TextMessage = message,
                Timestamp = DateTime.UtcNow
            };

            _dbContext.ChatMessages.Add(chatMessage);
            await _dbContext.SaveChangesAsync();

            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        public async override System.Threading.Tasks.Task OnConnectedAsync()
        {
            var previousMessages = _dbContext.ChatMessages.OrderBy(m => m.Timestamp).Take(50).ToList();

            foreach (var message in previousMessages)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", message.Sendername, message.TextMessage);
            }

            await base.OnConnectedAsync();
        }
    }
}
