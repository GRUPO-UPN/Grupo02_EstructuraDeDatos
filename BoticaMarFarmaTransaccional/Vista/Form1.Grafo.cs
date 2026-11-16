using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BoticaMarFarmaTransaccional.Entidades;
using BoticaMarFarmaTransaccional.Grafos;

namespace BoticaMarFarmaTransaccional
{
    public partial class Form1
    {
        // Panel principal y controles de la vista de Grafos (Red de Sedes y Rutas).
        private Panel pnlVistaGrafos = null!;
        private Label lblGrafoVertices = null!;
        private Label lblGrafoAristas = null!;
        private Label lblGrafoConexo = null!;
        private Label lblGrafoRutaResumen = null!;

        private ListBox lstGrafoAdyacencia = null!;
        private ComboBox cmbGrafoOrigen = null!;
        private ComboBox cmbGrafoDestino = null!;
        private Button btnGrafoDijkstra = null!;
        private Button btnGrafoBFS = null!;
        private Button btnGrafoDFS = null!;

        private ComboBox cmbGrafoProducto = null!;
        private Button btnGrafoDespachar = null!;

        private TextBox txtGrafoNuevoCod = null!;
        private TextBox txtGrafoNuevoNombre = null!;
        private ComboBox cmbGrafoNuevoTipo = null!;
        private Button btnGrafoAgregarVertice = null!;

        private ComboBox cmbGrafoAristaA = null!;
        private ComboBox cmbGrafoAristaB = null!;
        private NumericUpDown nudGrafoKm = null!;
        private Button btnGrafoAgregarArista = null!;

        private ListBox lstGrafoTrazas = null!;

        // Construye la vista de Grafos basada en lista de adyacencia y trazas (sin dibujos).
        private void ConstruirVistaGrafos()
        {
            pnlVistaGrafos = new Panel { Dock = DockStyle.Fill, Padding = new Padding(25), Visible = false };

            Label lblTit = new Label
            {
                Text = "RED LOGÍSTICA Y RUTAS DE DESPACHO (GRAFOS PONDERADOS)",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(25, 15),
                AutoSize = true
            };
            Label lblSub = new Label
            {
                Text = "Grafo no dirigido con lista de adyacencia: exploración BFS/DFS y cálculo de ruta más corta mediante Dijkstra.",
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(27, 40),
                AutoSize = true
            };

            lblGrafoVertices = CrearEtiquetaMetrica("Sedes / Puntos: 0", new Point(25, 65), 185);
            lblGrafoAristas = CrearEtiquetaMetrica("Rutas (Aristas): 0", new Point(220, 65), 185);
            lblGrafoConexo = CrearEtiquetaMetrica("Red: Conexa", new Point(415, 65), 165);
            lblGrafoRutaResumen = CrearEtiquetaMetrica("Ruta Óptima: Lista", new Point(590, 65), 415);
            lblGrafoRutaResumen.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);

            // Panel izquierdo: Lista de Adyacencia de todas las sedes y sus distancias.
            GroupBox grpAdyacencia = new GroupBox
            {
                Text = "Lista de Adyacencia (Conexiones de la Red MarFarma)",
                Location = new Point(25, 110),
                Size = new Size(575, 305)
            };

            lstGrafoAdyacencia = new ListBox
            {
                Location = new Point(12, 24),
                Size = new Size(550, 268),
                Font = new Font("Consolas", 9F),
                BackColor = Color.White,
                HorizontalScrollbar = true
            };
            grpAdyacencia.Controls.Add(lstGrafoAdyacencia);

            // Panel derecho: ejecución de Dijkstra, BFS, DFS, despacho y alta de vértices/aristas.
            GroupBox grpGrafoAlgoritmos = new GroupBox
            {
                Text = "Algoritmos y Gestión de Red",
                Location = new Point(615, 110),
                Size = new Size(390, 305)
            };

            Label lOri = new Label { Text = "Origen:", Location = new Point(15, 22), AutoSize = true };
            Label lDes = new Label { Text = "Destino:", Location = new Point(205, 22), AutoSize = true };
            cmbGrafoOrigen = new ComboBox { Location = new Point(15, 40), Size = new Size(175, 23), DropDownStyle = ComboBoxStyle.DropDownList, DropDownWidth = 250 };
            cmbGrafoDestino = new ComboBox { Location = new Point(205, 40), Size = new Size(170, 23), DropDownStyle = ComboBoxStyle.DropDownList, DropDownWidth = 250 };

