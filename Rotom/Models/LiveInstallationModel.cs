namespace Rotom.Models
{
    public class LiveInstallationModel
    {
        public int Id { get; set; }

        public string LocationId { get; set; }

        public string Name { get; set; }

        public string RabbitMQUsername { get; set; }

        public string RabbitMQPassword { get; set; }

        public string RabbitMQExchange { get; set; }

        public string RabbitMQVHost { get; set; }

        public string RabbitMQHostname { get; set; }

        public string RabbitMQPort { get; set; }
    }
}
