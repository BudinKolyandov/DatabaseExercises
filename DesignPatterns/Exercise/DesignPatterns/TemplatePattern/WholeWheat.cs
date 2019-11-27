namespace TemplatePattern
{
    class WholeWheat : Bread
    {
        public override void Bake()
        {
            System.Console.WriteLine("Baking the Whole Wheat Bread. (15 minutes)");
        }

        public override void MixIngredients()
        {
            System.Console.WriteLine("Gathering ingredients for Whole Wheat Bread.");
        }
    }
}
