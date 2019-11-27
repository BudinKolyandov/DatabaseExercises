namespace CompositePattern
{
    class SingleGift : GiftBase
    {
        public SingleGift(string name, decimal price)
            :base(name, price)
        {

        }


        public override decimal CalculateTotalPrice()
        {
            System.Console.WriteLine($"{name} with the price {price}");

            return price;
        }
    }
}
