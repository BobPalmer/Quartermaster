namespace Quartermaster
{
    public class Ingredient
    {
        public int Quantity { get; set; }
        public string ResourceName { get; set; }

        public Ingredient(string name, int qty)
        {
            ResourceName = name;
            Quantity = qty;
        }

        public Ingredient()
        {
            
        }
    }
}