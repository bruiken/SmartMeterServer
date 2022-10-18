namespace Abstract.Models
{
    public class Installation
    {
        public int Id { get; set; }

        public string LocationId { get; set; }

        public string Name { get; set; }

        public string RabbitMQUsername { get; set; }

        public string RabbitMQPassword { get; set; }

        public string RabbitMQExchange { get; set; }

        public string RabbitMQVHost { get; set; }

        public IEnumerable<InstallationAccess> InstallationAccesses { get; set; }
    }
}
