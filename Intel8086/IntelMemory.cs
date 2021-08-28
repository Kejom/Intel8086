using System;
using System.Collections.Generic;
using System.Text;

namespace Intel8086
{
    public class IntelMemory
    {
        private string[] memory;

        public IntelMemory()
        {
            memory = new string[65536];

            for (int i = 0; i < memory.Length; i++)
            {
                memory[i] = "00";
            }
        }

        private int convertHex(string hex)
        {
            return Int32.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        }

        private void saveValue(int index, string value)
        {
            var index1 = index % (memory.Length - 1);
            var index2 = (index1 + 1) % (memory.Length - 1);
            memory[index1] = value.Substring(0, 2);
            memory[index2] = value.Substring(2, 2);
        }

        private string getValue(int index)
        {
            var index1 = index % (memory.Length - 1);
            var index2 = (index1 + 1) % (memory.Length - 1);
            return memory[index1] + memory[index2];
        }

        public void Mov(string indexOrBase, string disp, string value)
        {
            var index = convertHex(indexOrBase) + convertHex(disp);
            saveValue(index, value);
        }

        public void Mov(string i, string b, string disp, string value)
        {
            var index = convertHex(i) + convertHex(b) + convertHex(disp);
            saveValue(index, value);
        }

        public string Xchg(string indexOrBase, string disp, string value)
        {
            var index = convertHex(indexOrBase) + convertHex(disp);
            var memVal = getValue(index);
            saveValue(index, value);
            return value;
        }

        public string Xchg(string i, string b, string disp, string value)
        {
            var index = convertHex(i) + convertHex(b) + convertHex(disp);
            var memVal = getValue(index);
            saveValue(index, value);
            return value;
        }
    }
}
