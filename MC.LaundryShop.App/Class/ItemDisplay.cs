namespace MC.LaundryShop.App.Class
{
    public class ItemDisplay
    {
        public long Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;

        public override string ToString()
        {
            return Name;
        }
    }
}
