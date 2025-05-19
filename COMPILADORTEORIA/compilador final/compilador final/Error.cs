using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiladorFinal
{
    public class Error
    {
        public string Tipo { get; set; }
        public string Descripcion { get; set; }
        public int Linea { get; set; }
        public int Columna { get; set; }

        public Error(string tipo, string descripcion, int linea, int columna)
        {
            Tipo = tipo;
            Descripcion = descripcion;
            Linea = linea;
            Columna = columna;
        }

        public override string ToString()
        {
            return $"[{Tipo}] Línea {Linea}, Columna {Columna}: {Descripcion}";
        }
    }
}
