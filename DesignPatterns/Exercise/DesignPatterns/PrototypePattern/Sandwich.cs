using System.Linq;

namespace PrototypePattern
{
    public class Sandwich : SandwichPrototype
    {
        private string bread;
        private string meat;
        private string cheese;
        private string veggies;

        public Sandwich(string bread, string meat, string cheese, string veggies)
        {
            this.bread = bread;
            this.meat = meat;
            this.cheese = cheese;
            this.veggies = veggies;
        }

        private string GetIngredientList()
        {
            return $"{this.bread}, {this.meat}, {this.cheese}, {this.veggies}";
        }


        public override SandwichPrototype Clone()
        {
            string ingredients = GetIngredientList();
            var ingredientsArray = ingredients.Split(", ", System.StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

            System.Console.WriteLine($"Cloning sandwich with ingredients: " +
                $"{string.Join(", ", ingredientsArray)}");

            return MemberwiseClone() as SandwichPrototype;
        }
    }
}
