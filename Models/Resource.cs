namespace MediScope.Models
{
    public class Resource
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";  // ex., "Equipment", "Room", "Supplies"
        public int Quantity { get; set; }
    }
}