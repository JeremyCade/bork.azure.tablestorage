namespace Bork.Azure.TableStorage
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Bork.Azure.TableStorage.Entities;
    using Bork.Azure.TableStorage.Services;
    using Microsoft.Extensions.Configuration;


    public class Program
    {
        private const int entitiesToInsertPerProvider = 10;

        private static IConfigurationRoot configuration;

        public static async Task<int> Main(string[] args)
        {
            var directory = Path.GetDirectoryName(typeof(Program).Assembly.Location);

            // Read Configuration
            var builder = new ConfigurationBuilder()
                .SetBasePath(directory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            configuration = builder.Build();

            await InsertWithAzureRetrieveWithCosmos();
            await InsertWithCosmosRetrieveWithAzure();

            return 0;
        }

        public static async Task InsertWithAzureRetrieveWithCosmos()
        {
            var random = new Random();
            string tableName = $"AzureInsert{DateTime.UtcNow.ToString("hhmmss")}";

            await Task.Yield();

            var azure = new BorkedAzureService<BorkedAzureEntity>(configuration.GetConnectionString("StorageConnection"), tableName);
            var cosmos = new BorkedCosmosService<BorkedCosmosEntity>(configuration.GetConnectionString("StorageConnection"), tableName);

            try
            {
                for (var i = 0; i < entitiesToInsertPerProvider; i++)
                {
                    var entity = new BorkedAzureEntity();
                    var months = (random.Next(i) * 5 * -1) / 100;
                    entity.Name = $"Azure {i}";
                    entity.BirthDate = DateTime.UtcNow.AddMonths(months);

                    if (i % 2 == 0)
                    {
                        entity.NaN = random.NextDouble() * 100;
                    }
                    else
                    {
                        entity.NaN = double.NaN;
                    }

                    // OK
                    await azure.InsertAsync(entity);
                }

                var results = await cosmos.PrepareForTheBork();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cosmos BORKED: {ex.Message}");
            }
            finally
            {
                await azure.Cleanup();
            }
        }

        public static async Task InsertWithCosmosRetrieveWithAzure()
        {
            var random = new Random();
            string tableName = $"AzureInsert{DateTime.UtcNow.ToString("hhmmss")}";

            await Task.Yield();

            var azure = new BorkedAzureService<BorkedAzureEntity>(configuration.GetConnectionString("StorageConnection"), tableName);
            var cosmos = new BorkedCosmosService<BorkedCosmosEntity>(configuration.GetConnectionString("StorageConnection"), tableName);

            try
            {
                for (var i = 0; i < entitiesToInsertPerProvider; i++)
                {
                    var entity = new BorkedCosmosEntity();
                    var months = (random.Next(i) * 5 * -1) / 100;
                    entity.Name = $"Azure {i}";
                    entity.BirthDate = DateTime.UtcNow.AddMonths(months);

                    if (i % 2 == 0)
                    {
                        entity.NaN = random.NextDouble() * 100;
                    }
                    else
                    {
                        entity.NaN = double.NaN;
                    }

                    // OK
                    await cosmos.InsertAsync(entity);
                }

                var results = await azure.PrepareForTheBork();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Azure BORKED: {ex.Message}");
            }
            finally
            {
                await cosmos.Cleanup();
            }
        }
    }
}
