namespace MC.LaundryShop.App.Class
{
    public class TransactionDeleted
    {
        public long Id { get; set; }
        public long ItemId { get; set; }
        public decimal Amount { get; set; } = 0;
    }
}
