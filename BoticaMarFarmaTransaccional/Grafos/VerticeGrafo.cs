using System.Collections.Generic;
using BoticaMarFarmaTransaccional.Entidades;

namespace BoticaMarFarmaTransaccional.Grafos
{
    // Vértice del grafo que encapsula un punto de la red MarFarma y su lista de adyacencia.
    public class VerticeGrafo
    {
        public PuntoRed Dato { get; set; }
        public List<AristaGrafo> Adyacentes { get; }

        public VerticeGrafo(PuntoRed dato)
        {
            Dato = dato;
            Adyacentes = new List<AristaGrafo>();
        }

        public override string ToString() => Dato.ToString();
    }
}
