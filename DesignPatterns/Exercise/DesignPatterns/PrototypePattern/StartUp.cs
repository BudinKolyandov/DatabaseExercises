using System;

namespace PrototypePattern
{
    public class StartUp
    {
        public static void Main()
        {
            SandwichMenu sandwichMenu = new SandwichMenu();
            sandwichMenu["BLT"] = new Sandwich("Wheat", "Bacon", "", "Lettuce, Tomato");
            sandwichMenu["PB&J"] = new Sandwich("White", "", "", "Peanut Butter, Jelly");
            sandwichMenu["Turkey"] = new Sandwich("Rye", "Turkey", "Swiss", "Lettuce, Onion, Tomato");

            sandwichMenu["LoadedBLT"] = new Sandwich("Wheat", "Ham, Bacon", "American", "Lettuce, Tomato, Onion, Olives");
            sandwichMenu["ThreeMeatCombo"] = new Sandwich("Rye", "Turkey, Ham, Bacon", "Swiss", "Lettuce, Tomato, Onion");
            sandwichMenu["Vegetarian"] = new Sandwich("Wheat", "", "", "Lettuce, Onion, Tomato, Olives, Cucumber");

            Sandwich sandwich1 = sandwichMenu["LoadedBLT"].Clone() as Sandwich;
            Sandwich sandwich2 = sandwichMenu["PB&J"].Clone() as Sandwich;
            Sandwich sandwich3 = sandwichMenu["ThreeMeatCombo"].Clone() as Sandwich;

        }
    }
}
