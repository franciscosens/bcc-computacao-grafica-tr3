#define CG_Gizmo
// #define CG_Privado

using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Collections.Generic;
using OpenTK.Input;
using CG_Biblioteca;
using Color = OpenTK.Color;

namespace gcgcg
{
    class Mundo : GameWindow
    {
        private static Mundo instanciaMundo = null;
        private Camera camera = new Camera();
        protected List<Objeto> objetosLista = new List<Objeto>();
        private Ponto4D pontoSelecionado = null;
        private bool moverPto = false;
        private PrimitiveType tipoPrimitiva = PrimitiveType.LineLoop;
        private bool bBoxDesenhar = false;
        int mouseX, mouseY;
        private ObjetoAramado objetoNovo = null, objetoFilho = null, objetoSelecionado = null;
        private int objetoId = 0;
        private bool adicionarFilhos = false;

        private Mundo(int width, int height) : base(width, height) { }

        public static Mundo GetInstance(int width, int height)
        {
            if (instanciaMundo == null)
                instanciaMundo = new Mundo(width, height);
            return instanciaMundo;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GL.ClearColor(Color.Gray);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(camera.xmin, camera.xmax, camera.ymin, camera.ymax, camera.zmin, camera.zmax);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
#if CG_Gizmo
            Sru3D();
#endif
            for (var i = 0; i < objetosLista.Count; i++)
                objetosLista[i].Desenhar();
            if (bBoxDesenhar && (objetoSelecionado != null))
                objetoSelecionado.BBox.Desenhar();
            this.SwapBuffers();
        }

        /// <summary>
        /// Método que controla a ação do mouse.
        /// </summary>
        /// <remarks>
        /// Botão direito move o vértice selecionado e botão esquerdo insere o polígono.
        /// </remarks>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {

            if (e.Button == MouseButton.Right)
            {
                MoverPonto(objetosLista);
            }
            else if (e.Button == MouseButton.Left)
            {
                AdicionarPoligono();
            }
        }

        /// <summary>
        /// Método que permite mover o vértice selecionado.
        /// </summary>
        private void MoverPonto(List<Objeto> objetosLista)
        {
            objetoSelecionado = null;

            Ponto4D pontoNovo = new Ponto4D(mouseX, mouseY, 0);
            foreach (var objeto in objetosLista)
            {
                var poligono = (ObjetoAramado)objeto;
                int numeroInterseccoes = poligono.PontoEmPoligono(pontoNovo);
                if (numeroInterseccoes % 2 != 0)
                {
                    objetoSelecionado = poligono;
                    double maiorDistancia = double.MaxValue;
                    foreach (var pontoAtual in poligono.ObterPontos())
                    {
                        double distancia = CalcularDistancia(pontoAtual, pontoNovo);
                        if (distancia < maiorDistancia)
                        {
                            maiorDistancia = distancia;
                            pontoSelecionado = pontoAtual;
                        }
                    }
                    break;
                }
                else
                {
                    for (int i = 0; i < poligono.ObjetosLista().Count; i++)
                    {
                        MoverPonto(poligono.ObjetosLista());
                    }
                }

            }
            if (objetoSelecionado != null)
            {
                bBoxDesenhar = true;
            }
        }

        /// <summary>
        /// Método que calcula a distância entre o vértice atual e o novo vértice.
        /// </summary>
        /// <param name="pontoAtual"></param>
        /// <param name="pontoNovo"></param>
        /// <returns> Retorna a distância entre os dois pontos.</returns>
        private double CalcularDistancia(Ponto4D pontoAtual, Ponto4D pontoNovo)
        {
            double a = (pontoNovo.X - pontoAtual.X);
            double b = (pontoNovo.Y - pontoAtual.Y);
            double disctancia = Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
            return disctancia;
        }


        /// <summary>
        /// Método que permite remover o vértice selecionado.
        /// </summary>
        private void ApagarPontoAtual()
        {
            if (objetoNovo != null)
            {
                if (objetoNovo.QuantidadePontos() > 2)
                {
                    objetoNovo.RemoverUltimoPonto();
                }
                else if (objetoNovo.QuantidadePontos() == 2)
                {
                    objetoNovo.RemoverUltimoPonto();
                    objetosLista.Remove(objetoNovo);
                    objetoNovo = null;
                }
            }
        }

        /// <summary>
        /// Método que permite remover o vértice selecionado.
        /// </summary>
        private void RemoverPontoSelecionado()
        {
            if (objetoSelecionado != null && pontoSelecionado != null)
            {
                objetoSelecionado.RemoverPonto(pontoSelecionado);
            }
        }

