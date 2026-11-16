using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BoticaMarFarmaTransaccional.Arboles;
using BoticaMarFarmaTransaccional.Entidades;

namespace BoticaMarFarmaTransaccional
{
    public partial class Form1
    {
        // Panel principal y controles de la vista del Árbol Binario de Búsqueda (ABB).
        private Panel pnlVistaInventario = null!;
        private Label lblAbbTotalNodos = null!;
        private Label lblAbbAltura = null!;
        private Label lblAbbStockTotal = null!;

        private ListBox lstArbolJerarquia = null!;
        private TextBox txtAbbCodigo = null!;
        private TextBox txtAbbNombre = null!;
        private TextBox txtAbbPrecio = null!;
        private NumericUpDown nudAbbStock = null!;
        private ComboBox cmbAbbCategoria = null!;
        private Button btnAbbInsertar = null!;
        private Button btnAbbActualizarStock = null!;

        private TextBox txtAbbBuscarCod = null!;
        private Button btnAbbBuscar = null!;
        private Button btnAbbEliminar = null!;
        private Label lblAbbResultadoBusqueda = null!;

        private Button btnAbbInOrden = null!;
        private Button btnAbbPreOrden = null!;
        private Button btnAbbPostOrden = null!;
        private Label lblAbbSecuenciaRecorrido = null!;
        private DataGridView dgvInventarioArbol = null!;

        private string modoRecorridoAbb = "INORDEN";

