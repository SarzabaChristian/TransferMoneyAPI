using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AssesmentOnlineAPI.Helpers
{
    public class ThreadControl
    {   
        public static long threadCount = 0;
        public static object obj = threadCount;         
        public static List<Task> threads = new List<Task>();       
    }
}
