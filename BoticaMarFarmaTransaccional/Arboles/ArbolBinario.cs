using System;
using System.Collections.Generic;

namespace BoticaMarFarmaTransaccional.Arboles
{
    // Árbol Binario de Búsqueda (ABB) genérico para el inventario ordenado de medicamentos.
    // Todo elemento menor que el nodo actual va a la izquierda y todo elemento mayor va a la derecha.
    public class ArbolBinario<T> where T : IComparable<T>
    {
        private NodoArbol<T>? raiz;

        public NodoArbol<T>? ObtenerRaiz() => raiz;
        public int Count { get; private set; }
        public bool EstaVacio => raiz == null;

        public ArbolBinario()
        {
            raiz = null;
            Count = 0;
        }

        // Inserta un nuevo producto en el ABB; devuelve false si ya existe un código repetido.
        public bool Insertar(T dato)
        {
            bool insertado = false;
            raiz = InsertarRec(raiz, dato, ref insertado);
            if (insertado)
            {
                Count++;
            }
            return insertado;
        }

        // Método recursivo que busca un espacio nulo respetando el orden izquierda < nodo < derecha.
        private NodoArbol<T> InsertarRec(NodoArbol<T>? nodo, T dato, ref bool insertado)
        {
            if (nodo == null)
            {
                insertado = true;
                return new NodoArbol<T>(dato);
            }

            int cmp = dato.CompareTo(nodo.Dato);
            if (cmp < 0)
            {
                nodo.Izquierda = InsertarRec(nodo.Izquierda, dato, ref insertado);
            }
            else if (cmp > 0)
            {
                nodo.Derecha = InsertarRec(nodo.Derecha, dato, ref insertado);
            }
            else
            {
                // Si la comparación da 0 significa que el código ya existe y no se duplica.
                insertado = false;
            }

            return nodo;
        }

        // Busca un nodo por su clave en tiempo promedio O(log n) bajando por izquierda o derecha.
        public NodoArbol<T>? BuscarNodo(T dato)
        {
            NodoArbol<T>? actual = raiz;
            while (actual != null)
            {
                int cmp = dato.CompareTo(actual.Dato);
                if (cmp == 0)
                {
                    return actual;
                }
                actual = cmp < 0 ? actual.Izquierda : actual.Derecha;
            }
            return null;
        }

        // Elimina un nodo del árbol manteniendo el orden del ABB.
        public bool Eliminar(T dato)
        {
            bool eliminado = false;
            raiz = EliminarRec(raiz, dato, ref eliminado);
            if (eliminado)
            {
                Count--;
            }
            return eliminado;
        }

        private NodoArbol<T>? EliminarRec(NodoArbol<T>? nodo, T dato, ref bool eliminado)
        {
            if (nodo == null) return null;

            int cmp = dato.CompareTo(nodo.Dato);
            if (cmp < 0)
            {
                nodo.Izquierda = EliminarRec(nodo.Izquierda, dato, ref eliminado);
            }
            else if (cmp > 0)
            {
                nodo.Derecha = EliminarRec(nodo.Derecha, dato, ref eliminado);
            }
            else
            {
                eliminado = true;

                // Caso 1 y 2: si no tiene hijo izquierdo o derecho, se reemplaza por el otro hijo.
                if (nodo.Izquierda == null) return nodo.Derecha;
                if (nodo.Derecha == null) return nodo.Izquierda;

                // Caso 3: si tiene dos hijos, se reemplaza por el menor nodo del subárbol derecho (sucesor).
                NodoArbol<T> sucesor = ObtenerMinimo(nodo.Derecha);
                nodo.Dato = sucesor.Dato;
                bool ignorar = false;
                nodo.Derecha = EliminarRec(nodo.Derecha, sucesor.Dato, ref ignorar);
            }

            return nodo;
        }

        private static NodoArbol<T> ObtenerMinimo(NodoArbol<T> nodo)
        {
            while (nodo.Izquierda != null)
            {
                nodo = nodo.Izquierda;
            }
            return nodo;
        }

        // Recorrido InOrden (Izquierda -> Raíz -> Derecha): devuelve los productos ordenados por código.
        public List<T> RecorridoInOrden()
        {
            List<T> lista = new List<T>();
            InOrdenRec(raiz, lista);
            return lista;
        }

        private void InOrdenRec(NodoArbol<T>? nodo, List<T> lista)
        {
            if (nodo == null) return;
            InOrdenRec(nodo.Izquierda, lista);
            lista.Add(nodo.Dato);
            InOrdenRec(nodo.Derecha, lista);
        }

        // Recorrido PreOrden (Raíz -> Izquierda -> Derecha).
        public List<T> RecorridoPreOrden()
        {
            List<T> lista = new List<T>();
            PreOrdenRec(raiz, lista);
            return lista;
        }

        private void PreOrdenRec(NodoArbol<T>? nodo, List<T> lista)
        {
            if (nodo == null) return;
            lista.Add(nodo.Dato);
            PreOrdenRec(nodo.Izquierda, lista);
            PreOrdenRec(nodo.Derecha, lista);
        }

        // Recorrido PostOrden (Izquierda -> Derecha -> Raíz).
        public List<T> RecorridoPostOrden()
        {
            List<T> lista = new List<T>();
            PostOrdenRec(raiz, lista);
            return lista;
        }

        private void PostOrdenRec(NodoArbol<T>? nodo, List<T> lista)
        {
            if (nodo == null) return;
            PostOrdenRec(nodo.Izquierda, lista);
            PostOrdenRec(nodo.Derecha, lista);
            lista.Add(nodo.Dato);
        }

        // Calcula la altura máxima del árbol de forma recursiva.
        public int ObtenerAltura()
        {
            return AlturaRec(raiz);
        }

        private int AlturaRec(NodoArbol<T>? nodo)
        {
            if (nodo == null) return 0;
            return 1 + Math.Max(AlturaRec(nodo.Izquierda), AlturaRec(nodo.Derecha));
        }
    }
}
