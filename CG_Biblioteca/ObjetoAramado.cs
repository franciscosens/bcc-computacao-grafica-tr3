using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace CG_Biblioteca
{
    public class ObjetoAramado : Objeto
    {
        protected List<Ponto4D> pontosLista = new List<Ponto4D>();
        public Color Cor { get; set; }

        public ObjetoAramado(string rotulo) : base(rotulo)
        {
            Cor = Color.White;
        }

        protected override void DesenharAramado()
        {
            GL.LineWidth(base.PrimitivaTamanho);
            GL.Color3(Cor);
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
        public void RemoverUltimoPonto()
        {
            pontosLista.RemoveAt(pontosLista.Count - 1);
            pontosLista.RemoveAt(pontosLista.Count - 1);
        }

        public int QuantidadePontos() => pontosLista.Count;

        public void DefinirPrimitiva(PrimitiveType primitiveType)
        {
            base.PrimitivaTipo = primitiveType;
        }
    }
}