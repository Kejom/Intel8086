using System;
using System.Collections.Generic;
using System.Text;

namespace Intel8086
{
    public class IntelStack
    {
        private Stack<string> memoryStack;
        private const int MaxLength = 65536;

        public IntelStack()
        {
            memoryStack = new Stack<string>();
        }

        public void Push(string value)
        {
            if (memoryStack.Count + 2 > MaxLength)
                throw new ArgumentOutOfRangeException("stackoverflow, maximum capacity reached");
            memoryStack.Push(value.Substring(0,2));
            memoryStack.Push(value.Substring(2, 2));
        }

        public string Pop()
        {
            if (memoryStack.Count < 2)
                throw new ArgumentOutOfRangeException("stack is empty");
            var val2 = memoryStack.Pop();
            var val1 = memoryStack.Pop();
            return val1 + val2;
        }
    }
}
