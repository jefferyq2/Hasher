using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HasherTest.Interfaces
{
    public enum HashFunction
    {
        MD5 = 1, 
        SHA1 = 2, 
        SHA256 = 3, 
        SHA384 = 4,
        SHA512 = 5, 
        Blake2b = 6, 
        Blake3 = 7,
        Blake3MultiThreaded = 8,
    }
}
