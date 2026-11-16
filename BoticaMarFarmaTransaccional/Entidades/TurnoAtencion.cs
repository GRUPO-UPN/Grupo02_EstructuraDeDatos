using System;

namespace BoticaMarFarmaTransaccional.Entidades
{
    // Representa un turno en la cola FIFO de atención al paciente (mostrador o Agente BCP).
    // Adapta la entidad de turno del módulo de Colas al cliente registrado del sistema.
    public class TurnoAtencion
    {
        public int NumeroTurno { get; set; }
        public Cliente Paciente { get; set; }
        public string TipoServicio { get; set; }
        public DateTime HoraLlegada { get; set; }

        // Etiqueta compacta para dibujar el nodo en el diagrama visual de la cola.
        public string EtiquetaCorta => $"#{NumeroTurno:D2}";

        public TurnoAtencion(int numeroTurno, Cliente paciente, string tipoServicio)
        {
            NumeroTurno = numeroTurno;
            Paciente = paciente;
            TipoServicio = tipoServicio;
            HoraLlegada = DateTime.Now;
        }

        public override string ToString()
        {
            return $"#{NumeroTurno:D2} - {Paciente.NombreCompleto} | {TipoServicio} (Llegó: {HoraLlegada:HH:mm:ss})";
        }
    }
}
