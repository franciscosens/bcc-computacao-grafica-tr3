using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK.Input;
using CG_Biblioteca;
using System.Collections.Generic;

namespace questao01
{

    public class Mundo : GameWindow
    {
        #region Declaracoes
        public static Mundo instance = null;
        public ObjetoAramado poligonoAtual = null;

        private Camera camera = new Camera();
        protected List<Objeto> objetosLista = new List<Objeto>();
        private bool moverPto = false;
        private Ponto4D pontoSelecionado = null;
        #endregion

        #region ConstrutorSingleton
        public Mundo(int width, int height) : base(width, height) { }

        public static Mundo getInstance()
        {
            if (instance == null)
                instance = new Mundo(600, 600);
            return instance;
        }
        #endregion

        #region Frame
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // GerarLinhas();

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

            for (var i = 0; i < objetosLista.Count; i++)
            {
                objetosLista[i].Desenhar();
            }

            this.SwapBuffers();
        }
        #endregion

        #region Input
        protected override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Exit();
                    break;
                case Key.E:
                    for (var i = 0; i < objetosLista.Count; i++)
                        objetosLista[i].PontosExibirObjeto();
                    break;
                case Key.M:
                    moverPto = !moverPto;
                    break;
                case Key.Space:
                    poligonoAtual = null;
                    break;
                #region questao03:
                case Key.V:
                    RemoverPontoSelecionado();
                    break;
                #endregion
                #region questao04:
                case Key.D:
                    ApagarPontoAtual();
                    break;
                #endregion
                #region questao08:
                case Key.B:
                    poligonoAtual.Cor = Color.Blue;
                    break;
                case Key.G:
                    poligonoAtual.Cor = Color.Green;
                    break;
                case Key.R:
                    poligonoAtual.Cor = Color.Red;
                    break;
                #endregion
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Right)
            {
                MoverPonto();
            }
            else if (e.Button == MouseButton.Left)
            {
                AdicionarPoligono(e);
            }
        }


        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            //retanguloB.MoverPtoSupDir(new Ponto4D(e.Position.X, 600 - e.Position.Y, 0));
        }
        #endregion

        private void RemoverPontoSelecionado()
        {
            throw new NotImplementedException();
        }

        private void ApagarPontoAtual()
        {
            if (poligonoAtual != null)
            {
                if (poligonoAtual.QuantidadePontos() > 2)
                {
                    poligonoAtual.RemoverUltimoPonto();
                }
                else if (poligonoAtual.QuantidadePontos() == 2)
                {
                    poligonoAtual.RemoverUltimoPonto();
                    objetosLista.Remove(poligonoAtual);
                    poligonoAtual = null;
                }
            }
        }

        private void MoverPonto()
        {
            //pontoSelecionado = poligonoAtual.
        }

        private void AdicionarPoligono(MouseButtonEventArgs e)
        {
            if (poligonoAtual == null)
            {
                poligonoAtual = new ObjetoAramado("A");
                poligonoAtual.DefinirPrimitiva(PrimitiveType.LineLoop);
                poligonoAtual.PontosAdicionar(new Ponto4D(e.Position.X, 600 - e.Position.Y, 0));
                objetosLista.Add(poligonoAtual);
            }
            else
            { 
                poligonoAtual.PontosAdicionar(new Ponto4D(e.Position.X, 600 - e.Position.Y, 0));
            }
        }
    }
}