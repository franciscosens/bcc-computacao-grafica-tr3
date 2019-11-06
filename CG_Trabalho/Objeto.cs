using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using CG_Biblioteca;

namespace gcgcg
{
    internal abstract class Objeto
    {
        protected string rotulo;
        private PrimitiveType primitivaTipo = PrimitiveType.LineLoop;
        protected PrimitiveType PrimitivaTipo { get => primitivaTipo; set => primitivaTipo = value; }
        private float primitivaTamanho = 2;
        protected float PrimitivaTamanho { get => primitivaTamanho; set => primitivaTamanho = value; }
        private BBox bBox = new BBox();
        public BBox BBox { get => bBox; set => bBox = value; }
        private List<Objeto> objetosLista = new List<Objeto>();
        private Transformacao4D matriz = new Transformacao4D();
        /// Matrizes temporarias que sempre sao inicializadas com matriz Identidade entao podem ser "static".
        private static Transformacao4D matrizTmpTranslacao = new Transformacao4D();
        private static Transformacao4D matrizTmpTranslacaoInversa = new Transformacao4D();
        private static Transformacao4D matrizTmpEscala = new Transformacao4D();
        private static Transformacao4D matrizTmpRotacao = new Transformacao4D();
        private static Transformacao4D matrizGlobal = new Transformacao4D();

        public Objeto(string rotulo)
        {
            this.rotulo = rotulo;
        }

        public void Desenhar()
        {
            GL.PushMatrix(); // N3-Exe14: grafo de cena
            GL.MultMatrix(matriz.ObterDados());
            DesenharAramado();
            for (var i = 0; i < objetosLista.Count; i++)
            {
                var objeto = objetosLista[i];
                objeto.Desenhar();
                var listaObjetos = objeto.objetosLista;
                if (listaObjetos.Count != 0)
                {
                    for (var j = 0; j < listaObjetos.Count; i++)
                    {
                        listaObjetos[i].Desenhar();
                    }
                }

            }
            GL.PopMatrix(); // N3-Exe14: grafo de cena
        }

        protected abstract void DesenharAramado();

        public void FilhoAdicionar(Objeto filho)
        {
            this.objetosLista.Add(filho);
        }

        public void FilhoRemover(Objeto filho)
        {
            this.objetosLista.Remove(filho);
        }

        public List<Objeto> ObjetosLista() => objetosLista;

        public abstract void AplicarTransformacao(int tipo, double valor1, double valor2);

        protected abstract void PontosExibir();

        /// <summary>
        /// Método para exibir os pontos do objeto
        /// </summary>
        public void PontosExibirObjeto()
        {
            PontosExibir();
        }

        /// <summary>
        /// Método para exibir a matriz
        /// </summary>
        public void ExibeMatriz()
        {
            matriz.ExibeMatriz();
        }

        /// <summary>
        /// Método para atribuir a matriz identidade
        /// </summary>
        public void AtribuirIdentidade()
        {
            matriz.AtribuirIdentidade();
        }

        /// <summary>
        /// Método para movimentar o polígono
        /// </summary>
        /// <param name="tx"></param>
        /// <param name="ty"></param>
        public void TranslacaoXY(double tx, double ty)
        {
            Transformacao4D matrizTranslate = new Transformacao4D();
            matrizTranslate.AtribuirTranslacao(tx, ty, 0);
            matriz = matrizTranslate.MultiplicarMatriz(matriz);
        }

        /// <summary>
        /// Método para redimensionar o polígono
        /// </summary>
        /// <param name="Sx"></param>
        /// <param name="Sy"></param>
        public void EscalaXY(double Sx, double Sy)
        {
            Transformacao4D matrizScale = new Transformacao4D();
            matrizScale.AtribuirEscala(Sx, Sy, 1.0);
            matriz = matrizScale.MultiplicarMatriz(matriz);
        }

        /// <summary>
        /// Método para redimensionar o polígono em relação ao centro da BBox
        /// </summary>
        /// <param name="escala"></param>
        public void EscalaXYBBox(double escala)
        {
            matrizGlobal.AtribuirIdentidade();
            Ponto4D pontoPivo = bBox.obterCentro;

            matrizTmpTranslacao.AtribuirTranslacao(-pontoPivo.X, -pontoPivo.Y, -pontoPivo.Z); // Inverter sinal
            matrizGlobal = matrizTmpTranslacao.MultiplicarMatriz(matrizGlobal);

            matrizTmpEscala.AtribuirEscala(escala, escala, 1.0);
            matrizGlobal = matrizTmpEscala.MultiplicarMatriz(matrizGlobal);

            matrizTmpTranslacaoInversa.AtribuirTranslacao(pontoPivo.X, pontoPivo.Y, pontoPivo.Z);
            matrizGlobal = matrizTmpTranslacaoInversa.MultiplicarMatriz(matrizGlobal);

            matriz = matriz.MultiplicarMatriz(matrizGlobal);
        }

        /// <summary>
        /// Método para rotacionar o polígono
        /// </summary>
        /// <param name="angulo"></param>
        public void RotacaoZ(double angulo)
        {
            matrizTmpRotacao.AtribuirRotacaoZ(Transformacao4D.DEG_TO_RAD * angulo);
            matriz = matrizTmpRotacao.MultiplicarMatriz(matriz);
        }

        /// <summary>
        /// Método para rotacionar o polígono em relação ao centro da BBox
        /// </summary>
        /// <param name="angulo"></param>
        public void RotacaoZBBox(double angulo)
        {
            matrizGlobal.AtribuirIdentidade();
            Ponto4D pontoPivo = bBox.obterCentro;

            matrizTmpTranslacao.AtribuirTranslacao(-pontoPivo.X, -pontoPivo.Y, -pontoPivo.Z); // Inverter sinal
            matrizGlobal = matrizTmpTranslacao.MultiplicarMatriz(matrizGlobal);

            matrizTmpRotacao.AtribuirRotacaoZ(Transformacao4D.DEG_TO_RAD * angulo);
            matrizGlobal = matrizTmpRotacao.MultiplicarMatriz(matrizGlobal);

            matrizTmpTranslacaoInversa.AtribuirTranslacao(pontoPivo.X, pontoPivo.Y, pontoPivo.Z);
            matrizGlobal = matrizTmpTranslacaoInversa.MultiplicarMatriz(matrizGlobal);

            matriz = matriz.MultiplicarMatriz(matrizGlobal);
        }
    }
}