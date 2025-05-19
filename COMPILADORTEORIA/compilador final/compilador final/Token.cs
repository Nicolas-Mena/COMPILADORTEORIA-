using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiladorFinal
{
    
        public class Token
        {
            public string Tipo { get; set; }
            public string Valor { get; set; }
            public int Linea { get; set; }
            public int Columna { get; set; }

            public Token(string tipo, string valor, int linea, int columna)
            {
                Tipo = tipo;
                Valor = valor;
                Linea = linea;
                Columna = columna;
            }

            public override string ToString()
            {
                return $"[{Tipo}, {Valor}, Línea: {Linea}, Columna: {Columna}]";
            }
        }
    
}
