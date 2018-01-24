using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.AzureStorage.Tables.Paging;
using Lykke.JobTriggers.Extenstions;
using Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Addresses;
using Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Balances;
using Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Transactions;
using Lykke.Service.RaiblocksApi.AzureRepositories.Repositories.Addresses;
using Lykke.Service.RaiblocksApi.AzureRepositories.Repositories.Balances;
using Lykke.Service.RaiblocksApi.AzureRepositories.Repositories.Transactions;
using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Balances;
using Lykke.Service.RaiblocksApi.Core.Repositories.Addresses;
using Lykke.Service.RaiblocksApi.Core.Repositories.Balances;
using Lykke.Service.RaiblocksApi.Core.Repositories.Transactions;
using Lykke.Service.RaiblocksApi.Core.Services;
using Lykke.Service.RaiblocksApi.Core.Settings.ServiceSettings;
using Lykke.Service.RaiblocksApi.Services;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.RaiblocksApi.Modules
{
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<RaiblocksApiSettings> _settings;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public ServiceModule(IReloadingManager<RaiblocksApiSettings> settings, ILog log)
        {
            _settings = settings;
            _log = log;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            // TODO: Do not register entire settings in container, pass necessary settings to services which requires them
            // ex:
            //  builder.RegisterType<QuotesPublisher>()
            //      .As<IQuotesPublisher>()
            //      .WithParameter(TypedParameter.From(_settings.CurrentValue.QuotesPublication))

            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();


            // Repositories setup
            builder.RegisterType<BalanceObservationRepository>()
                .As<IBalanceObservationRepository<BalanceObservation>>()
                .WithParameter(TypedParameter.From(_settings.Nested(s => s.Db.DataConnString)));

            builder.RegisterType<AddressBalanceRepository>()
                .As<IAddressBalanceRepository<AddressBalance>>()
                .WithParameter(TypedParameter.From(_settings.Nested(s => s.Db.DataConnString)));


            builder.RegisterType<AddressHistoryEntryRepository>()
                .As<IAddressHistoryEntryRepository<AddressHistoryEntry>>()
                .WithParameter(TypedParameter.From(_settings.Nested(s => s.Db.DataConnString)));

            builder.RegisterType<AddressObservationRepository>()
                .As<IAddressObservationRepository<AddressObservation>>()
                .WithParameter(TypedParameter.From(_settings.Nested(s => s.Db.DataConnString)));

            builder.RegisterType<TransactionBodyRepository>()
                .As<ITransactionBodyRepository<TransactionBody>>()
                .WithParameter(TypedParameter.From(_settings.Nested(s => s.Db.DataConnString)));

            builder.RegisterType<TransactionMetaRepository>()
                .As<ITransactionMetaRepository<TransactionMeta>>()
                .WithParameter(TypedParameter.From(_settings.Nested(s => s.Db.DataConnString)));

            builder.RegisterType<TransactionObservationRepository>()
                .As<ITransactionObservationRepository<TransactionObservation>>()
                .WithParameter(TypedParameter.From(_settings.Nested(s => s.Db.DataConnString)));


            // Services setup
            builder.RegisterType<BalanceService<BalanceObservation, AddressBalance>>()
                .As<IBalanceService<BalanceObservation, AddressBalance>>();

            builder.RegisterType<HistoryService<AddressHistoryEntry, AddressObservation>>()
                .As<IHistoryService<AddressHistoryEntry, AddressObservation>>();

            builder.RegisterType<TransactionService<TransactionBody, TransactionMeta, TransactionObservation>>()
                .As<ITransactionService<TransactionBody, TransactionMeta, TransactionObservation>>();

            builder.RegisterType<AssetService>()
                .As<IAssetService>();

            builder.RegisterType<RaiBlockchainService<AddressBalance, BalanceObservation>>()
                .As<IBlockchainService<AddressBalance, BalanceObservation>>()
                .WithParameter("privateNodeURL", _settings.Nested(s => s.nodeAPI.PrivateNodeURL).CurrentValue)
                .WithParameter("publicNodeURL", _settings.Nested(s => s.nodeAPI.PublicNodeURL).CurrentValue);

            // TODO: Add your dependencies here

            builder.AddTriggers(pool =>
            {
                pool.AddDefaultConnection(_settings.Nested(x => x.Db.DataConnString));
            });

            builder.Populate(_services);
        }
    }
}
