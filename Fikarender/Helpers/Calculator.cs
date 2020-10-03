namespace Helpers
{
    public class Calculator
    {
        public double CalcDiscount(double price, int rate)
        {
            return price * rate / 100;
        }
    }
}
