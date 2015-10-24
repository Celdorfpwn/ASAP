using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SushiPikant.UI.ViewModels
{
    public class TaskViewModel
    {
        public string Title { get; private set; }

        public string Reporter { get; private set; }

        public string Description { get; private set; }

        private static Random _random = new Random();

        public TaskViewModel()
        {
            Title = "#BUG-" + _random.Next(100, 999).ToString();
            Reporter = GetOwner(_random.Next(0, 30));
            Description = GetDescription(_random.Next(0, 5)) + "...";
        }


        private static string GetOwner(int nr)
        {
            if (nr < 10)
            {
                return "@qa.boss";
            }
            else if (nr < 20)
            {
                return "@noob.qa.training";
            }
            else
            {
                return "@qa.for.fun";
            }

        }

        private static string GetDescription(int nr)
        {
            switch(nr)
            {
                case 0: return "This bug is empty";
                case 1: return "I logged this bug for fun";
                case 2: return "I don't remember the bug details but I'm sure it's a bug ";
                case 3: return "Bug BUg Bug and another bug";
                default:return "Just a bug";
            }
               
        }
    }
}
