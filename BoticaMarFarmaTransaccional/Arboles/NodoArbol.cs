namespace BoticaMarFarmaTransaccional.Arboles
{
    // Nodo genérico de un Árbol Binario de Búsqueda (ABB).
    // Almacena el dato y las referencias a sus subárboles izquierdo y derecho.
    public class NodoArbol<T>
    {
        public T Dato { get; set; }
        public NodoArbol<T>? Izquierda { get; set; }
        public NodoArbol<T>? Derecha { get; set; }

        public NodoArbol(T dato)
        {
            Dato = dato;
            Izquierda = null;
            Derecha = null;
        }
    }
}
