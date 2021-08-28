using System;
using System.Collections.Generic;
using System.Text;

namespace Intel8086
{
    public static class Helpers
    {

        public static bool validateHex(string hex)
        {
            foreach (var c in hex)
            {
                if (!Uri.IsHexDigit(c))
                    return false;
            }
            return true;
        }
    }
}
