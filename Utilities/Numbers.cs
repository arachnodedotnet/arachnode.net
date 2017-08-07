using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arachnode.Utilities
{
    public class Numbers
    {
        public static ulong NumberOfSetBits(ulong i)
        {
            ulong count;
            for (count = 0; count < i; count++)
                i &= i - 1;
            return count;
        }
    }
}
