using System;
using System.Collections.Generic;
using System.Text;

namespace gcgcg
{
    class Program
    {
        static void Main(string[] args)
        {
            Mundo window = Mundo.GetInstance(600, 600);
            window.Title = "CG-N3";
            window.Run(1.0 / 60.0);
        }
    }
}
