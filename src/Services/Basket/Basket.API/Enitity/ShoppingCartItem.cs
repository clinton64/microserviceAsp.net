namespace Basket.API.Enitity
{
    public class ShoppingCartItem
    {
        public int Quantity { set; get; }
        public decimal Price { set; get; }
        public string ProductName { set; get; }
        public string ProductId { set; get; }
        public string Color { set; get; }
    }
}