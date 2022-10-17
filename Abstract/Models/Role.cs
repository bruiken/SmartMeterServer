namespace Abstract.Models
{
    public class Role
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Permissions Permissions { get; set; }
    }
}
