
# 🛠️ Compilador en C# - Proyecto Final

Este es un **compilador implementado en C# con Windows Forms**, desarrollado como proyecto final para un lenguaje imperativo personalizado. El sistema realiza análisis léxico, sintáctico y semántico, con una interfaz gráfica básica para facilitar la interacción del usuario.

---

## 📁 Estructura del Proyecto

- `AnalizadorLexico.cs` → Tokeniza el código fuente.
- `AnalizadorSintactico.cs` → Verifica la estructura gramatical del programa.
- `AnalizadorSemantico.cs` → Revisa errores semánticos como uso indebido de variables.
- `Token.cs` → Define la estructura y tipos de tokens.
- `Error.cs` → Modelo de errores con tipo, línea y columna.
- `ErroresCompilador.txt` → Archivo de salida con la lista de errores detectados.
- `Form1.cs / Form1.Designer.cs / Form1.resx` → Interfaz gráfica del compilador (Windows Forms).
- `Program.cs` → Punto de entrada de la aplicación.
- `compilador final.csproj` / `compilador final.sln` → Archivos del proyecto y solución de Visual Studio.

---

## ✅ Funcionalidades

- **Análisis léxico** con identificación de palabras clave, identificadores, números y símbolos.
- **Análisis sintáctico** con validación de estructuras como:
  - Declaraciones de variables
  - Asignaciones
  - Condicionales (`if` / `else`)
  - Ciclos (`for` / `while`)
  - Instrucciones de salida (`print`)
- **Ámbitos** gestionados por pila de diccionarios.
- **Interfaz gráfica** con Windows Forms para cargar y analizar código.
- en cada una está señalado cuando se usa poo que se usa en su mayoria en los analizadores semanticos y sintactico y tambien en estos son los que se usa programacion funcional
---
## Diagrama
![Imagen de WhatsApp 2025-05-28 a las 23 35 43_6346bfbf](https://github.com/user-attachments/assets/acc52590-8494-465c-a557-02b0ae78cf61)

---

## 🧪 Ejemplo de Código Compatible

class prueba
{

  int x = 5;

  if (x > 2){
   print("x es mayor que 2");
  }


  for(int i=0; i < 5 ; i++){

   print("i es mayor que 5");

  }

}

---


Reglas del compilador

1. debe comenzar con la palabra reservada class y su respectivo nombre este debe llevar llaves para cerrar.
2. todo if se abre y se cierra con llaves, debe de arrojar error en caso que no se cierre y además todo if debe tener contenido dentro de los paréntesis, en caso contrario arroja error, además el if debe validar la existencia de las variables.
3. el sistema debe permitir solo este tipo de variables reservadas: int, string, char , float y double, la variable se puede declarar en cualquier parte del código pero no puede ser asignada antes de declararla.
4. la asignación de las variables solo se puede hacer con el símbolo "=", la asignación siempre debe ser de izq a derecha.
5. solo permite ciclos for y while, la sintaxis del for debe ser: for (int i; condición; i++) y la del while entre paréntesis, ambos deben de llevar llaves para abrir y cerrar, además debe validar la existencia de las variables.
6. los condicionales de lógica deben de ser && and y || or
7. los operadores matemáticos son +, -, * y /.
8. solo se debe permitir operadores matemáticos con variables tipo int, float y double en caso contrario arroja error.
9. toda linea de codigo debe terminar con ";" excepto las que van con { }.
10. para imprimir las variables solo se permite la palabra reservada print, esta debe ir con () y adentro el mensaje entre comillas "".
11. el compilador debe permitir operadores lógicos >,<, <=, >=, ==, != en caso contrario arroja error.

---

## 🖥️ Cómo Ejecutar

1. Abre `compilador final.sln` en Visual Studio.
2. Ejecuta el proyecto (`F5` o botón "Iniciar").
3. Usa la interfaz para escribir o cargar código fuente.
4. Presiona el botón para compilar y ver los errores en pantalla o en `ErroresCompilador.txt`.

--- Reflexión 
Lo mas facil del proyecto fue aplicar los conceptos basicos de POO aprendidos durante el curso , dado a que durantte varios meses tuvimos tiempo para aprender sus conceptos y funcionamiento.
Lo mas dificil fue aprender la estructura de funcionamiento de un compilador y el tema de programacion funcional.
Como se mencionó anteriormente se tuvo que aprender el funcionamiento de un compilador y sus conceptos clave, este fue el conocimiento adquirida¿o principalmente, ademas de que tuvimos que perfeccionar nuestro uso de los conceptos aprendidos durante clase.


