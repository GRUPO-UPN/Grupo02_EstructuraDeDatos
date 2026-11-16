using System;
using System.Collections.Generic;
using System.Globalization;

namespace BoticaMarFarmaTransaccional.Pilas
{
    // Nodo genérico enlazado usado internamente por la pila de conversión y evaluación postfija.
    internal class NodoPila<T>
    {
        public T Dato { get; set; }
        public NodoPila<T>? Siguiente { get; set; }

        public NodoPila(T dato)
        {
            Dato = dato;
            Siguiente = null;
        }
    }

    // Pila dinámica simple para operar con tokens (operadores y operandos) en tiempo O(1).
    internal class PilaSimple<T>
    {
        private NodoPila<T>? cima;

        public bool EstaVacia => cima == null;

        public void Push(T dato)
        {
            NodoPila<T> nuevo = new NodoPila<T>(dato);
            nuevo.Siguiente = cima;
            cima = nuevo;
        }

        public T Pop()
        {
            if (cima == null) throw new InvalidOperationException("Pila vacía.");
            T dato = cima.Dato;
            cima = cima.Siguiente;
            return dato;
        }

        public T Peek()
        {
            if (cima == null) throw new InvalidOperationException("Pila vacía.");
            return cima.Dato;
        }
    }

    // Conversor de expresiones Infijas a Postfijas y evaluador aritmético mediante Pila LIFO.
    // Aplica el tema de notaciones (Sílabo Tema 4.3) para validar descuentos e IGV en ventas.
    public static class CalculadoraPostfija
    {
        // Convierte una expresión infija separada por espacios (ej. "( 100 - 10 ) * 1.18") a notación postfija.
        public static string ConvertirInfijaAPostfija(string expresionInfija)
        {
            string[] tokens = expresionInfija.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            List<string> salida = new List<string>();
            PilaSimple<string> pilaOperadores = new PilaSimple<string>();

            foreach (string token in tokens)
            {
                if (decimal.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                {
                    // Los números pasan directamente a la salida postfija.
                    salida.Add(token);
                }
                else if (token == "(")
                {
                    pilaOperadores.Push(token);
                }
                else if (token == ")")
                {
                    // Desapila operadores hasta encontrar el paréntesis de apertura correspondiente.
                    while (!pilaOperadores.EstaVacia && pilaOperadores.Peek() != "(")
                    {
                        salida.Add(pilaOperadores.Pop());
                    }
                    if (!pilaOperadores.EstaVacia && pilaOperadores.Peek() == "(")
                    {
                        pilaOperadores.Pop();
                    }
                }
                else
                {
                    // Respeta la precedencia de operadores (*, / antes que +, -).
                    while (!pilaOperadores.EstaVacia && ObtenerPrecedencia(pilaOperadores.Peek()) >= ObtenerPrecedencia(token))
                    {
                        salida.Add(pilaOperadores.Pop());
                    }
                    pilaOperadores.Push(token);
                }
            }

            while (!pilaOperadores.EstaVacia)
            {
                salida.Add(pilaOperadores.Pop());
            }

            return string.Join(" ", salida);
        }

        // Evalúa una expresión ya convertida a notación postfija apilando y operando valores numéricos.
        public static decimal EvaluarPostfija(string expresionPostfija)
        {
            string[] tokens = expresionPostfija.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            PilaSimple<decimal> pilaValores = new PilaSimple<decimal>();

            foreach (string token in tokens)
            {
                if (decimal.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal numero))
                {
                    pilaValores.Push(numero);
                }
                else
                {
                    // En postfija el primer Pop es el operando derecho y el segundo es el izquierdo.
                    decimal b = pilaValores.Pop();
                    decimal a = pilaValores.Pop();

                    decimal resultado = token switch
                    {
                        "+" => a + b,
                        "-" => a - b,
                        "*" => a * b,
                        "/" => b != 0 ? a / b : 0,
                        _ => 0
                    };
                    pilaValores.Push(resultado);
                }
            }

            return pilaValores.EstaVacia ? 0m : pilaValores.Pop();
        }

        private static int ObtenerPrecedencia(string op)
        {
            if (op == "*" || op == "/") return 2;
            if (op == "+" || op == "-") return 1;
            return 0;
        }
    }
}
