using Lykke.Service.RaiblocksApi.Core.Settings.ServiceSettings;
using Lykke.Service.RaiblocksApi.Core.Settings.SlackNotifications;

namespace Lykke.Service.RaiblocksApi.Core.Settings
{
    public class AppSettings
    {
        public RaiblocksApiSettings RaiblocksApiService { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}
