using System;

namespace BoticaMarFarmaTransaccional.Entidades
{
    // Producto del catálogo e inventario de la botica.
    // Implementa IComparable<Producto> para poder ordenarse en el Árbol Binario de Búsqueda (ABB).
    public class Producto : IComparable<Producto>
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int Stock { get; set; }
        public string Categoria { get; set; }

        public Producto(string codigo, string nombre, decimal precioUnitario, int stock = 20, string categoria = "Medicamento")
        {
            Codigo = codigo;
            Nombre = nombre;
            PrecioUnitario = precioUnitario;
            Stock = stock;
            Categoria = categoria;
        }

        // Compara dos productos por su código para ubicarlos a la izquierda (< 0) o derecha (> 0) en el ABB.
        public int CompareTo(Producto? otro)
        {
            if (otro == null)
            {
                return 1;
            }
            return string.Compare(this.Codigo, otro.Codigo, StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            return $"[{Codigo}] {Nombre} - S/ {PrecioUnitario:F2} (Stock: {Stock})";
        }
    }
}
