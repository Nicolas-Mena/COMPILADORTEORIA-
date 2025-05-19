using System.Collections.Generic;

namespace CompiladorFinal
{
    /// <summary>
    /// Clase que implementa el analizador semántico para el compilador
    /// </summary>
    public class AnalizadorSemantico
    {
        // Lista de errores semánticos encontrados
        private List<Error> errores;

        // Diccionario para registrar variables declaradas y sus tipos
        private Dictionary<string, string> variablesDeclaradas;

        /// <summary>
        /// Constructor del analizador semántico
        /// </summary>
        public AnalizadorSemantico()
        {
            errores = new List<Error>();
            variablesDeclaradas = new Dictionary<string, string>();
        }

        /// <summary>
        /// Método principal que realiza el análisis semántico
        /// </summary>
        /// <param name="tokens">Lista de tokens a analizar</param>
        /// <returns>Lista de errores semánticos encontrados</returns>
        public List<Error> Analizar(List<Token> tokens)
        {
            // Limpiar estados previos
            errores.Clear();
            variablesDeclaradas.Clear();

            // Recorrer todos los tokens
            for (int i = 0; i < tokens.Count; i++)
            {
                // ==============================================
                // DETECCIÓN DE DECLARACIONES DE VARIABLES
                // ==============================================
                if (EsTipoDeclaracion(tokens[i].Tipo) && i + 1 < tokens.Count && tokens[i + 1].Tipo == "IDENTIFICADOR")
                {
                    string tipoVariable = tokens[i].Valor.ToLower();
                    string nombreVariable = tokens[i + 1].Valor;

                    // Registrar la variable en el diccionario
                    variablesDeclaradas[nombreVariable] = tipoVariable;

                    // Verificar si hay una asignación directa (ej: int x = 5)
                    if (i + 2 < tokens.Count && tokens[i + 2].Tipo == "ASIGNACION" && i + 3 < tokens.Count)
                    {
                        // Validación especial para strings (deben tener comillas)
                        if (tipoVariable == "string" && tokens[i + 3].Tipo != "CADENA")
                        {
                            errores.Add(new Error("Semántico",
                                "Las variables de tipo string deben asignarse con valores entre comillas dobles",
                                tokens[i + 3].Linea,
                                tokens[i + 3].Columna));
                        }
                        else
                        {
                            // Validar la compatibilidad de tipos en la asignación
                            ValidarAsignacion(tipoVariable, tokens[i + 3], tokens[i + 1].Linea, tokens[i + 1].Columna);
                        }
                        i += 3; // Saltar tipo, nombre, = y valor
                    }
                }
                // ==============================================
                // DETECCIÓN DE ASIGNACIONES POSTERIORES
                // ==============================================
                else if (tokens[i].Tipo == "IDENTIFICADOR" && i + 1 < tokens.Count && tokens[i + 1].Tipo == "ASIGNACION" && i + 2 < tokens.Count)
                {
                    string nombreVariable = tokens[i].Valor;

                    // Verificar si la variable fue declarada
                    if (variablesDeclaradas.TryGetValue(nombreVariable, out string tipoVariable))
                    {
                        // Validación especial para strings
                        if (tipoVariable == "string" && tokens[i + 2].Tipo != "CADENA")
                        {
                            errores.Add(new Error("Semántico",
                                "Las variables de tipo string deben asignarse con valores entre comillas dobles",
                                tokens[i + 2].Linea,
                                tokens[i + 2].Columna));
                        }
                        else
                        {
                            // Validar la compatibilidad de tipos
                            ValidarAsignacion(tipoVariable, tokens[i + 2], tokens[i].Linea, tokens[i].Columna);
                        }
                        i += 2; // Saltar nombre y =
                    }
                    else
                    {
                        // Error: variable no declarada
                        errores.Add(new Error("Semántico",
                            $"La variable '{nombreVariable}' no ha sido declarada",
                            tokens[i].Linea,
                            tokens[i].Columna));
                    }
                }
            }

            return errores;
        }

        /// <summary>
        /// Valida que una asignación sea compatible con el tipo de variable
        /// </summary>
        /// <param name="tipoVariable">Tipo de la variable</param>
        /// <param name="valorToken">Token con el valor a asignar</param>
        /// <param name="linea">Línea donde ocurre la asignación</param>
        /// <param name="columna">Columna donde ocurre la asignación</param>
        private void ValidarAsignacion(string tipoVariable, Token valorToken, int linea, int columna)
        {
            // Determinar el tipo del valor a asignar
            string tipoValor = valorToken.Tipo switch
            {
                "CADENA" => "string",
                "CARACTER" => "char",
                "ENTERO" => "int",
                "DECIMAL" => "double",
                "IDENTIFICADOR" => ObtenerTipoVariable(valorToken.Valor),
                _ => "desconocido"
            };

            // Verificar compatibilidad de tipos
            if (!EsAsignacionValida(tipoVariable, tipoValor))
            {
                errores.Add(new Error("Semántico",
                    $"Incompatibilidad de tipos: no se puede asignar {valorToken.Valor} (tipo {tipoValor}) a {tipoVariable}",
                    linea, columna));
            }
        }

        /// <summary>
        /// Obtiene el tipo de una variable previamente declarada
        /// </summary>
        /// <param name="nombreVariable">Nombre de la variable a buscar</param>
        /// <returns>Tipo de la variable o "desconocido" si no existe</returns>
        private string ObtenerTipoVariable(string nombreVariable)
        {
            return variablesDeclaradas.TryGetValue(nombreVariable, out string tipo) ? tipo : "desconocido";
        }

        /// <summary>
        /// Determina si una asignación entre tipos es válida
        /// </summary>
        /// <param name="tipoVariable">Tipo de la variable</param>
        /// <param name="tipoValor">Tipo del valor a asignar</param>
        /// <returns>True si la asignación es válida</returns>
        private bool EsAsignacionValida(string tipoVariable, string tipoValor)
        {
            return (tipoVariable, tipoValor) switch
            {
                // Strings solo aceptan strings
                ("string", "string") => true,

                // Chars solo aceptan chars
                ("char", "char") => true,

                // Ints solo aceptan ints
                ("int", "int") => true,

                // Floats aceptan ints y floats
                ("float", "int") => true,
                ("float", "float") => true,

                // Doubles aceptan ints, floats y doubles
                ("double", "int") => true,
                ("double", "float") => true,
                ("double", "double") => true,

                // Cualquier otro caso es inválido
                _ => false
            };
        }

        /// <summary>
        /// Determina si un token representa una declaración de tipo
        /// </summary>
        /// <param name="tipoToken">Tipo de token a verificar</param>
        /// <returns>True si es una declaración de tipo</returns>
        private bool EsTipoDeclaracion(string tipoToken)
        {
            return tipoToken == "INT" || tipoToken == "STRING" || tipoToken == "CHAR" ||
                   tipoToken == "FLOAT" || tipoToken == "DOUBLE";
        }
    }
}