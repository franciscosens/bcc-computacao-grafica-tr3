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
    protected List<Ponto4D> pontosLista = new List<Ponto4D>();

    public ObjetoAramado(string rotulo) : base(rotulo) { }

    public Color Cor { get; set; }

    protected override void DesenharAramado()
    {
      GL.Color3(Color.White);
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
      if (pontosLista.Count.Equals(1))
        base.BBox.Atribuir(pto);
      else
        base.BBox.Atualizar(pto);
      base.BBox.ProcessarCentro();
    }

    public void PontosRemoverUltimo()
    {
      pontosLista.RemoveAt(pontosLista.Count - 1);
    }

    public void PontosRemoverTodos()
    {
      pontosLista.Clear();
    }

    public Ponto4D PontosUltimo()
    {
      return pontosLista[pontosLista.Count - 1];
    }

    protected override void PontosExibir()
    {
      Console.WriteLine("__ Objeto: " + base.rotulo);
      for (var i = 0; i < pontosLista.Count; i++)
      {
        Console.WriteLine("P" + i + "[" + pontosLista[i].X + "," + pontosLista[i].Y + "," + pontosLista[i].Z + "," + pontosLista[i].W + "]");
      }
    }

    public void PontoProximo(int mouseX, int mouseY)
    {

    }


    public bool IsPointInPolygon( Ponto4D p )
    {
        double minX = pontosLista[ 0 ].X;
        double maxX = pontosLista[ 0 ].X;
        double minY = pontosLista[ 0 ].Y;
        double maxY = pontosLista[ 0 ].Y;
        for ( int i = 1 ; i < pontosLista.Count ; i++ )
        {
            Ponto4D q = pontosLista[ i ];
            minX = Math.Min( q.X, minX );
            maxX = Math.Max( q.X, maxX );
            minY = Math.Min( q.Y, minY );
            maxY = Math.Max( q.Y, maxY );
        }

        if ( p.X < minX || p.X > maxX || p.Y < minY || p.Y > maxY )
        {
            return false;
        }

        // Ray-casting
        bool inside = false;
        for ( int i = 0, j = pontosLista.Count - 1 ; i < pontosLista.Count ; j = i++ )
        {
            if ( ( pontosLista[ i ].Y > p.Y ) != ( pontosLista[ j ].Y > p.Y ) 
                && p.X < ( pontosLista[ j ].X - pontosLista[ i ].X ) 
                    * ( p.Y - pontosLista[ i ].Y ) 
                    / ( pontosLista[ j ].Y - pontosLista[ i ].Y ) 
                    + pontosLista[ i ].X )
            {
                inside = !inside;
            }
        }

        return inside;
    }

  }
}