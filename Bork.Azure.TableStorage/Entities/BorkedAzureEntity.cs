namespace Bork.Azure.TableStorage.Entities
{
    using System;
    using Microsoft.WindowsAzure.Storage.Table;

    public class BorkedAzureEntity : TableEntity
    {
        public BorkedAzureEntity()
        {
            this.PartitionKey = Guid.NewGuid().ToString();
            this.RowKey = Guid.NewGuid().ToString();
        }

        public string Name { get; set; }

        public DateTime BirthDate { get; set; }

        public double NaN { get; set; }
    }
}