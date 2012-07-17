using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconListener
{
    class Program
    {
        static void Main(string[] args)
        {
            Listener listener = new Listener();
            listener.Start();
            Console.ReadKey();
            listener.Stop();
        }
    }
}
