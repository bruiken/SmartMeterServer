namespace Rotom.Models
{
    public class CreateInstallationModel : ErrorViewModel
    {
        public int Id { get; set; }

        public string LocationId { get; set; }

        public string Name { get; set; }

        public string RabbitMQUsername { get; set; }

        public string RabbitMQPassword { get; set; }

        public string RabbitMQExchange { get; set; }

        public string RabbitMQVHost { get; set; }

        public List<CreateInstallationAccessModel> InstallationAccesses { get; set; }
    }

    public class CreateInstallationAccessModel
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        public bool Selected { get; set; }
    }
}
