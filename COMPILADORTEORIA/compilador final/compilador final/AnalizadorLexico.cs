using System;
using System.Collections.Generic;

namespace CompiladorFinal
{

    /// Clase que implementa el analizador léxico para el compilador

    public class AnalizadorLexico
    {
        // Diccionario de palabras reservadas del lenguaje con sus tokens correspondientes
        //usa poo
        private readonly Dictionary<string, string> PalabrasReservadas = new Dictionary<string, string>
        {
            {"class", "CLASS"}, {"if", "IF"}, {"else", "ELSE"}, {"for", "FOR"}, {"while", "WHILE"},
            {"int", "INT"}, {"string", "STRING"}, {"char", "CHAR"}, {"float", "FLOAT"}, {"double", "DOUBLE"},
            {"print", "PRINT"}, {"and", "AND"}, {"or", "OR"}
        };

        // Diccionario de operadores con sus tokens correspondientes
        //usa poo
        private readonly Dictionary<string, string> Operadores = new Dictionary<string, string>
        {
            {"+", "SUMA"}, {"-", "RESTA"}, {"*", "MULTIPLICACION"}, {"/", "DIVISION"}, {"=", "ASIGNACION"},
            {"&&", "AND_LOGICO"}, {"||", "OR_LOGICO"}, {"<", "MENOR"}, {">", "MAYOR"}, {"<=", "MENOR_IGUAL"},
            {">=", "MAYOR_IGUAL"}, {"==", "IGUALDAD"}, {"!=", "DIFERENTE"}
        };

        // Diccionario de delimitadores con sus tokens correspondientes
        //usa poo
        private readonly Dictionary<string, string> Delimitadores = new Dictionary<string, string>
        {
            {"(", "PARENTESIS_IZQ"}, {")", "PARENTESIS_DER"}, {"{", "LLAVE_IZQ"}, {"}", "LLAVE_DER"},
            {";", "PUNTO_COMA"}, {"\"", "COMILLA"}, {"'", "COMILLA_SIMPLE"}
        };

    
        /// Método principal que analiza el código fuente y genera una lista de tokens
       
