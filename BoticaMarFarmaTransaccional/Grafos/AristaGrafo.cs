namespace BoticaMarFarmaTransaccional.Grafos
{
    // Representa una arista ponderada en la lista de adyacencia del grafo.
    // Almacena el vértice destino y los pesos operativos (distancia en km y minutos).
    public class AristaGrafo
    {
        public VerticeGrafo Destino { get; set; }
        public double DistanciaKm { get; set; }
        public int TiempoEstimadoMin { get; set; }

        public AristaGrafo(VerticeGrafo destino, double distanciaKm, int tiempoEstimadoMin)
        {
            Destino = destino;
            DistanciaKm = distanciaKm;
            TiempoEstimadoMin = tiempoEstimadoMin;
        }
    }
}