            btnGrafoDijkstra = new Button
            {
                Text = "Calcular Ruta Más Corta (Dijkstra)",
                Location = new Point(15, 72),
                Size = new Size(360, 30),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            btnGrafoBFS = new Button
            {
                Text = "Recorrido BFS (Anchura)",
                Location = new Point(15, 108),
                Size = new Size(175, 28),
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            btnGrafoDFS = new Button
            {
                Text = "Recorrido DFS (Profundidad)",
                Location = new Point(200, 108),
                Size = new Size(175, 28),
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            Label lDesp = new Label { Text = "Despacho de producto (ABB -> Ruta Óptima):", Location = new Point(15, 144), AutoSize = true, Font = new Font("Segoe UI", 8.5F, FontStyle.Bold) };
            cmbGrafoProducto = new ComboBox { Location = new Point(15, 162), Size = new Size(235, 23), DropDownStyle = ComboBoxStyle.DropDownList, DropDownWidth = 290 };
            btnGrafoDespachar = new Button
            {
                Text = "Despachar",
                Location = new Point(258, 161),
                Size = new Size(117, 25),
                BackColor = Color.FromArgb(230, 126, 34),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            Label lSed = new Label { Text = "Agregar Sede (Código / Nombre / Tipo):", Location = new Point(15, 195), AutoSize = true };
            txtGrafoNuevoCod = new TextBox { Location = new Point(15, 213), Size = new Size(65, 23), PlaceholderText = "SED-07" };
            txtGrafoNuevoNombre = new TextBox { Location = new Point(85, 213), Size = new Size(140, 23), PlaceholderText = "Nueva Sede" };
            cmbGrafoNuevoTipo = new ComboBox { Location = new Point(230, 213), Size = new Size(95, 23), DropDownStyle = ComboBoxStyle.DropDownList, DropDownWidth = 170 };
            cmbGrafoNuevoTipo.Items.AddRange(new object[] { "Sucursal MarFarma", "Agente BCP", "Centro Médico", "Almacén Central" });
            cmbGrafoNuevoTipo.SelectedIndex = 0;
            btnGrafoAgregarVertice = new Button
            {
                Text = "+",
                Location = new Point(332, 212),
                Size = new Size(43, 25),
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            Label lAri = new Label { Text = "Conectar Ruta (Sede A <-> Sede B | km):", Location = new Point(15, 244), AutoSize = true };
            cmbGrafoAristaA = new ComboBox { Location = new Point(15, 262), Size = new Size(95, 23), DropDownStyle = ComboBoxStyle.DropDownList, DropDownWidth = 220 };
            cmbGrafoAristaB = new ComboBox { Location = new Point(116, 262), Size = new Size(95, 23), DropDownStyle = ComboBoxStyle.DropDownList, DropDownWidth = 220 };
            nudGrafoKm = new NumericUpDown
            {
                Location = new Point(217, 262),
                Size = new Size(65, 23),
                DecimalPlaces = 1,
                Increment = 0.5m,
                Minimum = 0.2m,
                Maximum = 50.0m,
                Value = 1.5m
            };
            btnGrafoAgregarArista = new Button
            {
                Text = "+ Ruta",
                Location = new Point(288, 261),
                Size = new Size(87, 25),
                BackColor = Color.FromArgb(44, 62, 80),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            grpGrafoAlgoritmos.Controls.AddRange(new Control[]
            {
                lOri, lDes, cmbGrafoOrigen, cmbGrafoDestino,
                btnGrafoDijkstra, btnGrafoBFS, btnGrafoDFS,
                lDesp, cmbGrafoProducto, btnGrafoDespachar,
                lSed, txtGrafoNuevoCod, txtGrafoNuevoNombre, cmbGrafoNuevoTipo, btnGrafoAgregarVertice,
                lAri, cmbGrafoAristaA, cmbGrafoAristaB, nudGrafoKm, btnGrafoAgregarArista
            });

            // Panel inferior: detalle paso a paso de BFS, DFS, Dijkstra o despacho.
            GroupBox grpGrafoTrazas = new GroupBox
            {
                Text = "Resultados y Trazas Paso a Paso (BFS / DFS / Dijkstra / Despacho)",
                Location = new Point(25, 425),
                Size = new Size(980, 225)
            };

            lstGrafoTrazas = new ListBox
            {
                Location = new Point(15, 24),
                Size = new Size(950, 188),
                Font = new Font("Consolas", 9F),
                BackColor = Color.White,
                HorizontalScrollbar = true
            };

            grpGrafoTrazas.Controls.Add(lstGrafoTrazas);

            pnlVistaGrafos.Controls.AddRange(new Control[]
            {
                lblTit, lblSub,
                lblGrafoVertices, lblGrafoAristas, lblGrafoConexo, lblGrafoRutaResumen,
                grpAdyacencia, grpGrafoAlgoritmos, grpGrafoTrazas
            });
            pnlContenedorPrincipal.Controls.Add(pnlVistaGrafos);
        }

        // Vincula los botones de Dijkstra, BFS, DFS, despacho y creación de nodos/rutas.
        private void VincularEventosGrafos()
        {
            btnGrafoDijkstra.Click += (s, e) => EjecutarDijkstraEnVista(null);

            btnGrafoBFS.Click += (s, e) =>
            {
                if (cmbGrafoOrigen.SelectedItem is not VerticeGrafo origen) return;

                List<string> trazas = new List<string>();
                List<VerticeGrafo> visitados = grafoRutas.RecorrerBFS(origen.Dato.Codigo, trazas);

                List<string> codigos = new List<string>();
                foreach (var v in visitados) codigos.Add(v.Dato.Codigo);

                lblGrafoRutaResumen.Text = $"BFS ({visitados.Count} sedes): {string.Join(" -> ", codigos)}";
                MostrarTrazasGrafo(trazas);
            };

            btnGrafoDFS.Click += (s, e) =>
            {
                if (cmbGrafoOrigen.SelectedItem is not VerticeGrafo origen) return;

                List<string> trazas = new List<string>();
                List<VerticeGrafo> visitados = grafoRutas.RecorrerDFS(origen.Dato.Codigo, trazas);

                List<string> codigos = new List<string>();
                foreach (var v in visitados) codigos.Add(v.Dato.Codigo);

                lblGrafoRutaResumen.Text = $"DFS ({visitados.Count} sedes): {string.Join(" -> ", codigos)}";
                MostrarTrazasGrafo(trazas);
            };

            btnGrafoDespachar.Click += (s, e) =>
            {
                if (cmbGrafoProducto.SelectedItem is not Producto prod)
                {
                    MessageBox.Show("No hay productos disponibles en el inventario.", "Despacho", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (prod.Stock <= 0)
                {
                    MessageBox.Show($"El producto '{prod.Nombre}' no tiene stock disponible.", "Sin Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                EjecutarDijkstraEnVista(prod);
            };

            btnGrafoAgregarVertice.Click += (s, e) =>
            {
                string codigo = string.IsNullOrWhiteSpace(txtGrafoNuevoCod.Text)
                    ? $"SED-{grafoRutas.CantidadVertices + 1:D2}"
                    : txtGrafoNuevoCod.Text.Trim().ToUpper();

                if (string.IsNullOrWhiteSpace(txtGrafoNuevoNombre.Text))
                {
                    MessageBox.Show("Ingrese el nombre de la sede o punto de entrega.", "Nueva Sede", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string tipo = cmbGrafoNuevoTipo.SelectedItem?.ToString() ?? "Sucursal MarFarma";
                if (grafoRutas.AgregarVertice(new PuntoRed(codigo, txtGrafoNuevoNombre.Text.Trim(), tipo)))
                {
                    txtGrafoNuevoNombre.Clear();
                    RefrescarVistaGrafos();
                    txtGrafoNuevoCod.Text = $"SED-{grafoRutas.CantidadVertices + 1:D2}";
                    ActualizarResumenSidebar();
                }
                else
                {
                    MessageBox.Show($"Ya existe una sede con código '{codigo}'.", "Código Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            btnGrafoAgregarArista.Click += (s, e) =>
            {
                if (cmbGrafoAristaA.SelectedItem is not VerticeGrafo vA || cmbGrafoAristaB.SelectedItem is not VerticeGrafo vB)
                    return;

                if (vA == vB)
                {
                    MessageBox.Show("Seleccione dos sedes distintas para conectar.", "Ruta Inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                double km = (double)nudGrafoKm.Value;
                int min = Math.Max(2, (int)Math.Round(km * 4));

                if (grafoRutas.AgregarArista(vA.Dato.Codigo, vB.Dato.Codigo, km, min))
                {
                    RefrescarVistaGrafos();
                    MostrarTrazasGrafo(new List<string>
                    {
                        $"[RED] Ruta conectada: [{vA.Dato.Codigo}] <-> [{vB.Dato.Codigo}] ({km:F1} km / {min} min)"
                    });
                }
            };
        }

        // Calcula y muestra la ruta más corta entre origen y destino usando Dijkstra.
        private void EjecutarDijkstraEnVista(Producto? productoDespacho)
        {
            if (cmbGrafoOrigen.SelectedItem is not VerticeGrafo origen || cmbGrafoDestino.SelectedItem is not VerticeGrafo destino)
                return;

            List<string> trazas = new List<string>();
            List<VerticeGrafo> camino = grafoRutas.CalcularRutaMasCorta(
                origen.Dato.Codigo,
                destino.Dato.Codigo,
                out double distanciaTotal,
                out int tiempoTotal,
                trazas);

            if (camino.Count == 0)
            {
                lblGrafoRutaResumen.Text = "Sin ruta disponible entre ambas sedes.";
                MostrarTrazasGrafo(trazas);
                return;
            }

            List<string> pasosTexto = new List<string>();
            foreach (var v in camino) pasosTexto.Add(v.Dato.Codigo);

            string resumen = $"{string.Join(" -> ", pasosTexto)} ({distanciaTotal:F1} km, ~{tiempoTotal} min)";
            lblGrafoRutaResumen.Text = $"Óptima: {resumen}";

            trazas.Add("--------------------------------------------------");
            trazas.Add($"RUTA ÓPTIMA: {resumen}");

            if (productoDespacho != null)
            {
                trazas.Add($"DESPACHO: [{productoDespacho.Codigo}] {productoDespacho.Nombre} (Stock: {productoDespacho.Stock} und.)");
                MessageBox.Show(
                    $"Despacho planificado para '{productoDespacho.Nombre}'.\nRuta óptima (Dijkstra): {resumen}",
                    "Despacho Logístico MarFarma",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            MostrarTrazasGrafo(trazas);
        }

        private void MostrarTrazasGrafo(List<string> trazas)
        {
            lstGrafoTrazas.Items.Clear();
            foreach (var linea in trazas)
            {
                lstGrafoTrazas.Items.Add(linea);
            }
        }

        // Actualiza los selectores de sedes, métricas de conectividad y lista de adyacencia.
        private void RefrescarVistaGrafos()
        {
            string? selOri = (cmbGrafoOrigen.SelectedItem as VerticeGrafo)?.Dato.Codigo;
            string? selDes = (cmbGrafoDestino.SelectedItem as VerticeGrafo)?.Dato.Codigo;
            string? selA = (cmbGrafoAristaA.SelectedItem as VerticeGrafo)?.Dato.Codigo;
            string? selB = (cmbGrafoAristaB.SelectedItem as VerticeGrafo)?.Dato.Codigo;

            var vertices = grafoRutas.ObtenerVertices();
            ComboBox[] combos = { cmbGrafoOrigen, cmbGrafoDestino, cmbGrafoAristaA, cmbGrafoAristaB };
            foreach (var c in combos)
            {
                c.Items.Clear();
                foreach (var v in vertices) c.Items.Add(v);
            }

            RestaurarSeleccionVertice(cmbGrafoOrigen, selOri, 0);
            RestaurarSeleccionVertice(cmbGrafoDestino, selDes, Math.Max(0, vertices.Count - 1));
            RestaurarSeleccionVertice(cmbGrafoAristaA, selA, 0);
            RestaurarSeleccionVertice(cmbGrafoAristaB, selB, Math.Min(1, Math.Max(0, vertices.Count - 1)));

            lblGrafoVertices.Text = $"Sedes / Puntos: {grafoRutas.CantidadVertices}";
            lblGrafoAristas.Text = $"Rutas (Aristas): {grafoRutas.CantidadAristas}";
            bool conexo = grafoRutas.EsGrafoConexo();
            lblGrafoConexo.Text = conexo ? "Red: 100% Conexa" : "Red: Con Componentes";
            lblGrafoConexo.ForeColor = conexo ? Color.FromArgb(39, 174, 96) : Color.FromArgb(211, 84, 0);

            lstGrafoAdyacencia.Items.Clear();
            foreach (var linea in grafoRutas.ObtenerListaAdyacencia())
            {
                lstGrafoAdyacencia.Items.Add(linea);
            }

            if (string.IsNullOrWhiteSpace(txtGrafoNuevoCod.Text))
            {
                txtGrafoNuevoCod.Text = $"SED-{grafoRutas.CantidadVertices + 1:D2}";
            }
        }

        private static void RestaurarSeleccionVertice(ComboBox combo, string? codigo, int indicePorDefecto)
        {
            if (combo.Items.Count == 0) return;
            if (!string.IsNullOrWhiteSpace(codigo))
            {
                for (int i = 0; i < combo.Items.Count; i++)
                {
                    if (combo.Items[i] is VerticeGrafo v && v.Dato.Codigo.Equals(codigo, StringComparison.OrdinalIgnoreCase))
                    {
                        combo.SelectedIndex = i;
                        return;
                    }
                }
            }
            combo.SelectedIndex = Math.Min(indicePorDefecto, combo.Items.Count - 1);
        }
    }
}
