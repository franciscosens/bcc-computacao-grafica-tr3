using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace CG_Biblioteca
{
    public class ObjetoAramado : Objeto
    {
        protected List<Ponto4D> pontosLista = new List<Ponto4D>();

        public ObjetoAramado(string rotulo) : base(rotulo) { }

        protected override void DesenharAramado()
        {
            GL.LineWidth(base.PrimitivaTamanho);
            GL.Color3(Color.White);
            GL.Begin(base.PrimitivaTipo);
            foreach (Ponto4D pto in pontosLista)
            {
                GL.Vertex2(pto.X, pto.Y);
            }
            GL.End();
        }

        public void PontosAdicionar(Ponto4D pto)
        {
            pontosLista.Add(pto);
        }

        protected void PontosRemoverTodos()
        {
            pontosLista.Clear();
        }

        protected override void PontosExibir()
        {
            Console.WriteLine("__ Objeto: " + base.rotulo);
            for (var i = 0; i < pontosLista.Count; i++)
            {
                Console.WriteLine("P" + i + "[" + pontosLista[i].X + "," + pontosLista[i].Y + "," + pontosLista[i].Z + "," + pontosLista[i].W + "]");
            }
        }

        public Ponto4D ObterUltimoPonto() => pontosLista[pontosLista.Count - 1];

        public void DefinirPrimitiva(PrimitiveType primitiveType)
        {
            base.PrimitivaTipo = primitiveType;
        }
    }
}