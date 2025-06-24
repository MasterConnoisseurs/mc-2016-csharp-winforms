namespace MC.LaundryShop.App.Class
{
    public class ItemValue
    {
        public long? Id { get; set; }
        public long ItemId { get; set; } = 0;
        public string ItemName { get; set; } = string.Empty;
        public long ItemPriceId { get; set; } = 0;
        public decimal ItemPrice { get; set; } = 0;
        public decimal Amount { get; set; } = 0;
        public decimal TotalCost { get; set; } = 0;

        public override string ToString()
        {
            return ItemName;
        }
    }
}
