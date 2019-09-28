using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace questao01
{
    public class Program
    {
        static void Main(string[] args)
        {
            GameWindow window = new Mundo(600, 600);
            window.Run(1.0 / 60.0);
        }
    }
}