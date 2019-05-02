using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace snakeGame
{
    class Apple
    {
        //David
        private static int randomNumberMethod()
        {
            //word generation for easy + hard
            int randomnumber;
            //generates random number within 0-20
            Random r = new Random();
            randomnumber = r.Next(17);
            return randomnumber;
        }
        //ToDo (Dave): Globals
        readonly Point position;

       
        //ToDo (Josh): Methods
    }
}
