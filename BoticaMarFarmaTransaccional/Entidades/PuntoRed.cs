namespace BoticaMarFarmaTransaccional.Entidades
{
    // Representa una sede o punto de entrega dentro del grafo logístico de Botica MarFarma.
    public class PuntoRed
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; } // Almacén Central, Sucursal MarFarma, Agente BCP, Centro Médico

        public PuntoRed(string codigo, string nombre, string tipo)
        {
            Codigo = codigo;
            Nombre = nombre;
            Tipo = tipo;
        }

        public override string ToString()
        {
            return $"[{Codigo}] {Nombre} ({Tipo})";
        }
    }
}