    //usa poo
        public List<Token> Analizar(string codigo, List<Error> errores)
        {
            List<Token> tokens = new List<Token>();
            int linea = 1, columna = 1, posicion = 0;

            // Recorrer todo el código fuente caracter por caracter
            while (posicion < codigo.Length)
            {
                char caracterActual = codigo[posicion];

                // ==============================================
                // MANEJO DE ESPACIOS EN BLANCO Y SALTOS DE LÍNEA
                // ==============================================
                if (char.IsWhiteSpace(caracterActual))
                {
                    if (caracterActual == '\n')
                    {
                        linea++;
                        columna = 1;
                    }
                    else
                    {
                        columna++;
                    }
                    posicion++;
                    continue;
                }

                // ==============================================
                // MANEJO DE COMENTARIOS DE LÍNEA
                // ==============================================
                if (caracterActual == '/' && posicion + 1 < codigo.Length && codigo[posicion + 1] == '/')
                {
                    // Saltar todo hasta el final de la línea
                    while (posicion < codigo.Length && codigo[posicion] != '\n')
                        posicion++;
                    linea++;
                    columna = 1;
                    continue;
                }

                // ==============================================
                // IDENTIFICADORES Y PALABRAS RESERVADAS
                // ==============================================
                if (char.IsLetter(caracterActual) || caracterActual == '_')
                {
                    int inicio = posicion;
                    // Avanzar mientras sean letras, dígitos o guiones bajos
                    while (posicion < codigo.Length && (char.IsLetterOrDigit(codigo[posicion]) || codigo[posicion] == '_'))
                        posicion++;

                    string lexema = codigo.Substring(inicio, posicion - inicio);
                    columna += lexema.Length;

                    // Verificar si es palabra reservada
                    if (PalabrasReservadas.ContainsKey(lexema))
                        tokens.Add(new Token(PalabrasReservadas[lexema], lexema, linea, columna - lexema.Length));
                    else
                        tokens.Add(new Token("IDENTIFICADOR", lexema, linea, columna - lexema.Length));
                    continue;
                }

                // ==============================================
                // NÚMEROS (ENTEROS Y DECIMALES)
                // ==============================================
                if (char.IsDigit(caracterActual))
                {
                    int inicio = posicion;
                    bool tienePunto = false;

                    // Avanzar mientras sean dígitos o puntos
                    while (posicion < codigo.Length && (char.IsDigit(codigo[posicion]) || codigo[posicion] == '.'))
                    {
                        if (codigo[posicion] == '.')
                        {
                            if (tienePunto)
                            {
                                // Error: múltiples puntos decimales
                                errores.Add(new Error("Léxico", "Número con múltiples puntos decimales", linea, columna));
                                break;
                            }
                            tienePunto = true;
                        }
                        posicion++;
                    }

                    string lexema = codigo.Substring(inicio, posicion - inicio);
                    columna += lexema.Length;

                    // Determinar si es entero o decimal
                    tokens.Add(new Token(tienePunto ? "DECIMAL" : "ENTERO", lexema, linea, columna - lexema.Length));
                    continue;
                }

                // ==============================================
                // CADENAS Y CARACTERES
                // ==============================================
                if (caracterActual == '"' || caracterActual == '\'')
                {
                    char delimitador = caracterActual;
                    int inicio = posicion;
                    posicion++; columna++;
                    bool cerrada = false;
                    int longitud = 0; // Para contar caracteres internos

                    // Buscar el cierre del delimitador
                    while (posicion < codigo.Length && codigo[posicion] != delimitador)
                    {
                        if (codigo[posicion] == '\n')
                        {
                            linea++;
                            columna = 1;
                        }
                        posicion++;
                        columna++;
                        longitud++;
                    }

                    // Verificar si se encontró el cierre
                    if (posicion < codigo.Length && codigo[posicion] == delimitador)
                    {
                        cerrada = true;
                        posicion++;
                        columna++;
                    }

                    string lexema = codigo.Substring(inicio, posicion - inicio);

                    // Manejo especial para caracteres
                    if (delimitador == '\'')
                    {
                        // Caso especial para carácter vacío
                        if (lexema == "''" || longitud == 0)
                        {
                            tokens.Add(new Token("CARACTER", "''", linea, columna - 2));
                        }
                        else if (longitud != 1)
                        {
                            errores.Add(new Error("Léxico", "Demasiados caracteres en literal de carácter", linea, columna - lexema.Length));
                        }
                        else
                        {
                            tokens.Add(new Token("CARACTER", lexema, linea, columna - lexema.Length));
                        }
                    }
                    else // Manejo de cadenas
                    {
                        if (!cerrada)
                        {
                            errores.Add(new Error("Léxico", "Cadena no cerrada", linea, columna));
                        }
                        tokens.Add(new Token("CADENA", lexema, linea, columna - lexema.Length));
                    }
                    continue;
                }

                // ==============================================
                // OPERADORES Y DELIMITADORES
                // ==============================================
                bool encontrado = false;

                // Primero verificar operadores de 2 caracteres
                if (posicion + 1 < codigo.Length)
                {
                    string dosCaracteres = codigo.Substring(posicion, 2);
                    if (Operadores.ContainsKey(dosCaracteres) || Delimitadores.ContainsKey(dosCaracteres))
                    {
                        string tipo = Operadores.ContainsKey(dosCaracteres) ? Operadores[dosCaracteres] : Delimitadores[dosCaracteres];
                        tokens.Add(new Token(tipo, dosCaracteres, linea, columna));
                        posicion += 2;
                        columna += 2;
                        encontrado = true;
                    }
                }

                // Si no se encontró operador de 2 caracteres, verificar de 1 carácter
                if (!encontrado)
                {
                    string unCaracter = codigo[posicion].ToString();
                    if (Operadores.ContainsKey(unCaracter) || Delimitadores.ContainsKey(unCaracter))
                    {
                        string tipo = Operadores.ContainsKey(unCaracter) ? Operadores[unCaracter] : Delimitadores[unCaracter];
                        tokens.Add(new Token(tipo, unCaracter, linea, columna));
                        posicion++;
                        columna++;
                        encontrado = true;
                    }
                }

                // Si no es ningún token reconocido, registrar error
                if (!encontrado)
                {
                    errores.Add(new Error("Léxico", $"Carácter no reconocido: '{caracterActual}'", linea, columna));
                    posicion++;
                    columna++;
                }
            }

            return tokens;
        }
    }
}
