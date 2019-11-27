namespace TemplatePattern
{
    public class TwelveGrain : Bread
    {
        public override void Bake()
        {
            System.Console.WriteLine("Baking the 12-Grain Bread. (25 minutes)");
        }

        public override void MixIngredients()
        {
            System.Console.WriteLine("Gathering ingredients for 12-Grain Bread.");
        }
    }
}
