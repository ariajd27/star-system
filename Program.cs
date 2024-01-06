using System;
using System.Linq;

namespace StarSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0) {
                System system = new(false);
            }
            else if (args.Any(x => x == "-h")) {
                System system = new(true);
            }
            else {
                System system = new(false);
            }
        }
    }
}
