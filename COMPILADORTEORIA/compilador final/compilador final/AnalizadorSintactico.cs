using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiladorFinal
{
   
    /// Clase que implementa el analizador sintáctico para el compilador
    public class AnalizadorSintactico
    {
        // Lista de tokens a analizar
        private List<Token> tokens;
        // Posición actual en la lista de tokens
        private int posicionActual;
        // Lista de errores encontrados durante el análisis
        private List<Error> errores;
        // Pila de ámbitos para manejo de variables (scope)
        private Stack<Dictionary<string, string>> ambitos;

        
        /// Constructor del analizador sintáctico
        
        public AnalizadorSintactico()
        {
            errores = new List<Error>();
            ambitos = new Stack<Dictionary<string, string>>();
        }

      
        /// Método principal que inicia el análisis sintáctico
    
        /// <param name="tokens">Lista de tokens a analizar</param>
        /// <returns>Lista de errores encontrados</returns>
        public List<Error> Analizar(List<Token> tokens)
        {
            this.tokens = tokens;
            posicionActual = 0;
            errores.Clear();
            ambitos.Clear();

            try
            {
                // Iniciar ámbito global
                EntrarAmbito();
                AnalizarClase(); // Punto de entrada del análisis
                SalirAmbito();
            }
            catch (Exception ex)
            {
                errores.Add(new Error("Sintáctico", $"Error inesperado: {ex.Message}", 0, 0));
            }

            return errores;
        }

        // ==============================================
        // MÉTODOS PARA MANEJO DE ÁMBITOS (SCOPE)
        // ==============================================

        /// <summary>
        /// Crea y entra a un nuevo ámbito
        /// </summary>
        private void EntrarAmbito()
        {
            ambitos.Push(new Dictionary<string, string>());
        }

        /// <summary>
        /// Sale del ámbito actual
        /// </summary>
        private void SalirAmbito()
        {
            ambitos.Pop();
        }

        /// <summary>
        /// Propiedad para acceder al ámbito actual
        /// </summary>
        private Dictionary<string, string> AmbitoActual => ambitos.Peek();

        // ==============================================
        // MÉTODOS PARA ANÁLISIS DE ESTRUCTURAS PRINCIPALES
        // ==============================================

        /// <summary>
        /// Analiza la estructura de una clase
        /// </summary>
        private void AnalizarClase()
        {
            int contadorLlaves = 0; // Contador para verificar balance de llaves

            if (!Consumir("CLASS", "Se esperaba la palabra reservada 'class'"))
                return;

            if (!Consumir("IDENTIFICADOR", "Se esperaba el nombre de la clase"))
                return;

            if (!Consumir("LLAVE_IZQ", "Se esperaba '{' después del nombre de la clase"))
                return;

            contadorLlaves++; // Incrementar por llave de apertura

            // Nuevo ámbito para el contenido de la clase
            EntrarAmbito();

            // Analizar todas las declaraciones dentro de la clase
            while (posicionActual < tokens.Count && !EsTipoActual("LLAVE_DER"))
            {
                AnalizarDeclaracion();
            }

            if (!Consumir("LLAVE_DER", "Se esperaba '}' para cerrar la clase"))
                return;

            contadorLlaves--; // Decrementar por llave de cierre

            // Verificar balance de llaves
            if (contadorLlaves != 0)
            {
                AgregarError("Sintáctico", $"Llaves desbalanceadas. Faltan {(contadorLlaves > 0 ? "cerrar" : "abrir")} {Math.Abs(contadorLlaves)} llave(s)",
                            tokens[posicionActual].Linea, tokens[posicionActual].Columna);
            }

            SalirAmbito();

            // Verificar que no haya código después del cierre de clase
            if (posicionActual < tokens.Count)
            {
                AgregarError("Sintáctico", "Código después del cierre de la clase",
                            tokens[posicionActual].Linea, tokens[posicionActual].Columna);
            }
        }

        /// <summary>
        /// Analiza bloques de código entre llaves
        /// </summary>
        /// <param name="contadorLlaves">Referencia al contador de llaves</param>
        private void AnalizarBloque(ref int contadorLlaves)
        {
            if (!Consumir("LLAVE_IZQ", "Se esperaba '{' para iniciar el bloque"))
                return;

            contadorLlaves++;

            // Analizar todas las declaraciones dentro del bloque
            while (posicionActual < tokens.Count && !EsTipoActual("LLAVE_DER"))
            {
                AnalizarDeclaracion();
            }

            if (!Consumir("LLAVE_DER", "Se esperaba '}' para cerrar el bloque"))
                return;

            contadorLlaves--;
        }

        /// <summary>
        /// Analiza diferentes tipos de declaraciones
        /// </summary>
        private void AnalizarDeclaracion()
        {
            if (EsTipoActual("IF"))
            {
                AnalizarIf();
            }
            else if (EsTipoActual("FOR"))
            {
                AnalizarFor();
            }
            else if (EsTipoActual("WHILE"))
            {
                AnalizarWhile();
            }
            else if (EsTipoDeclaracionVariable())
            {
                AnalizarDeclaracionVariable();
            }
            else if (EsTipoActual("PRINT"))
            {
                AnalizarPrint();
            }
            else if (EsTipoActual("IDENTIFICADOR"))
            {
                AnalizarAsignacion();
            }
            else
            {
                AgregarError("Sintáctico", $"Declaración no válida: {tokens[posicionActual].Valor}",
                            tokens[posicionActual].Linea, tokens[posicionActual].Columna);
                Avanzar();
            }
        }

        // ==============================================
        // MÉTODOS PARA ANÁLISIS DE ESTRUCTURAS DE CONTROL
        // ==============================================

        /// <summary>
        /// Analiza estructuras if
        /// </summary>
        private void AnalizarIf()
        {
            if (!Consumir("IF", "Se esperaba 'if'"))
                return;

            if (!Consumir("PARENTESIS_IZQ", "Se esperaba '(' después de 'if'"))
                return;

            // Analizar la condición del if
            AnalizarCondicion();

            if (!Consumir("PARENTESIS_DER", "Se esperaba ')' después de la condición"))
                return;

            if (!Consumir("LLAVE_IZQ", "Se esperaba '{' para iniciar el bloque"))
                return;

            // Analizar el cuerpo del if
            while (posicionActual < tokens.Count && !EsTipoActual("LLAVE_DER"))
            {
                AnalizarDeclaracion();
            }

            if (!Consumir("LLAVE_DER", "Se esperaba '}' para cerrar el bloque"))
                return;
        }

        /// <summary>
        /// Analiza estructuras for
        /// </summary>
        private void AnalizarFor()
        {
            if (!Consumir("FOR", "Se esperaba 'for'"))
                return;

            if (!Consumir("PARENTESIS_IZQ", "Se esperaba '(' después de 'for'"))
                return;

            // --- Fase 1: Inicialización ---
            int inicioInicializacion = posicionActual;
            if (Consumir("INT", "Se esperaba 'int' en la declaración del for"))
            {
                string nombreVariable = tokens[posicionActual].Valor;
                if (!Consumir("IDENTIFICADOR", "Se esperaba identificador en el for"))
                    return;

                // Registrar variable del for en el ámbito actual
                AmbitoActual[nombreVariable] = "int";

                // Manejar asignación inicial (i = 0)
                if (EsTipoActual("ASIGNACION"))
                {
                    Avanzar();
                    int inicioAsignacion = posicionActual;
                    AnalizarExpresion();
                    VerificarVariablesEnRango(inicioAsignacion, posicionActual - 1);
                }
            }
            else if (EsTipoActual("IDENTIFICADOR"))
            {
                // Para variables ya declaradas
                VerificarVariableDeclarada(tokens[posicionActual]);
                Avanzar();
                if (EsTipoActual("ASIGNACION"))
                {
                    Avanzar();
                    int inicioAsignacion = posicionActual;
                    AnalizarExpresion();
                    VerificarVariablesEnRango(inicioAsignacion, posicionActual - 1);
                }
            }

            if (!Consumir("PUNTO_COMA", "Se esperaba ';' después de la inicialización"))
                return;

            // --- Fase 2: Condición ---
            int inicioCondicion = posicionActual;
            if (!EsTipoActual("PUNTO_COMA")) // Permitir condición vacía
            {
                AnalizarCondicion();
            }
            VerificarVariablesEnRango(inicioCondicion, posicionActual - 1);

            if (!Consumir("PUNTO_COMA", "Se esperaba ';' después de la condición"))
                return;

            // --- Fase 3: Incremento ---
            int inicioIncremento = posicionActual;
            if (!EsTipoActual("PARENTESIS_DER")) // Permitir incremento vacío
            {
                if (Consumir("IDENTIFICADOR", "Se esperaba identificador en el incremento"))
                {
                    VerificarVariableDeclarada(tokens[posicionActual - 1]);

                    if (EsTipoActual("SUMA"))
                    {
                        Avanzar();
                        if (EsTipoActual("SUMA")) // i++
                        {
                            Avanzar();
                        }
                        else if (EsTipoActual("ASIGNACION")) // i +=
                        {
                            Avanzar();
                            AnalizarExpresion();
                            VerificarVariablesEnRango(inicioIncremento, posicionActual - 1);
                        }
                    }
                }
            }

            if (!Consumir("PARENTESIS_DER", "Se esperaba ')' después del incremento"))
                return;

            if (!Consumir("LLAVE_IZQ", "Se esperaba '{' para el cuerpo del for"))
                return;

            // Nuevo ámbito para el cuerpo del for
            EntrarAmbito();

            while (posicionActual < tokens.Count && !EsTipoActual("LLAVE_DER"))
            {
                AnalizarDeclaracion();
            }

            SalirAmbito();

            if (!Consumir("LLAVE_DER", "Se esperaba '}' para cerrar el bloque for"))
                return;
        }

        /// <summary>
        /// Analiza estructuras while
        /// </summary>
        private void AnalizarWhile()
        {
            if (!Consumir("WHILE", "Se esperaba 'while'"))
                return;

            if (!Consumir("PARENTESIS_IZQ", "Se esperaba '(' después de 'while'"))
                return;

            // Verificar variables en la condición
            int inicioCondicion = posicionActual;
            AnalizarCondicion();
            int finCondicion = posicionActual - 1;

            // Verificar variables usadas en la condición
            VerificarVariablesEnCondicion(inicioCondicion, finCondicion);

            if (!Consumir("PARENTESIS_DER", "Se esperaba ')' después de la condición"))
                return;

            if (!Consumir("LLAVE_IZQ", "Se esperaba '{' para el cuerpo del while"))
                return;

            // Nuevo ámbito para el cuerpo del while
            EntrarAmbito();

            while (posicionActual < tokens.Count && !EsTipoActual("LLAVE_DER"))
            {
                AnalizarDeclaracion();
            }

            SalirAmbito();

            if (!Consumir("LLAVE_DER", "Se esperaba '}' para cerrar el bloque while"))
                return;
        }

        // ==============================================
        // MÉTODOS PARA ANÁLISIS DE EXPRESIONES Y CONDICIONES
        // ==============================================

        /// <summary>
        /// Analiza condiciones lógicas
        /// </summary>
        private void AnalizarCondicion()
        {
            // Estado inicial
            bool esperandoOperando = true;

            while (posicionActual < tokens.Count && !EsTipoActual("PARENTESIS_DER"))
            {
                if (esperandoOperando)
                {
                    // Esperamos un operando (identificador, número o paréntesis)
                    if (EsTipoActual("PARENTESIS_IZQ"))
                    {
                        Avanzar();
                        AnalizarCondicion(); // Recursión para paréntesis anidados
                        if (!Consumir("PARENTESIS_DER", "Se esperaba ')' después de la expresión"))
                            return;
                    }
                    else if (EsTipoActual("IDENTIFICADOR") || EsTipoActual("ENTERO") || EsTipoActual("DECIMAL"))
                    {
                        if (EsTipoActual("IDENTIFICADOR"))
                        {
                            VerificarVariableDeclarada(tokens[posicionActual]);
                        }
                        Avanzar();
                        esperandoOperando = false; // Ahora esperamos un operador
                    }
                    else
                    {
                        AgregarError("Sintáctico", $"Se esperaba un operando (variable o valor), encontrado: '{tokens[posicionActual].Valor}'",
                                    tokens[posicionActual].Linea, tokens[posicionActual].Columna);
                        Avanzar();
                    }
                }
                else
                {
                    // Esperamos un operador de comparación o lógico
                    if (EsOperadorComparacion(tokens[posicionActual].Tipo) ||
                        EsTipoActual("AND_LOGICO") || EsTipoActual("OR_LOGICO"))
                    {
                        Avanzar();
                        esperandoOperando = true; // Después de operador, esperamos operando
                    }
                    else if (!EsTipoActual("PARENTESIS_DER"))
                    {
                        AgregarError("Sintáctico", $"Se esperaba un operador de comparación (como <, >, <=, >=), encontrado: '{tokens[posicionActual].Valor}'",
                                    tokens[posicionActual].Linea, tokens[posicionActual].Columna);
                        Avanzar();
                    }
                }
            }
        }

        /// <summary>
        /// Analiza declaraciones de variables
        /// </summary>
        private void AnalizarDeclaracionVariable()
        {
            string tipo = tokens[posicionActual].Valor;
            Avanzar();

            if (!Consumir("IDENTIFICADOR", "Se esperaba nombre de variable"))
                return;

            string nombreVariable = tokens[posicionActual - 1].Valor;

            // Verificar si la variable ya fue declarada en este ámbito
            if (AmbitoActual.ContainsKey(nombreVariable))
            {
                AgregarError("Semántico", $"La variable '{nombreVariable}' ya fue declarada en este ámbito",
                            tokens[posicionActual - 1].Linea, tokens[posicionActual - 1].Columna);
            }
            else
            {
                // Registrar la nueva variable
                AmbitoActual.Add(nombreVariable, tipo);
            }

            // Manejar asignación opcional
            if (EsTipoActual("ASIGNACION"))
            {
                Avanzar();

                // Manejar asignaciones especiales para cada tipo
                if (tipo.ToLower() == "char" && EsTipoActual("CARACTER"))
                {
                    Avanzar(); // Aceptar cualquier carácter (incluido vacío)
                }
                else if (tipo.ToLower() == "string" && EsTipoActual("CADENA"))
                {
                    Avanzar(); // Aceptar cualquier cadena
                }
                else
                {
                    AnalizarExpresion();
                }
            }

            if (!Consumir("PUNTO_COMA", "Se esperaba ';' al final de la declaración"))
                return;
        }

        /// <summary>
        /// Analiza asignaciones de variables
        /// </summary>
        private void AnalizarAsignacion()
        {
            // Guardar información de la variable para mensajes de error
            Token variable = tokens[posicionActual];
            string nombreVariable = variable.Valor;
            Avanzar(); // Consumir el identificador

            // Verificar si la variable está declarada
            bool variableDeclarada = false;
            foreach (var ambito in ambitos)
            {
                if (ambito.ContainsKey(nombreVariable))
                {
                    variableDeclarada = true;
                    break;
                }
            }

            if (!variableDeclarada)
            {
                AgregarError("Semántico", $"La variable '{nombreVariable}' no ha sido declarada",
                            variable.Linea, variable.Columna);
            }

            // Verificar operador de asignación
            if (!Consumir("ASIGNACION", "Se esperaba '=' en la asignación"))
                return;

            // Analizar la expresión completa
            AnalizarExpresion();

            // Verificar punto y coma final
            if (!Consumir("PUNTO_COMA", "Se esperaba ';' al final de la asignación"))
                return;
        }

        /// <summary>
        /// Analiza sentencias print
        /// </summary>
        private void AnalizarPrint()
        {
            if (!Consumir("PRINT", "Se esperaba 'print'"))
                return;

            if (!Consumir("PARENTESIS_IZQ", "Se esperaba '(' después de 'print'"))
                return;

            // Aceptar cadena vacía o con contenido
            if (!EsTipoActual("CADENA"))
            {
                AgregarError("Sintáctico", "Se esperaba cadena de texto entre comillas",
                            tokens[posicionActual].Linea, tokens[posicionActual].Columna);
                return;
            }

            // Consumir la cadena (puede ser vacía "")
            Avanzar();

            if (!Consumir("PARENTESIS_DER", "Se esperaba ')' después del mensaje"))
                return;

            if (!Consumir("PUNTO_COMA", "Se esperaba ';' al final de la sentencia print"))
                return;
        }

        /// <summary>
        /// Analiza expresiones matemáticas
        /// </summary>
        private void AnalizarExpresion()
        {
            AnalizarTermino();

            // Manejar operaciones matemáticas
            while (posicionActual < tokens.Count &&
                  (EsTipoActual("SUMA") || EsTipoActual("RESTA") ||
                   EsTipoActual("MULTIPLICACION") || EsTipoActual("DIVISION")))
            {
                Avanzar();
                AnalizarTermino();
            }
        }

        /// <summary>
        /// Analiza términos de expresiones
        /// </summary>
        private void AnalizarTermino()
        {
            if (EsTipoActual("IDENTIFICADOR"))
            {
                VerificarVariableDeclarada(tokens[posicionActual]);
                Avanzar();
            }
            else if (EsTipoActual("ENTERO") || EsTipoActual("DECIMAL"))
            {
                Avanzar();
            }
            else if (EsTipoActual("CARACTER"))
            {
                // Aceptar caracteres vacíos o con contenido
                Avanzar();
            }
            else if (EsTipoActual("PARENTESIS_IZQ"))
            {
                Avanzar();
                AnalizarExpresion();
                if (!Consumir("PARENTESIS_DER", "Se esperaba ')' después de la expresión"))
                    return;
            }
            else if (!EsOperadorComparacion(tokens[posicionActual].Tipo))
            {
                AgregarError("Sintáctico",
                            $"Término no válido: '{tokens[posicionActual].Valor}'",
                            tokens[posicionActual].Linea,
                            tokens[posicionActual].Columna);
                Avanzar();
            }
        }

        // ==============================================
        // MÉTODOS AUXILIARES
        // ==============================================

        /// <summary>
        /// Verifica variables en un rango de tokens
        /// </summary>
        /// <param name="inicio">Índice de inicio</param>
        /// <param name="fin">Índice de fin</param>
        private void VerificarVariablesEnRango(int inicio, int fin)
        {
            for (int i = inicio; i <= fin; i++)
            {
                if (tokens[i].Tipo == "IDENTIFICADOR")
                {
                    VerificarVariableDeclarada(tokens[i]);
                }
            }
        }

        /// <summary>
        /// Verifica si una variable está declarada
        /// </summary>
        /// <param name="token">Token con la variable a verificar</param>
        private void VerificarVariableDeclarada(Token token)
        {
            if (token.Tipo != "IDENTIFICADOR") return;

            string nombreVariable = token.Valor;
            bool declarada = false;

            // Buscar en todos los ámbitos (del más interno al más externo)
            foreach (var ambito in ambitos)
            {
                if (ambito.ContainsKey(nombreVariable))
                {
                    declarada = true;
                    break;
                }
            }

            if (!declarada)
            {
                AgregarError("Semántico", $"La variable '{nombreVariable}' no ha sido declarada",
                            token.Linea, token.Columna);
            }
        }

        /// <summary>
        /// Verifica variables en una condición
        /// </summary>
        /// <param name="inicio">Índice de inicio</param>
        /// <param name="fin">Índice de fin</param>
        private void VerificarVariablesEnCondicion(int inicio, int fin)
        {
            for (int i = inicio; i <= fin; i++)
            {
                if (tokens[i].Tipo == "IDENTIFICADOR"))
                {
                string nombreVariable = tokens[i].Valor;
                bool variableDeclarada = false;

                // Buscar en todos los ámbitos (del más interno al más externo)
                foreach (var ambito in ambitos)
                {
                    if (ambito.ContainsKey(nombreVariable))
                    {
                        variableDeclarada = true;
                        break;
                    }
                }

                if (!variableDeclarada)
                {
                    AgregarError("Semántico", $"La variable '{nombreVariable}' no ha sido declarada",
                                tokens[i].Linea, tokens[i].Columna);
                }
            }
        }
        }

        /// <summary>
        /// Identifica operadores de comparación
        /// </summary>
        /// <param name="tipoToken">Tipo de token a verificar</param>
        /// <returns>True si es operador de comparación</returns>
        private bool EsOperadorComparacion(string tipoToken)
        {
            string[] operadoresComparacion = {
                "MENOR", "MAYOR", "MENOR_IGUAL", "MAYOR_IGUAL",
                "IGUALDAD", "DIFERENTE"
            };
            return operadoresComparacion.Contains(tipoToken);
        }

        /// <summary>
        /// Consume un token esperado
        /// </summary>
        /// <param name="tipo">Tipo de token esperado</param>
        /// <param name="mensajeError">Mensaje de error si no coincide</param>
        /// <returns>True si el token coincide</returns>
        private bool Consumir(string tipo, string mensajeError)
        {
            if (posicionActual < tokens.Count && tokens[posicionActual].Tipo == tipo)
            {
                Avanzar();
                return true;
            }

            AgregarError("Sintáctico", mensajeError, tokens[posicionActual].Linea, tokens[posicionActual].Columna);
            return false;
        }

        /// <summary>
        /// Verifica el tipo del token actual
        /// </summary>
        /// <param name="tipo">Tipo a verificar</param>
        /// <returns>True si coincide</returns>
        private bool EsTipoActual(string tipo)
        {
            return posicionActual < tokens.Count && tokens[posicionActual].Tipo == tipo;
        }

        /// <summary>
        /// Verifica si el token actual es una declaración de variable
        /// </summary>
        /// <returns>True si es declaración de variable</returns>
        private bool EsTipoDeclaracionVariable()
        {
            return EsTipoActual("INT") || EsTipoActual("FLOAT") ||
                   EsTipoActual("CHAR") || EsTipoActual("STRING");
        }

        /// <summary>
        /// Avanza al siguiente token
        /// </summary>
        private void Avanzar()
        {
            posicionActual++;
        }

        /// <summary>
        /// Agrega un error a la lista de errores
        /// </summary>
        /// <param name="tipo">Tipo de error</param>
        /// <param name="descripcion">Descripción del error</param>
        /// <param name="linea">Línea donde ocurrió</param>
        /// <param name="columna">Columna donde ocurrió</param>
        private void AgregarError(string tipo, string descripcion, int linea, int columna)
        {
            errores.Add(new Error(tipo, descripcion, linea, columna));
        }
    }
}