using System;
using System.Collections.Generic;
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using Color = OpenTK.Color;

namespace gcgcg
{
    internal class ObjetoAramado : Objeto
    {

        public const int EscalaXY = 0, EscalaXYBBox = 1, RotacaoZ = 2, RotacaoZBBox = 3, TranslacaoXY = 4;

        protected List<Ponto4D> pontosLista = new List<Ponto4D>();
        public Color Cor { get; set; }
        public ObjetoAramado(string rotulo) : base(rotulo)
        {
            Cor = Color.White;
        }

        /// <summary>
        /// Método para desenhar o polígono
        /// </summary>
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

        /// <summary>
        /// Método para adicionar vértices no polígono
        /// </summary>
        /// <param name="pto"></param>
        public void PontosAdicionar(Ponto4D pto)
        {
            pontosLista.Add(pto);
            if (pontosLista.Count.Equals(1))
                base.BBox.Atribuir(pto);
            else
                base.BBox.Atualizar(pto);
            base.BBox.ProcessarCentro();
        }

        public void ReprocessarBBox()
        {
            base.BBox.Atribuir(pontosLista[0]);
            foreach (var ponto in pontosLista)
            {
                base.BBox.Atualizar(ponto);
            }
            base.BBox.ProcessarCentro();
        }

        /// <summary>
        /// Método para remover o último vértice adicionado no polígono
        /// </summary>
        public void PontosRemoverUltimo()
        {
            pontosLista.RemoveAt(pontosLista.Count - 1);
        }

        /// <summary>
        /// Método para remover todos os vértices do polígono
        /// </summary>
        public void PontosRemoverTodos()
        {
            pontosLista.Clear();
        }

        /// <summary>
        /// Método para retornar o último vértice adicionado no polígono
        /// </summary>
        /// <returns> Retorna o último ponto da lista.</returns>
        public Ponto4D PontosUltimo()
        {
            return pontosLista[pontosLista.Count - 1];
        }

        /// <summary>
        /// Método para exibir os pontos do polígono
        /// </summary>
        protected override void PontosExibir()
        {
            Console.WriteLine("__ Objeto: " + base.rotulo);
            for (var i = 0; i < pontosLista.Count; i++)
            {
                Console.WriteLine("P" + i + "[" + pontosLista[i].X + "," + pontosLista[i].Y + "," + pontosLista[i].Z + "," + pontosLista[i].W + "]");
            }
            var listaObjetos = ObjetosLista();
            for (int i = 0; i < ObjetosLista().Count; i++)
            {
                listaObjetos[i].PontosExibirObjeto();
            }
        }

        /// <summary>
        /// Método para obter o último ponto do polígono 
        /// </summary>
        public Ponto4D ObterUltimoPonto() => pontosLista[pontosLista.Count - 1];

        public int PontoEmPoligono(Ponto4D pontoSelecao)
        {
            int numeroInterseccoes = 0;
            for (int i = 0; i < pontosLista.Count; i++)
            {

                Ponto4D pontoAtual = pontosLista[i];
                Ponto4D pontoProximo = i + 1 < pontosLista.Count ? pontosLista[i + 1] : pontosLista[i];

                double ti = (pontoSelecao.Y - pontoAtual.Y) / (pontoProximo.Y - pontoAtual.Y);
                if (ti >= 0 && ti <= 1)
                {
                    double xi = pontoAtual.X + ((pontoProximo.X - pontoAtual.X) * ti);
                    if (xi >= pontoSelecao.X)
                    {
                        numeroInterseccoes++;
                    }
                }

            }
            return numeroInterseccoes;
        }

        /// <summary>
        /// Método para remover o último ponto do polígono
        /// </summary>
        public void RemoverUltimoPonto()
        {
            pontosLista.RemoveAt(pontosLista.Count - 1);
        }

        /// <summary>
        /// Método para definir o tipo da primitiva do polígono
        /// </summary>
        /// <param name="primitiveType"></param>
        public void DefinirPrimitiva(PrimitiveType primitiveType)
        {
            base.PrimitivaTipo = primitiveType;
        }

        /// <summary>
        /// Método para obter os pontos do polígono
        /// </summary>
        public List<Ponto4D> ObterPontos() => pontosLista;

        /// <summary>
        /// Método para obter a quantidade de pontos do polígono
        /// </summary>
        public int QuantidadePontos() => pontosLista.Count;

        /// <summary>
        /// Método para remover o ponto selecionado do polígono
        /// </summary>
        /// <param name="pontoSelecionado"></param>
        public void RemoverPonto(Ponto4D pontoSelecionado)
        {
            pontosLista.Remove(pontoSelecionado);
        }

        /// <summary>
        /// Método para verificar se o ponto n ao polígono
        /// </summary>
        /// <param name="p"></param>
        /// <returns> Retorna se o ponto pertence ao polígono.</returns>
        public bool IsPointInPolygon(Ponto4D p)
        {
            double minX = pontosLista[0].X;
            double maxX = pontosLista[0].X;
            double minY = pontosLista[0].Y;
            double maxY = pontosLista[0].Y;
            for (int i = 1; i < pontosLista.Count; i++)
            {
                Ponto4D q = pontosLista[i];
                minX = Math.Min(q.X, minX);
                maxX = Math.Max(q.X, maxX);
                minY = Math.Min(q.Y, minY);
                maxY = Math.Max(q.Y, maxY);
            }

            if (p.X < minX || p.X > maxX || p.Y < minY || p.Y > maxY)
            {
                return false;
            }

            // Ray-casting
            bool inside = false;
            for (int i = 0, j = pontosLista.Count - 1; i < pontosLista.Count; j = i++)
            {
                if ((pontosLista[i].Y > p.Y) != (pontosLista[j].Y > p.Y)
                    && p.X < (pontosLista[j].X - pontosLista[i].X)
                        * (p.Y - pontosLista[i].Y)
                        / (pontosLista[j].Y - pontosLista[i].Y)
                        + pontosLista[i].X)
                {
                    inside = !inside;
                }
            }

            return inside;
        }


        public override void AplicarTransformacao(int tipo, double valor1, double valor2 = 0)
        {
            switch (tipo)
            {
                case EscalaXY:
                    EscalaXY(valor1, valor2);
                    break;
                case EscalaXYBBox:
                    EscalaXYBBox(valor1);
                    break;
                case RotacaoZ:
                    RotacaoZ(valor1);
                    break;
                case RotacaoZBBox:
                    RotacaoZBBox(valor1);
                    break;
                case TranslacaoXY:
                    TranslacaoXY(valor1, valor2);
                    break;
            }
            var objetosFilhos = ObjetosLista();
            for (int i = 0; i < objetosFilhos.Count; i++)
            {
                objetosFilhos[i].AplicarTransformacao(tipo, valor1, valor1);
            }

        }

    }
}