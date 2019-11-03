﻿using System;
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
        private ObjetoAramado poligonoSelecionado;
        private PrimitiveType tipoPrimitiva = PrimitiveType.LineLoop;
        #endregion

        #region ConstrutorSingleton
        public Mundo(int width, int height) : base(width, height)
        {

        }

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
                #region questao04:
                case Key.D:
                    ApagarPontoAtual();
                    break;
                #endregion
                #region questao05:
                case Key.V:
                    RemoverPontoSelecionado();
                    break;
                #endregion
                #region questao07:
                case Key.P:
                    tipoPrimitiva = PrimitiveType.LineStrip;
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
            Ponto4D pontoNovo = new Ponto4D(e.Position.X, 600 - e.Position.Y, 0);

            if (e.Button == MouseButton.Right)
            {
                MoverPonto(pontoNovo);
            }
            else if (e.Button == MouseButton.Left)
            {
                AdicionarPoligono(pontoNovo);
            }
        }


        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            //retanguloB.MoverPtoSupDir(new Ponto4D(e.Position.X, 600 - e.Position.Y, 0));
        }
        #endregion

        private void RemoverPontoSelecionado()
        {
            if (poligonoSelecionado != null && pontoSelecionado != null)
            {
                poligonoSelecionado.RemoverPonto(pontoSelecionado);
            }
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

        private void MoverPonto(Ponto4D pontoNovo)
        {
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
                        poligonoSelecionado = poligono ;
                    }
                }
            }
        }

        private double CalcularDistancia(Ponto4D pontoAtual, Ponto4D pontoNovo)
        {
            double a = (pontoNovo.X - pontoAtual.X);
            double b = (pontoNovo.Y - pontoAtual.Y);
            double disctancia = Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
            return disctancia;
        }

        private void AdicionarPoligono(Ponto4D pontoNovo)
        {
            if (pontoSelecionado != null)
            {
                pontoSelecionado.X = pontoNovo.X;
                pontoSelecionado.Y = pontoNovo.Y;
                pontoSelecionado = null;
                poligonoSelecionado = null;
            }
            else if (poligonoAtual == null)
            {
                poligonoAtual = new ObjetoAramado("A");
                poligonoAtual.DefinirPrimitiva(tipoPrimitiva);
                poligonoAtual.PontosAdicionar(pontoNovo);
                objetosLista.Add(poligonoAtual);
            }
            else
            {
                poligonoAtual.DefinirPrimitiva(tipoPrimitiva);
                poligonoAtual.PontosAdicionar(pontoNovo);
            }
        }
    }
}