        /// <summary>
        /// Método que permite inserir vértice no polígono atual. Mostra o rastro ao desenhar segmentos do polígono
        /// </summary>
        private void AdicionarPoligono()
        {

            if (pontoSelecionado != null)
            {
                if (adicionarFilhos == true)
                {
                    if (objetoFilho == null)
                    {
                        objetoFilho = new ObjetoAramado("A" + ++objetoId);
                        objetoFilho.PontosAdicionar(new Ponto4D(mouseX, mouseY));
                        objetoFilho.PontosAdicionar(new Ponto4D(mouseX, mouseY));
                        objetoSelecionado.FilhoAdicionar(objetoFilho);
                    }
                    objetoFilho.DefinirPrimitiva(tipoPrimitiva);
                    objetoFilho.PontosAdicionar(new Ponto4D(mouseX, mouseY));

                }
                else
                {
                    pontoSelecionado.X = mouseX;
                    pontoSelecionado.Y = mouseY;
                    pontoSelecionado = null;
                    objetoSelecionado = null;
                    bBoxDesenhar = false;
                }
            }
            else
            {
                if (objetoNovo == null)
                {
                    objetoNovo = new ObjetoAramado("A" + ++objetoId);
                    objetoNovo.DefinirPrimitiva(tipoPrimitiva);
                    objetosLista.Add(objetoNovo);
                    objetoNovo.PontosAdicionar(new Ponto4D(mouseX, mouseY));
                    objetoNovo.PontosAdicionar(new Ponto4D(mouseX, mouseY));  // N3-Exe6: "troque" para deixar o rastro
                }
                else
                {
                    objetoNovo.DefinirPrimitiva(tipoPrimitiva);
                }
                objetoNovo.PontosAdicionar(new Ponto4D(mouseX, mouseY));
            }
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            // N3-Exe2: usar o arquivo docs/umlClasses.wsd
            // N3-Exe3: usar o arquivo bin/documentação.XML -> ver exemplo CG_Biblioteca/bin/documentação.XML
            switch (e.Key)
            {
                case Key.Escape: // N3-Exe4: Inserir ponto no polígono atual
                    Exit();
                    break;
                case Key.C:
                    Console.Clear();
                    break;

                case Key.X:
                    if (objetoSelecionado != null)
                        objetoSelecionado.PontosExibirObjeto();
                    break;
                case Key.E: // Exibe os pontos do polígono
                    {
                        for (var i = 0; i < objetosLista.Count; i++)
                        {
                            objetosLista[i].PontosExibirObjeto();
                        }
                        break;
                    }
                case Key.O:
                    bBoxDesenhar = !bBoxDesenhar; // N3-Exe9: exibe a BBox
                    break;
                case Key.M: // Exibe a matriz
                    if (objetoSelecionado != null)
                        objetoSelecionado.ExibeMatriz();
                    break;
                case Key.P: // N3-Exe7: Desenhar polígonos abertos ou fechados
                    if (objetoNovo != null)
                    {
                        tipoPrimitiva = tipoPrimitiva == PrimitiveType.LineStrip ? PrimitiveType.LineLoop : PrimitiveType.LineStrip;
                        objetoNovo.DefinirPrimitiva(tipoPrimitiva);
                    }
                    break;
                case Key.I: // Atribui a matriz identidade
                    if (objetoSelecionado != null)
                        objetoSelecionado.AtribuirIdentidade();
                    break;
                case Key.Left: // N3-Exe10: translação esquerda
                    if (objetoSelecionado != null)
                        objetoSelecionado.AplicarTransformacao(ObjetoAramado.TranslacaoXY, -10, 0);
                    break;
                case Key.Right: // N3-Exe10: translação direita
                    if (objetoSelecionado != null)
                        objetoSelecionado.AplicarTransformacao(ObjetoAramado.TranslacaoXY, 10, 0);
                    break;
                case Key.Up: // N3-Exe10: translação cima
                    if (objetoSelecionado != null)
                        objetoSelecionado.AplicarTransformacao(ObjetoAramado.TranslacaoXY, 0, 10);
                    break;
                case Key.Down: // N3-Exe10: translação baixo
                    if (objetoSelecionado != null)
                        objetoSelecionado.AplicarTransformacao(ObjetoAramado.TranslacaoXY, 0, -10);
                    break;
                case Key.D: // N3-Exe4: Remove vértice do polígono atual
                    ApagarPontoAtual();
                    break;
                case Key.V: // N3-Exe4: Remove vértice do polígono selecionado
                    RemoverPontoSelecionado();
                    break;
                case Key.PageUp: // N3-Exe11: escala
                    if (objetoSelecionado != null)
                        objetoSelecionado.AplicarTransformacao(ObjetoAramado.EscalaXY, 2, 2);
                    break;
                case Key.PageDown: // N3-Exe11: escala
                    if (objetoSelecionado != null)
                        objetoSelecionado.AplicarTransformacao(ObjetoAramado.EscalaXY, 0.5, 0.5);
                    break;
                case Key.Home: // N3-Exe11: escala
                    if (objetoSelecionado != null)
                        objetoSelecionado.AplicarTransformacao(ObjetoAramado.EscalaXYBBox, 0.5);
                    break;
                case Key.End: // N3-Exe11: escala
                    if (objetoSelecionado != null)
                        objetoSelecionado.AplicarTransformacao(ObjetoAramado.EscalaXYBBox, 2);
                    break;
                case Key.Number1: // N3-Exe12: rotação
                    if (objetoSelecionado != null)
                        objetoSelecionado.AplicarTransformacao(ObjetoAramado.RotacaoZ, 10);
                    break;
                case Key.Number2: // N3-Exe12: rotação
                    if (objetoSelecionado != null)
                        objetoSelecionado.AplicarTransformacao(ObjetoAramado.RotacaoZ, -10);
                    break;
                case Key.Number3: // N3-Exe12: rotação
                    if (objetoSelecionado != null)
                        objetoSelecionado.AplicarTransformacao(ObjetoAramado.RotacaoZBBox, 10);
                    break;
                case Key.Number4: // N3-Exe12: rotação
                    if (objetoSelecionado != null)
                        objetoSelecionado.AplicarTransformacao(ObjetoAramado.RotacaoZBBox, -10);
                    break;
                case Key.T: // N3-Exe4: Remve todos os vértices do polígono selecionado
                    if (objetoSelecionado != null)
                        objetoSelecionado.PontosRemoverTodos();
                    break;
                case Key.U: // N3-Exe4: Remve o último vértice adicionado do polígono selecionado
                    if (objetoSelecionado != null)
                    {
                        objetoSelecionado.PontosRemoverUltimo();
                        if (objetoSelecionado.QuantidadePontos() == 0)
                            objetoSelecionado = null;
                    }
                    break;
                case Key.R: // N3-Exe8: Muda a cor do polígono selecionado para vermelho
                    if (objetoSelecionado != null)
                        objetoSelecionado.Cor = Color.Red;
                    break;
                case Key.S: // Seleciona o ponto do polígono mais próximo do clique
                    {
                        foreach (ObjetoAramado objeto in objetosLista)
                        {
                            bool isInside = objeto.IsPointInPolygon(new Ponto4D(mouseX, mouseY));
                            if (isInside)
                            {
                                objetoSelecionado = objeto;
                                adicionarFilhos = false;
                                break;
                            }
                        }
                        break;
                    }
                case Key.G: // N3-Exe8: Muda a cor do polígono selecionado para verde
                    if (objetoSelecionado != null)
                        objetoSelecionado.Cor = Color.Green;
                    break;
                case Key.B: // N3-Exe8: Muda a cor do polígono selecionado para azul
                    if (objetoSelecionado != null)
                        objetoSelecionado.Cor = Color.Blue;
                    break;
                case Key.Enter: // N3-Exe4: Finaliza o polígono atual
                    if (objetoFilho != null)
                    {
                        objetoFilho.PontosRemoverUltimo();
                        adicionarFilhos = false;
                        objetoFilho = null;
                        objetoSelecionado = null;
                        objetoNovo = null;
                        pontoSelecionado = null;
                    }
                    else if (objetoNovo != null)
                    {
                        objetoSelecionado = objetoNovo;
                        objetoNovo.PontosRemoverUltimo(); // N3-Exe6: "troque" para deixar o rastro
                        objetoNovo = null;
                    }
                    break;
                case Key.Space: // N3-Exe4: Inserir ponto no polígono atual
                    AdicionarPoligono();
                    break;
                case Key.F:
                    adicionarFilhos = true;
                    break;
                case Key.Number9:
                    objetoSelecionado = null; //TODO: remover está tecla e atribuir o null qdo não tiver um poligono
                    break;
            }
        }

        //FIXME: não está considerando o NDC
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            mouseX = e.Position.X;
            mouseY = 600 - e.Position.Y; // Inverti eixo Y
            if (objetoNovo != null)
            {
                objetoNovo.PontosUltimo().X = mouseX; // N3-Exe5: movendo um vértice de um poligono específico
                objetoNovo.PontosUltimo().Y = mouseY;
            }
            else if (objetoFilho != null)
            {
                objetoFilho.PontosUltimo().X = mouseX; // N3-Exe5: movendo um vértice de um poligono específico
                objetoFilho.PontosUltimo().Y = mouseY;
            }
        }

#if CG_Gizmo
        private void Sru3D()
        {
            GL.LineWidth(1);
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.Red);
            GL.Vertex3(0, 0, 0); GL.Vertex3(200, 0, 0);
            GL.Color3(Color.Green);
            GL.Vertex3(0, 0, 0); GL.Vertex3(0, 200, 0);
            GL.Color3(Color.Blue);
            GL.Vertex3(0, 0, 0); GL.Vertex3(0, 0, 200);
            GL.End();
        }
#endif
    }

}
