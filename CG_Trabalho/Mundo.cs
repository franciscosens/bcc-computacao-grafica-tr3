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
        private ObjetoAramado objetoSelecionado = null;
        private Ponto4D pontoSelecionado = null;
        private bool moverPto = false;
        private PrimitiveType tipoPrimitiva = PrimitiveType.LineLoop;
        private bool bBoxDesenhar = false;
        int mouseX, mouseY;   //FIXME: achar método MouseDown para não ter variável Global
        private ObjetoAramado objetoNovo = null;
        private String objetoId = "A";

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

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {

            if (e.Button == MouseButton.Right)
            {
                MoverPonto();
            }
            else if (e.Button == MouseButton.Left)
            {
                AdicionarPoligono();
            }
        }

        private void MoverPonto()
        {
            Ponto4D pontoNovo = new Ponto4D(mouseX, mouseY, 0);

            double maiorDistancia = double.MaxValue;
            foreach (var objeto in objetosLista)
            {
                var poligono = (ObjetoAramado)objeto;
                foreach (var pontoAtual in poligono.ObterPontos())
                {
                    double distancia = CalcularDistancia(pontoAtual, pontoNovo);
                    if (distancia < maiorDistancia)
                    {
                        maiorDistancia = distancia;
                        pontoSelecionado = pontoAtual;
                        objetoSelecionado = poligono;
                    }
                }
            }
        }

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

        private void RemoverPontoSelecionado()
        {
            if (objetoSelecionado != null && pontoSelecionado != null)
            {
                objetoSelecionado.RemoverPonto(pontoSelecionado);
            }
        }

        private void AdicionarPoligono()
        {
            if (pontoSelecionado != null)
            {
                pontoSelecionado.X = mouseX;
                pontoSelecionado.Y = mouseY;
                pontoSelecionado = null;
            }
            else
            {
                if (objetoNovo == null)
                {
                    objetoNovo = new ObjetoAramado(objetoId + 1);
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

        private double CalcularDistancia(Ponto4D pontoAtual, Ponto4D pontoNovo)
        {
            double a = (pontoNovo.X - pontoAtual.X);
            double b = (pontoNovo.Y - pontoAtual.Y);
            double disctancia = Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
            return disctancia;
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            // N3-Exe2: usar o arquivo docs/umlClasses.wsd
            // N3-Exe3: usar o arquivo bin/documentação.XML -> ver exemplo CG_Biblioteca/bin/documentação.XML
            switch (e.Key)
            {
                case Key.Escape:
                    Exit();
                    break;
                case Key.E:
                    {
                        for (var i = 0; i < objetosLista.Count; i++)
                        {
                            objetosLista[i].PontosExibirObjeto();
                        }

                        break;
                    }

                case Key.O:
                    bBoxDesenhar = !bBoxDesenhar;   // N3-Exe9: exibe a BBox
                    break;
                case Key.M:
                    if (objetoSelecionado != null)
                        objetoSelecionado.ExibeMatriz();
                    break;
                case Key.P:
                    if(objetoSelecionado != null)
                        objetoSelecionado.PontosExibirObjeto();

                    tipoPrimitiva = tipoPrimitiva == PrimitiveType.LineStrip ? PrimitiveType.LineLoop : PrimitiveType.LineStrip;
                    objetoNovo.DefinirPrimitiva(tipoPrimitiva);
                    break;
                case Key.I:
                    if (objetoSelecionado != null)
                        objetoSelecionado.AtribuirIdentidade();
                    break;
                case Key.Left:
                    if (objetoSelecionado != null)
                        objetoSelecionado.TranslacaoXY(-10, 0);     // N3-Exe10: translação
                    break;
                case Key.Right:
                    if (objetoSelecionado != null)
                        objetoSelecionado.TranslacaoXY(10, 0);      // N3-Exe10: translação
                    break;
                case Key.Up:
                    if (objetoSelecionado != null)
                        objetoSelecionado.TranslacaoXY(0, 10);      // N3-Exe10: translação
                    break;
                case Key.Down:
                    if (objetoSelecionado != null)
                        objetoSelecionado.TranslacaoXY(0, -10);     // N3-Exe10: translação
                    break;
                case Key.D:
                    ApagarPontoAtual();
                    break;
                case Key.V:
                    RemoverPontoSelecionado();
                    break;
                case Key.PageUp:
                    if (objetoSelecionado != null)
                        objetoSelecionado.EscalaXY(2, 2);
                    break;
                case Key.PageDown:
                    if (objetoSelecionado != null)
                        objetoSelecionado.EscalaXY(0.5, 0.5);
                    break;
                case Key.Home:
                    if (objetoSelecionado != null)
                        objetoSelecionado.EscalaXYBBox(0.5);        // N3-Exe11: escala
                    break;
                case Key.End:
                    if (objetoSelecionado != null)
                        objetoSelecionado.EscalaXYBBox(2);          // N3-Exe11: escala
                    break;
                case Key.Number1:
                    if (objetoSelecionado != null)
                        objetoSelecionado.RotacaoZ(10);
                    break;
                case Key.Number2:
                    if (objetoSelecionado != null)
                        objetoSelecionado.RotacaoZ(-10);
                    break;
                case Key.Number3:
                    if (objetoSelecionado != null)
                        objetoSelecionado.RotacaoZBBox(10);         // N3-Exe12: rotação
                    break;
                case Key.Number4:
                    if (objetoSelecionado != null)
                        objetoSelecionado.RotacaoZBBox(-10);        // N3-Exe12: rotação
                    break;
                case Key.T:
                    if (objetoSelecionado != null)
                        objetoSelecionado.PontosRemoverTodos();
                    break;
                case Key.U:
                    if (objetoSelecionado != null)
                    {
                        objetoSelecionado.PontosRemoverUltimo();
                        if (objetoSelecionado.QuantidadePontos() == 0)
                            objetoSelecionado = null;
                    }
                    break;
                case Key.R:
                    if (objetoSelecionado != null)
                        objetoSelecionado.Cor = Color.Red;
                    break;
                case Key.S:
                    {
                        foreach (ObjetoAramado objeto in objetosLista)
                        {
                            bool isInside = objeto.IsPointInPolygon(new Ponto4D(mouseX, mouseY));

                            if (isInside)
                            {
                                objetoSelecionado = objeto;
                                break;
                            }
                        }

                        break;
                    }

                case Key.G:
                    if (objetoSelecionado != null)
                        objetoSelecionado.Cor = Color.Green;
                    break;
                case Key.B:
                    if (objetoSelecionado != null)
                        objetoSelecionado.Cor = Color.Blue;
                    break;
                case Key.Enter:
                    if (objetoNovo != null)
                    {
                        objetoSelecionado = objetoNovo;
                        objetoNovo.PontosRemoverUltimo();   // N3-Exe6: "troque" para deixar o rastro
                        objetoNovo = null;
                    }
                    break;
                case Key.Space:
                    AdicionarPoligono();
                    break;
                case Key.Number9:
                    objetoSelecionado = null;   //TODO: remover está tecla e atribuir o null qdo não tiver um poligono
                    break;
            }   //TODO: remover está tecla e atribuir o null qdo não tiver um poligono
        }

        //FIXME: não está considerando o NDC
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            mouseX = e.Position.X; mouseY = 600 - e.Position.Y; // Inverti eixo Y
            if (objetoNovo != null)
            {
                objetoNovo.PontosUltimo().X = mouseX;             // N3-Exe5: movendo um vértice de um poligono específico
                objetoNovo.PontosUltimo().Y = mouseY;
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
