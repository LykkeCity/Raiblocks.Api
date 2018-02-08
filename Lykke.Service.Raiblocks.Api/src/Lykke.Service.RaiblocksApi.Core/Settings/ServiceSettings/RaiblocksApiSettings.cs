namespace Lykke.Service.RaiblocksApi.Core.Settings.ServiceSettings
{
    public class RaiblocksApiSettings
    {
        public DbSettings Db { get; set; }

        public NodeAPISettings nodeAPI { get; set; }
        
        public int CurerntAccuracy { get; set; }
        
        public int MaxAccuracy { get; set; }
    }
}
