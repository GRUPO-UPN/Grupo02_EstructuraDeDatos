namespace BoticaMarFarmaTransaccional.Colas
{
    // Nodo genérico simplemente enlazado utilizado por la estructura Cola (FIFO).
    public class NodoCola<T>
    {
        public T Dato { get; set; }
        public NodoCola<T>? Siguiente { get; set; }

        public NodoCola(T dato)
        {
            Dato = dato;
            Siguiente = null;
        }
    }
}