        // Construye la vista de gestión de inventario usando un Árbol Binario de Búsqueda (sin dibujos).
        private void ConstruirVistaInventario()
        {
            pnlVistaInventario = new Panel { Dock = DockStyle.Fill, Padding = new Padding(25), Visible = false };

            Label lblTit = new Label
            {
                Text = "INVENTARIO DE MEDICAMENTOS (ÁRBOL BINARIO DE BÚSQUEDA - ABB)",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(25, 15),
                AutoSize = true
            };
            Label lblSub = new Label
            {
                Text = "Ordenado por código: subárbol izquierdo < raíz < subárbol derecho. Permite búsqueda, inserción, eliminación y recorridos.",
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(27, 40),
                AutoSize = true
            };

            // Métricas superiores del árbol.
            lblAbbTotalNodos = CrearEtiquetaMetrica("Productos (Nodos): 0", new Point(25, 65), 315);
            lblAbbAltura = CrearEtiquetaMetrica("Altura del Árbol: 0", new Point(355, 65), 315);
            lblAbbStockTotal = CrearEtiquetaMetrica("Stock Total: 0 unidades", new Point(685, 65), 320);

            // Panel izquierdo: representación en texto de la jerarquía izquierda/derecha del ABB.
            GroupBox grpJerarquia = new GroupBox
            {
                Text = "Estructura del Árbol (Raíz, Izquierda y Derecha)",
                Location = new Point(25, 110),
                Size = new Size(575, 290)
            };

            lstArbolJerarquia = new ListBox
            {
                Location = new Point(12, 24),
                Size = new Size(550, 252),
                Font = new Font("Consolas", 9F),
                BackColor = Color.White,
                HorizontalScrollbar = true
            };
            grpJerarquia.Controls.Add(lstArbolJerarquia);

            // Panel derecho: formulario para insertar, actualizar stock, buscar y eliminar.
            GroupBox grpAbbGestion = new GroupBox
            {
                Text = "Operaciones en el Árbol (ABB)",
                Location = new Point(615, 110),
                Size = new Size(390, 290)
            };

            Label lCod = new Label { Text = "Código:", Location = new Point(15, 22), AutoSize = true };
            Label lNom = new Label { Text = "Nombre Medicamento:", Location = new Point(130, 22), AutoSize = true };
            txtAbbCodigo = new TextBox { Location = new Point(15, 40), Size = new Size(108, 23), PlaceholderText = "MED-08" };
            txtAbbNombre = new TextBox { Location = new Point(130, 40), Size = new Size(245, 23), PlaceholderText = "Ej. Loratadina 10mg" };

            Label lPre = new Label { Text = "Precio S/:", Location = new Point(15, 68), AutoSize = true };
            Label lStk = new Label { Text = "Stock:", Location = new Point(125, 68), AutoSize = true };
            Label lCat = new Label { Text = "Categoría:", Location = new Point(220, 68), AutoSize = true };

            txtAbbPrecio = new TextBox { Location = new Point(15, 86), Size = new Size(100, 23), PlaceholderText = "10.50" };
            nudAbbStock = new NumericUpDown { Location = new Point(125, 86), Size = new Size(85, 23), Minimum = 0, Maximum = 9999, Value = 25 };
            cmbAbbCategoria = new ComboBox { Location = new Point(220, 86), Size = new Size(155, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbAbbCategoria.Items.AddRange(new object[] { "Analgésicos", "Antibióticos", "Gastro", "Vitaminas", "Antisépticos", "Pediatría", "Medicamento" });
            cmbAbbCategoria.SelectedIndex = 0;

            btnAbbInsertar = new Button
            {
                Text = "+ Insertar en ABB",
                Location = new Point(15, 118),
                Size = new Size(175, 34),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            btnAbbActualizarStock = new Button
            {
                Text = "Actualizar Stock",
                Location = new Point(200, 118),
                Size = new Size(175, 34),
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            Label lBus = new Label { Text = "Buscar / Eliminar por Código:", Location = new Point(15, 162), AutoSize = true, Font = new Font("Segoe UI", 8.5F, FontStyle.Bold) };
            txtAbbBuscarCod = new TextBox { Location = new Point(15, 182), Size = new Size(145, 23), PlaceholderText = "MED-03" };

            btnAbbBuscar = new Button
            {
                Text = "Buscar ABB",
                Location = new Point(168, 181),
                Size = new Size(105, 26),
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            btnAbbEliminar = new Button
            {
                Text = "Eliminar",
                Location = new Point(280, 181),
                Size = new Size(95, 26),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            lblAbbResultadoBusqueda = new Label
            {
                Text = "Busque un código o haga clic en una fila de la tabla para ver detalles.",
                Location = new Point(15, 215),
                Size = new Size(360, 62),
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 8.5F),
                Padding = new Padding(6)
            };

            grpAbbGestion.Controls.AddRange(new Control[]
            {
                lCod, lNom, txtAbbCodigo, txtAbbNombre,
                lPre, lStk, lCat, txtAbbPrecio, nudAbbStock, cmbAbbCategoria,
                btnAbbInsertar, btnAbbActualizarStock,
                lBus, txtAbbBuscarCod, btnAbbBuscar, btnAbbEliminar, lblAbbResultadoBusqueda
            });

            // Panel inferior: botones de recorrido y grilla sincronizada.
            GroupBox grpAbbRecorridos = new GroupBox
            {
                Text = "Recorridos del Árbol y Catálogo Sincronizado",
                Location = new Point(25, 410),
                Size = new Size(980, 240)
            };

            btnAbbInOrden = CrearBotonRecorrido("InOrden (Ordenado)", new Point(15, 22), true);
            btnAbbPreOrden = CrearBotonRecorrido("PreOrden", new Point(165, 22), false);
            btnAbbPostOrden = CrearBotonRecorrido("PostOrden", new Point(280, 22), false);

            lblAbbSecuenciaRecorrido = new Label
            {
                Text = "Secuencia: ...",
                Location = new Point(400, 24),
                Size = new Size(565, 26),
                Font = new Font("Consolas", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                TextAlign = ContentAlignment.MiddleLeft
            };

            dgvInventarioArbol = new DataGridView
            {
                Location = new Point(15, 56),
                Size = new Size(950, 172),
                BackgroundColor = Color.White,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvInventarioArbol.Columns.Add("Codigo", "Código");
            dgvInventarioArbol.Columns.Add("Nombre", "Medicamento");
            dgvInventarioArbol.Columns.Add("Categoria", "Categoría");
            dgvInventarioArbol.Columns.Add("Precio", "Precio Unit.");
            dgvInventarioArbol.Columns.Add("Stock", "Stock Actual");
            dgvInventarioArbol.Columns.Add("Estado", "Estado Stock");

            grpAbbRecorridos.Controls.AddRange(new Control[]
            {
                btnAbbInOrden, btnAbbPreOrden, btnAbbPostOrden, lblAbbSecuenciaRecorrido, dgvInventarioArbol
            });

            pnlVistaInventario.Controls.AddRange(new Control[]
            {
                lblTit, lblSub, lblAbbTotalNodos, lblAbbAltura, lblAbbStockTotal,
                grpJerarquia, grpAbbGestion, grpAbbRecorridos
            });
            pnlContenedorPrincipal.Controls.Add(pnlVistaInventario);
        }

        private static Label CrearEtiquetaMetrica(string texto, Point ubicacion, int ancho)
        {
            return new Label
            {
                Text = texto,
                Location = ubicacion,
                Size = new Size(ancho, 34),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                TextAlign = ContentAlignment.MiddleCenter
            };
        }

        private static Button CrearBotonRecorrido(string texto, Point ubicacion, bool activo)
        {
            return new Button
            {
                Text = texto,
                Location = ubicacion,
                Size = new Size(texto.Length > 12 ? 142 : 108, 28),
                BackColor = activo ? Color.FromArgb(41, 128, 185) : Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
        }

        // Vincula los eventos de inserción, actualización de stock, búsqueda, eliminación y recorridos.
        private void VincularEventosInventario()
        {
            btnAbbInsertar.Click += (s, e) =>
            {
                string codigo = string.IsNullOrWhiteSpace(txtAbbCodigo.Text)
                    ? GenerarSiguienteCodigoProducto()
                    : txtAbbCodigo.Text.Trim().ToUpper();

                if (string.IsNullOrWhiteSpace(txtAbbNombre.Text) ||
                    !decimal.TryParse(txtAbbPrecio.Text, out decimal precio) ||
                    precio <= 0)
                {
                    MessageBox.Show("Ingrese un nombre válido y un precio mayor a 0.", "Validación ABB", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string categoria = cmbAbbCategoria.SelectedItem?.ToString() ?? "Medicamento";
                Producto nuevo = new Producto(codigo, txtAbbNombre.Text.Trim(), precio, (int)nudAbbStock.Value, categoria);

                if (arbolInventario.Insertar(nuevo))
                {
                    RefrescarComboProductos();
                    RefrescarVistaInventario();
                    ActualizarResumenSidebar();

                    txtAbbCodigo.Text = GenerarSiguienteCodigoProducto();
                    txtAbbNombre.Clear();
                    txtAbbPrecio.Clear();
                    lblAbbResultadoBusqueda.Text = $"Insertado en el ABB: [{nuevo.Codigo}] {nuevo.Nombre} (Stock: {nuevo.Stock}).";
                }
                else
                {
                    MessageBox.Show($"Ya existe un producto con el código '{codigo}' en el árbol.", "Código Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            btnAbbActualizarStock.Click += (s, e) =>
            {
                string codigo = txtAbbCodigo.Text.Trim().ToUpper();
                if (string.IsNullOrWhiteSpace(codigo))
                {
                    MessageBox.Show("Seleccione un producto de la tabla o escriba su código.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var nodo = arbolInventario.BuscarNodo(new Producto(codigo, "", 0m));
                if (nodo == null)
                {
                    MessageBox.Show($"No se encontró el código '{codigo}' en el ABB.", "No Encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                nodo.Dato.Stock = (int)nudAbbStock.Value;
                if (!string.IsNullOrWhiteSpace(txtAbbNombre.Text))
                {
                    nodo.Dato.Nombre = txtAbbNombre.Text.Trim();
                }
                if (decimal.TryParse(txtAbbPrecio.Text, out decimal nuevoPrecio) && nuevoPrecio > 0)
                {
                    nodo.Dato.PrecioUnitario = nuevoPrecio;
                }
                nodo.Dato.Categoria = cmbAbbCategoria.SelectedItem?.ToString() ?? nodo.Dato.Categoria;

                RefrescarComboProductos();
                RefrescarVistaInventario();
                lblAbbResultadoBusqueda.Text = $"Stock actualizado para [{nodo.Dato.Codigo}]: {nodo.Dato.Stock} unidades.";
            };

            btnAbbBuscar.Click += (s, e) =>
            {
                string codigo = txtAbbBuscarCod.Text.Trim().ToUpper();
                if (string.IsNullOrWhiteSpace(codigo))
                {
                    MessageBox.Show("Ingrese el código a buscar (ej. MED-03).", "Búsqueda ABB", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var nodo = arbolInventario.BuscarNodo(new Producto(codigo, "", 0m));
                if (nodo != null)
                {
                    CargarProductoEnFormularioAbb(nodo.Dato);
                    lblAbbResultadoBusqueda.Text =
                        $"ENCONTRADO: [{nodo.Dato.Codigo}] {nodo.Dato.Nombre}\n" +
                        $"Precio: S/ {nodo.Dato.PrecioUnitario:F2} | Stock: {nodo.Dato.Stock} ({nodo.Dato.Categoria})";
                }
                else
                {
                    lblAbbResultadoBusqueda.Text = $"NO ENCONTRADO: '{codigo}' no existe en el árbol.";
                }
            };

            btnAbbEliminar.Click += (s, e) =>
            {
                string codigo = string.IsNullOrWhiteSpace(txtAbbBuscarCod.Text)
                    ? txtAbbCodigo.Text.Trim().ToUpper()
                    : txtAbbBuscarCod.Text.Trim().ToUpper();

                if (string.IsNullOrWhiteSpace(codigo))
                {
                    MessageBox.Show("Indique el código del producto a eliminar.", "Eliminar ABB", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (arbolInventario.Eliminar(new Producto(codigo, "", 0m)))
                {
                    RefrescarComboProductos();
                    RefrescarVistaInventario();
                    ActualizarResumenSidebar();
                    lblAbbResultadoBusqueda.Text = $"Producto [{codigo}] eliminado correctamente del ABB.";
                }
                else
                {
                    MessageBox.Show($"No se encontró '{codigo}' para eliminar.", "No Encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            btnAbbInOrden.Click += (s, e) => CambiarModoRecorridoAbb("INORDEN");
            btnAbbPreOrden.Click += (s, e) => CambiarModoRecorridoAbb("PREORDEN");
            btnAbbPostOrden.Click += (s, e) => CambiarModoRecorridoAbb("POSTORDEN");

            dgvInventarioArbol.CellClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                string? cod = dgvInventarioArbol.Rows[e.RowIndex].Cells["Codigo"].Value?.ToString();
                if (string.IsNullOrWhiteSpace(cod)) return;

                var nodo = arbolInventario.BuscarNodo(new Producto(cod, "", 0m));
                if (nodo != null)
                {
                    CargarProductoEnFormularioAbb(nodo.Dato);
                    txtAbbBuscarCod.Text = nodo.Dato.Codigo;
                }
            };
        }

        private void CargarProductoEnFormularioAbb(Producto p)
        {
            txtAbbCodigo.Text = p.Codigo;
            txtAbbNombre.Text = p.Nombre;
            txtAbbPrecio.Text = p.PrecioUnitario.ToString("F2");
            nudAbbStock.Value = p.Stock > nudAbbStock.Maximum ? nudAbbStock.Maximum : p.Stock;
            if (cmbAbbCategoria.Items.Contains(p.Categoria))
            {
                cmbAbbCategoria.SelectedItem = p.Categoria;
            }
        }

        private void CambiarModoRecorridoAbb(string modo)
        {
            modoRecorridoAbb = modo;
            btnAbbInOrden.BackColor = modo == "INORDEN" ? Color.FromArgb(41, 128, 185) : Color.FromArgb(149, 165, 166);
            btnAbbPreOrden.BackColor = modo == "PREORDEN" ? Color.FromArgb(41, 128, 185) : Color.FromArgb(149, 165, 166);
            btnAbbPostOrden.BackColor = modo == "POSTORDEN" ? Color.FromArgb(41, 128, 185) : Color.FromArgb(149, 165, 166);
            RefrescarVistaInventario();
        }

        // Genera el siguiente código MED-XX disponible recorriendo los productos del árbol.
        private string GenerarSiguienteCodigoProducto()
        {
            int maxNumero = 0;
            foreach (var p in arbolInventario.RecorridoInOrden())
            {
                if (p.Codigo.StartsWith("MED-", StringComparison.OrdinalIgnoreCase) &&
                    int.TryParse(p.Codigo.Substring(4), out int n) &&
                    n > maxNumero)
                {
                    maxNumero = n;
                }
            }
            return $"MED-{maxNumero + 1:D2}";
        }

        // Actualiza los contadores, la tabla del recorrido elegido y la lista jerárquica del ABB.
        private void RefrescarVistaInventario()
        {
            List<Producto> lista = modoRecorridoAbb switch
            {
                "PREORDEN" => arbolInventario.RecorridoPreOrden(),
                "POSTORDEN" => arbolInventario.RecorridoPostOrden(),
                _ => arbolInventario.RecorridoInOrden()
            };

            int stockTotal = 0;
            List<string> secuencia = new List<string>();
            dgvInventarioArbol.Rows.Clear();

            foreach (var p in lista)
            {
                stockTotal += p.Stock;
                secuencia.Add(p.Codigo);

                string estado = p.Stock == 0 ? "AGOTADO" : (p.Stock <= 10 ? "STOCK BAJO" : "Disponible");
                dgvInventarioArbol.Rows.Add(
                    p.Codigo,
                    p.Nombre,
                    p.Categoria,
                    $"S/ {p.PrecioUnitario:F2}",
                    p.Stock,
                    estado);
            }

            lblAbbTotalNodos.Text = $"Productos (Nodos): {arbolInventario.Count}";
            lblAbbAltura.Text = $"Altura del Árbol: {arbolInventario.ObtenerAltura()}";
            lblAbbStockTotal.Text = $"Stock Total: {stockTotal} unidades";

            lblAbbSecuenciaRecorrido.Text = secuencia.Count > 0
                ? $"{modoRecorridoAbb}: {string.Join(" -> ", secuencia)}"
                : $"{modoRecorridoAbb}: (Árbol vacío)";

            // Construye la vista textual de niveles para entender qué nodo va a la izquierda y derecha.
            lstArbolJerarquia.Items.Clear();
            NodoArbol<Producto>? raiz = arbolInventario.ObtenerRaiz();
            if (raiz == null)
            {
                lstArbolJerarquia.Items.Add("(Árbol vacío)");
            }
            else
            {
                LlenarJerarquiaTexto(raiz, "", "RAÍZ");
            }
        }

        // Recorre el árbol recursivamente para listar en texto las ramas izquierda y derecha.
        private void LlenarJerarquiaTexto(NodoArbol<Producto>? nodo, string sangria, string rama)
        {
            if (nodo == null) return;

            lstArbolJerarquia.Items.Add($"{sangria}[{rama}] {nodo.Dato.Codigo} - {nodo.Dato.Nombre} (Stock: {nodo.Dato.Stock})");
            LlenarJerarquiaTexto(nodo.Izquierda, sangria + "   ", "IZQ");
            LlenarJerarquiaTexto(nodo.Derecha, sangria + "   ", "DER");
        }
    }
}
