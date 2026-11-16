using System;
using System.Collections.Generic;

namespace BoticaMarFarmaTransaccional.Colas
{
    // Cola genérica FIFO (First-In, First-Out) implementada con nodos enlazados.
    // Mantiene punteros al frente (primero en salir) y al final (último en entrar) para operar en O(1).
    public class Cola<T>
    {
        private NodoCola<T>? frente;
        private NodoCola<T>? final;
        private int contador;

        public Cola()
        {
            frente = null;
            final = null;
            contador = 0;
        }

        // Indica si no hay elementos esperando en la cola.
        public bool EstaVacia() => frente == null;

        // Devuelve la cantidad actual de elementos en espera.
        public int Longitud() => contador;

        // Encola un nuevo elemento al final de la estructura en tiempo constante O(1).
        public void Enqueue(T dato)
        {
            NodoCola<T> nuevoNodo = new NodoCola<T>(dato);

            if (EstaVacia())
            {
                // Si la cola estaba vacía, frente y final apuntan al mismo nodo inicial.
                frente = nuevoNodo;
                final = nuevoNodo;
            }
            else
            {
                // El antiguo nodo final enlaza hacia el nuevo y el puntero final avanza.
                final!.Siguiente = nuevoNodo;
                final = nuevoNodo;
            }

            contador++;
        }

        // Desencola y devuelve el elemento ubicado en el frente en tiempo constante O(1).
        public T Dequeue()
        {
            if (EstaVacia())
            {
                throw new InvalidOperationException("La cola se encuentra vacía.");
            }

            T dato = frente!.Dato;
            frente = frente.Siguiente;

            // Si al retirar el frente la cola quedó vacía, se limpia también el puntero final.
            if (frente == null)
            {
                final = null;
            }

            contador--;
            return dato;
        }

        // Consulta el elemento del frente sin retirarlo de la cola en O(1).
        public T Front()
        {
            if (EstaVacia())
            {
                throw new InvalidOperationException("La cola se encuentra vacía.");
            }

            return frente!.Dato;
        }

        // Reinicia la cola liberando las referencias a frente y final.
        public void Clear()
        {
            frente = null;
            final = null;
            contador = 0;
        }

        // Recorre secuencialmente la cola desde frente hasta final en O(n) para inspección visual.
        public List<T> ObtenerElementos()
        {
            List<T> elementos = new List<T>();
            NodoCola<T>? actual = frente;

            while (actual != null)
            {
                elementos.Add(actual.Dato);
                actual = actual.Siguiente;
            }

            return elementos;
        }
    }
}
