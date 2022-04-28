using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleChess
{
    public abstract class InputManager<T>
    {
        public Func<(T square, bool isSelected), bool> Validate { get; }
        public InputManager(Func<(T square, bool isSelected), bool> validate)
        {
            Validate = validate;
        }
        //public abstract (T square, bool isSelected) GetInput();
        public abstract (T square, bool isSelected) GetInput(T current);
    }
}
