namespace Bork.Azure.TableStorage.Entities
{
    using System;
    using Microsoft.Azure.Cosmos.Table;

    public class BorkedCosmosEntity : TableEntity
    {
        public BorkedCosmosEntity()
        {
            this.PartitionKey = Guid.NewGuid().ToString();
            this.RowKey = Guid.NewGuid().ToString();
        }

        public string Name { get; set; }

        public DateTime BirthDate { get; set; }

        public double NaN { get; set; }
    }
}