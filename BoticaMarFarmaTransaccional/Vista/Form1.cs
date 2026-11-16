using System;
using System.Drawing;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using BoticaMarFarmaTransaccional.Pilas;
using BoticaMarFarmaTransaccional.Colas;
using BoticaMarFarmaTransaccional.Arboles;
using BoticaMarFarmaTransaccional.Grafos;
using BoticaMarFarmaTransaccional.ListaSimple;
using BoticaMarFarmaTransaccional.Entidades;

namespace BoticaMarFarmaTransaccional
{
    public partial class Form1 : Form
    {
        // Estructuras enlazadas y no lineales que gestionan el carrito, historial, auditoría, campañas, turnos, inventario, consultas y rutas.
        private PilaDetalle pilaCarrito = new PilaDetalle();
        private ListaSimpleTransacciones listaSimpleHistorial = new ListaSimpleTransacciones();
        private ListaDobleTransacciones listaHistorial = new ListaDobleTransacciones();
        private ListaCircularCampanas carruselCampanas = new ListaCircularCampanas();
        private Cola<TurnoAtencion> colaAtencion = new Cola<TurnoAtencion>();
        private ArbolBinario<Producto> arbolInventario = new ArbolBinario<Producto>();
        private ArbolBinario<TransaccionVenta> arbolOperaciones = new ArbolBinario<TransaccionVenta>();
        private GrafoRutas grafoRutas = new GrafoRutas();

        // Datos en memoria: catálogos, numeración de registros y control del temporizador.
        private List<Cliente> listaClientes = new List<Cliente>();
        private int contadorOperaciones = 1041;
        private int contadorCampanas = 5;
        private int contadorTurnosCola = 1;
        private TurnoAtencion? ultimoTurnoAtendido = null;
        private bool timerActivo = true;

        // Contenedores y botones del menú lateral y de las vistas principales.
        private Panel pnlSidebar = null!;
        private Panel pnlContenedorPrincipal = null!;
        private Button btnNavVenta = null!;
        private Button btnNavHistorial = null!;
        private Button btnNavAuditoria = null!;
        private Button btnNavCarrusel = null!;
        private Button btnNavCola = null!;
        private Button btnNavInventario = null!;
        private Button btnNavGrafos = null!;
        private Label lblResumenSidebar = null!;

        private Panel pnlVistaVenta = null!;
        private Panel pnlVistaHistorial = null!;
        private Panel pnlVistaAuditoria = null!;
        private Panel pnlVistaCarrusel = null!;

        // Controles de selección del cliente, del producto y de los datos de cobro.
        private Label lblBannerPromocionVenta = null!;
        private RadioButton rbClienteExistente = null!;
        private RadioButton rbClienteNuevo = null!;
        private ComboBox cmbClientes = null!;
        private TextBox txtNuevoClienteNombre = null!;
        private TextBox txtNuevoClienteDni = null!;
        private Button btnGuardarCliente = null!;
        private Label lblClienteActivo = null!;

        private RadioButton rbProdExistente = null!;
        private RadioButton rbProdNuevo = null!;
        private ComboBox cmbProductos = null!;
        private TextBox txtNuevoProdNombre = null!;
        private TextBox txtNuevoProdPrecio = null!;
        private Button btnGuardarProducto = null!;
        private NumericUpDown nudCantidad = null!;
        private ComboBox cmbCanalCobro = null!;
        private Button btnAgregarAlCarrito = null!;

        private DataGridView dgvCarrito = null!;
        private Button btnDeshacerPila = null!;
        private Button btnLimpiarCarrito = null!;
        private NumericUpDown nudDescuentoPct = null!;
        private Label lblFormulaPostfija = null!;
        private Label lblSubtotalVenta = null!;
        private Label lblIgvVenta = null!;
        private Label lblTotalVenta = null!;
        private Button btnProcesarCobro = null!;

        // Controles para mostrar la tabla, métricas y búsqueda ABB del historial.
        private DataGridView dgvHistorialCompleto = null!;
        private Label lblMetricaTotalOperaciones = null!;
        private Label lblMetricaRecaudacionCaja = null!;
        private TextBox txtBuscarOpAbb = null!;
        private Button btnBuscarOpAbb = null!;

        // Controles para navegar por las transacciones y mostrar su comprobante.
        private Button btnAuditoriaAtras = null!;
        private Button btnAuditoriaAdelante = null!;
        private Label lblTicketAuditado = null!;
        private Label lblDetalleAuditoria = null!;

        // Controles para mostrar, recorrer e incorporar campañas al carrusel.
        private Panel pnlTarjetonAnuncio = null!;
        private Label lblCardTitulo = null!;
        private Label lblCardBeneficio = null!;
        private Label lblCardCanalYVigencia = null!;
        private Button btnAvanzarPromoManual = null!;
        private Button btnPausarReanudarTimer = null!;
        private ListBox lstTrazasAnillo = null!;
        private TextBox txtNuevaPromoTitulo = null!;
        private TextBox txtNuevaPromoBeneficio = null!;
        private ComboBox cmbNuevaPromoCanal = null!;
        private DateTimePicker dtpVigenciaPromo = null!;
        private Button btnAgregarPromoAnillo = null!;
        private System.Windows.Forms.Timer timerCarrusel = null!;

        // Configura la ventana, crea las vistas, carga datos, enlaza eventos e inicia el carrusel.
        public Form1()
        {
            this.Text = "SISTEMA TRANSACCIONAL - BOTICA MARFARMA [CIIN1190P - EVALUACIÓN FINAL]";
            this.Size = new Size(1320, 800);
            this.MinimumSize = new Size(1200, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 246, 250);
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            ConstruirSidebar();
            ConstruirVistaVenta();
            ConstruirVistaHistorial();
            ConstruirVistaAuditoria();
            ConstruirVistaCarrusel();
            ConstruirVistaCola();
            ConstruirVistaInventario();
            ConstruirVistaGrafos();

            CargarDatosSemilla();
            VincularEventos();
            VincularEventosCola();
            VincularEventosInventario();
            VincularEventosGrafos();

            timerCarrusel = new System.Windows.Forms.Timer { Interval = 3500 };
            timerCarrusel.Tick += (s, e) => AvanzarCarruselCircular();
            timerCarrusel.Start();

            ActualizarBannerEnPantalla();
            ActualizarGrillaHistorial();
            RefrescarAuditoriaVisual();
            RefrescarVistaCola();
            RefrescarVistaInventario();
            RefrescarVistaGrafos();
            ActualizarResumenSidebar();
            CambiarVista(pnlVistaVenta, btnNavVenta);
        }

        // Construye el menú lateral y el contenedor donde se muestran las distintas vistas.
        private void ConstruirSidebar()
        {
            pnlSidebar = new Panel
            {
                Size = new Size(270, 800),
                Dock = DockStyle.Left,
                BackColor = Color.FromArgb(34, 47, 62)
            };

            Label lblTit = new Label
            {
                Text = "BOTICA MARFARMA",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, 20),
                AutoSize = true
            };
            Label lblSub = new Label
            {
                Text = "Agente BCP & Billeteras Móviles",
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(178, 190, 195),
                Location = new Point(15, 45),
                AutoSize = true
            };

            Panel sep = new Panel { Location = new Point(15, 95), Size = new Size(240, 1), BackColor = Color.FromArgb(87, 101, 116) };

            btnNavVenta = CrearBotonMenu(" 1. Nueva Venta (Pila LIFO)", 110);
            btnNavHistorial = CrearBotonMenu(" 2. Historial General", 168);
            btnNavAuditoria = CrearBotonMenu(" 3. Auditoría Caja (Doble)", 226);
            btnNavCarrusel = CrearBotonMenu(" 4. Carrusel Promos (Circular)", 284);
            btnNavCola = CrearBotonMenu(" 5. Cola Atención (FIFO)", 342);
            btnNavInventario = CrearBotonMenu(" 6. Inventario (Árbol ABB)", 400);
            btnNavGrafos = CrearBotonMenu(" 7. Red y Rutas (Grafo)", 458);

            // Resumen en vivo de todas las estructuras cargadas en memoria.
            lblResumenSidebar = new Label
            {
                Text = "Estado de estructuras...",
                Location = new Point(12, 522),
                Size = new Size(246, 115),
                BackColor = Color.FromArgb(44, 62, 80),
                ForeColor = Color.FromArgb(200, 214, 229),
                Font = new Font("Consolas", 8.5F),
                Padding = new Padding(10),
                TextAlign = ContentAlignment.MiddleLeft
            };

            pnlSidebar.Controls.AddRange(new Control[]
            {
                lblTit, lblSub, sep,
                btnNavVenta, btnNavHistorial, btnNavAuditoria, btnNavCarrusel,
                btnNavCola, btnNavInventario, btnNavGrafos, lblResumenSidebar
            });

            pnlContenedorPrincipal = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(245, 246, 250) };
            this.Controls.Add(pnlContenedorPrincipal);
            this.Controls.Add(pnlSidebar);
        }

        // Actualiza los contadores en vivo mostrados en el panel lateral izquierdo.
        private void ActualizarResumenSidebar()
        {
            lblResumenSidebar.Text =
                $"RESUMEN EN MEMORIA:\n" +
                $"• Cola FIFO  : {colaAtencion.Longitud()} en espera\n" +
                $"• Árbol ABB  : {arbolInventario.Count} productos\n" +
                $"• Grafo Red  : {grafoRutas.CantidadVertices} sed. / {grafoRutas.CantidadAristas} rut.\n" +
                $"• Ventas     : {listaHistorial.TotalRegistros} regs.";
        }

        // Crea un botón del menú con un estilo común y la posición vertical indicada.
        private Button CrearBotonMenu(string texto, int y)
        {
            Button btn = new Button
            {
                Text = texto,
                Location = new Point(10, y),
                Size = new Size(250, 48),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(223, 230, 233),
                BackColor = Color.FromArgb(44, 62, 80),
                TextAlign = ContentAlignment.MiddleLeft,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        // Oculta las vistas anteriores, resalta la opción seleccionada y muestra su panel.
        private void CambiarVista(Panel destino, Button btnActivo)
        {
            pnlVistaVenta.Visible = false;
            pnlVistaHistorial.Visible = false;
            pnlVistaAuditoria.Visible = false;
            pnlVistaCarrusel.Visible = false;
            pnlVistaCola.Visible = false;
            pnlVistaInventario.Visible = false;
            pnlVistaGrafos.Visible = false;

            Button[] btns = { btnNavVenta, btnNavHistorial, btnNavAuditoria, btnNavCarrusel, btnNavCola, btnNavInventario, btnNavGrafos };
            foreach (var b in btns) { b.BackColor = Color.FromArgb(44, 62, 80); b.ForeColor = Color.FromArgb(223, 230, 233); }

            btnActivo.BackColor = Color.FromArgb(41, 128, 185);
            btnActivo.ForeColor = Color.White;
            destino.Visible = true;
            destino.BringToFront();
        }

        // Crea los controles para seleccionar cliente y producto, gestionar el carrito y cobrar.
        private void ConstruirVistaVenta()
        {
            pnlVistaVenta = new Panel { Dock = DockStyle.Fill, Padding = new Padding(25) };

            lblBannerPromocionVenta = new Label
            {
                Text = " CAMPAÑA DEL DÍA: Cargando promociones cíclicas...",
                Location = new Point(25, 12),
                Size = new Size(975, 34),
                BackColor = Color.FromArgb(254, 249, 231),
                ForeColor = Color.FromArgb(183, 119, 0),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            GroupBox grpEntradas = new GroupBox
            {
                Text = "Datos de Atención al Paciente",
                Location = new Point(25, 55),
                Size = new Size(420, 520)
            };

            Label lblSecCli = new Label { Text = "1. Paciente / Cliente:", Font = new Font("Segoe UI", 9F, FontStyle.Bold), Location = new Point(15, 22), AutoSize = true };
            rbClienteExistente = new RadioButton { Text = "Cliente Registrado", Location = new Point(15, 42), AutoSize = true, Checked = true };
            rbClienteNuevo = new RadioButton { Text = "Nuevo Cliente", Location = new Point(160, 42), AutoSize = true };

            cmbClientes = new ComboBox { Location = new Point(15, 68), Size = new Size(385, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            txtNuevoClienteNombre = new TextBox { Location = new Point(15, 68), Size = new Size(180, 23), PlaceholderText = "Nombre completo", Visible = false };
            txtNuevoClienteDni = new TextBox { Location = new Point(200, 68), Size = new Size(95, 23), PlaceholderText = "DNI", Visible = false };
            btnGuardarCliente = new Button { Text = "Guardar", Location = new Point(300, 67), Size = new Size(100, 25), BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Visible = false };

            lblClienteActivo = new Label
            {
                Text = "Seleccione un paciente",
                Location = new Point(15, 98),
                Size = new Size(385, 24),
                BackColor = Color.FromArgb(240, 243, 244),
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label lblSecMed = new Label { Text = "2. Selección de Medicamento:", Font = new Font("Segoe UI", 9F, FontStyle.Bold), Location = new Point(15, 135), AutoSize = true };
            rbProdExistente = new RadioButton { Text = "Del Catálogo", Location = new Point(15, 155), AutoSize = true, Checked = true };
            rbProdNuevo = new RadioButton { Text = "Crear Nuevo", Location = new Point(160, 155), AutoSize = true };

            cmbProductos = new ComboBox { Location = new Point(15, 180), Size = new Size(385, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            txtNuevoProdNombre = new TextBox { Location = new Point(15, 180), Size = new Size(180, 23), PlaceholderText = "Medicamento", Visible = false };
            txtNuevoProdPrecio = new TextBox { Location = new Point(200, 180), Size = new Size(95, 23), PlaceholderText = "Precio S/", Visible = false };
            btnGuardarProducto = new Button { Text = "Guardar", Location = new Point(300, 179), Size = new Size(100, 25), BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Visible = false };

            Label lCant = new Label { Text = "Cantidad:", Location = new Point(15, 212), AutoSize = true };
            nudCantidad = new NumericUpDown { Location = new Point(15, 232), Size = new Size(80, 23), Minimum = 1, Maximum = 100, Value = 1 };

            Label lCanal = new Label { Text = "Canal de Cobro:", Location = new Point(120, 212), AutoSize = true };
            cmbCanalCobro = new ComboBox { Location = new Point(120, 232), Size = new Size(280, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbCanalCobro.Items.AddRange(new object[] { "Yape (Billetera Móvil)", "BCP QR Interoperable", "Agente BCP (Depósito)", "Efectivo Mostrador" });
            cmbCanalCobro.SelectedIndex = 0;

            lblStockProductoVenta = new Label
            {
                Text = "Stock en ABB: -",
                Location = new Point(15, 261),
                Size = new Size(385, 20),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(22, 160, 133)
            };

            btnAgregarAlCarrito = new Button
            {
                Text = " + Añadir",
                Location = new Point(15, 288),
                Size = new Size(385, 45),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };

            // Sección integrada de Cola FIFO dentro de la ventana de ventas para llamar al siguiente turno.
            Panel sepColaVenta = new Panel { Location = new Point(15, 348), Size = new Size(385, 1), BackColor = Color.FromArgb(210, 218, 226) };
            Label lblTitColaVenta = new Label { Text = "3. Turnos en Espera (Cola FIFO):", Font = new Font("Segoe UI", 9F, FontStyle.Bold), Location = new Point(15, 358), AutoSize = true };
            lblEstadoColaEnVenta = new Label
            {
                Text = "Cola FIFO vacía.",
                Location = new Point(15, 382),
                Size = new Size(385, 36),
                BackColor = Color.FromArgb(240, 243, 244),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 8.5F),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(6, 0, 6, 0)
            };
            btnLlamarSiguienteEnVenta = new Button
            {
                Text = "Atender Siguiente de Cola (Dequeue)",
                Location = new Point(15, 428),
                Size = new Size(385, 42),
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLlamarSiguienteEnVenta.FlatAppearance.BorderSize = 0;

            grpEntradas.Controls.AddRange(new Control[] {
                lblSecCli, rbClienteExistente, rbClienteNuevo, cmbClientes, txtNuevoClienteNombre, txtNuevoClienteDni, btnGuardarCliente, lblClienteActivo,
                lblSecMed, rbProdExistente, rbProdNuevo, cmbProductos, txtNuevoProdNombre, txtNuevoProdPrecio, btnGuardarProducto,
                lCant, nudCantidad, lCanal, cmbCanalCobro, lblStockProductoVenta, btnAgregarAlCarrito,
                sepColaVenta, lblTitColaVenta, lblEstadoColaEnVenta, btnLlamarSiguienteEnVenta
            });

            GroupBox grpCarrito = new GroupBox
            {
                
                Location = new Point(460, 55),
                Size = new Size(540, 520)
            };

            dgvCarrito = new DataGridView
            {
                Location = new Point(15, 25),
                Size = new Size(510, 260),
                BackgroundColor = Color.White,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvCarrito.Columns.Add("Prod", "Medicamento");
            dgvCarrito.Columns.Add("Cant", "Cant.");
            dgvCarrito.Columns.Add("Precio", "P. Unit");
            dgvCarrito.Columns.Add("Sub", "Subtotal");

            btnDeshacerPila = new Button
            {
                Text = "<- Deshacer Último Medicamento",
                Location = new Point(15, 295),
                Size = new Size(360, 40),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnLimpiarCarrito = new Button
            {
                Text = "Vaciar",
                Location = new Point(385, 295),
                Size = new Size(140, 40),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            Label lblDesc = new Label { Text = "Desc. (%):", Location = new Point(15, 350), AutoSize = true };
            nudDescuentoPct = new NumericUpDown { Location = new Point(85, 348), Size = new Size(55, 23), Minimum = 0, Maximum = 50, Value = 0 };
            lblFormulaPostfija = new Label
            {
                Text = "Postfija (Pila): 0.00",
                Location = new Point(15, 378),
                Size = new Size(320, 42),
                Font = new Font("Consolas", 8F),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            lblSubtotalVenta = new Label { Text = "Subtotal: S/ 0.00", Location = new Point(350, 350), AutoSize = true };
            lblIgvVenta = new Label { Text = "IGV (18%): S/ 0.00", Location = new Point(350, 372), AutoSize = true };
            lblTotalVenta = new Label { Text = "TOTAL: S/ 0.00", Location = new Point(350, 395), AutoSize = true, Font = new Font("Segoe UI", 12F, FontStyle.Bold), ForeColor = Color.FromArgb(192, 57, 43) };

            btnProcesarCobro = new Button
            {
                Text = " PROCESAR VENTA / COBRO",
                Location = new Point(15, 435),
                Size = new Size(510, 55),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };

            grpCarrito.Controls.AddRange(new Control[] {
                dgvCarrito, btnDeshacerPila, btnLimpiarCarrito,
                lblDesc, nudDescuentoPct, lblFormulaPostfija,
                lblSubtotalVenta, lblIgvVenta, lblTotalVenta, btnProcesarCobro
            });

            pnlVistaVenta.Controls.AddRange(new Control[] { lblBannerPromocionVenta, grpEntradas, grpCarrito });
            pnlContenedorPrincipal.Controls.Add(pnlVistaVenta);
        }

        // Prepara la tabla cronológica (Lista Simple), métricas y buscador rápido ABB por código OP-XXXX.
        private void ConstruirVistaHistorial()
        {
            pnlVistaHistorial = new Panel { Dock = DockStyle.Fill, Padding = new Padding(25), Visible = false };

            Label lblTit = new Label { Text = "HISTORIAL GENERAL DE OPERACIONES (LISTA SIMPLE Y BÚSQUEDA ABB)", Font = new Font("Segoe UI", 13F, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80), Location = new Point(25, 15), AutoSize = true };

            lblMetricaTotalOperaciones = new Label { Text = "Operaciones Totales: 0", Location = new Point(25, 55), Size = new Size(240, 40), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter };
            lblMetricaRecaudacionCaja = new Label { Text = "Caja Total: S/ 0.00", Location = new Point(280, 55), Size = new Size(240, 40), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.FromArgb(39, 174, 96) };

            Label lblBusOp = new Label { Text = "Consulta O(log n) en ABB:", Location = new Point(540, 40), AutoSize = true, Font = new Font("Segoe UI", 8.5F, FontStyle.Bold) };
            txtBuscarOpAbb = new TextBox { Location = new Point(540, 62), Size = new Size(240, 26), PlaceholderText = "Código de operación (ej. OP-1039)" };
            btnBuscarOpAbb = new Button
            {
                Text = "Buscar Boleta (ABB)",
                Location = new Point(790, 58),
                Size = new Size(215, 34),
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            dgvHistorialCompleto = new DataGridView
            {
                Location = new Point(25, 110),
                Size = new Size(980, 480),
                BackgroundColor = Color.White,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvHistorialCompleto.Columns.Add("Cod", "Código");
            dgvHistorialCompleto.Columns.Add("Fecha", "Hora");
            dgvHistorialCompleto.Columns.Add("Cliente", "Paciente / Cliente");
            dgvHistorialCompleto.Columns.Add("Canal", "Canal Cobro");
            dgvHistorialCompleto.Columns.Add("Total", "Monto Total");
            dgvHistorialCompleto.Columns.Add("Estado", "Estado");
            dgvHistorialCompleto.Columns.Add("Items", "Medicamentos");

            pnlVistaHistorial.Controls.AddRange(new Control[]
            {
                lblTit, lblMetricaTotalOperaciones, lblMetricaRecaudacionCaja,
                lblBusOp, txtBuscarOpAbb, btnBuscarOpAbb, dgvHistorialCompleto
            });
            pnlContenedorPrincipal.Controls.Add(pnlVistaHistorial);
        }

        // Prepara la navegación bidireccional y el área de visualización del comprobante.
        private void ConstruirVistaAuditoria()
        {
            pnlVistaAuditoria = new Panel { Dock = DockStyle.Fill, Padding = new Padding(25), Visible = false };

            Label lblTit = new Label { Text = "AUDITORÍA Y ARQUEO BIDIRECCIONAL ", Font = new Font("Segoe UI", 13F, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80), Location = new Point(25, 15), AutoSize = true };

            btnAuditoriaAtras = new Button { Text = "<- Operación Anterior ", Location = new Point(25, 55), Size = new Size(290, 45), BackColor = Color.FromArgb(52, 73, 94), ForeColor = Color.White, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), FlatStyle = FlatStyle.Flat };
            btnAuditoriaAdelante = new Button { Text = "Operación Siguiente -> ", Location = new Point(330, 55), Size = new Size(290, 45), BackColor = Color.FromArgb(52, 73, 94), ForeColor = Color.White, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), FlatStyle = FlatStyle.Flat };

            GroupBox grpTicket = new GroupBox { Text = "", Location = new Point(25, 115), Size = new Size(980, 460) };
            lblTicketAuditado = new Label { Text = "Sin transacciones auditadas.", Font = new Font("Segoe UI", 12F, FontStyle.Bold), ForeColor = Color.FromArgb(41, 128, 185), Location = new Point(20, 25), Size = new Size(940, 30) };
            lblDetalleAuditoria = new Label { Text = "Aún no se han procesado ventas.", Font = new Font("Consolas", 10F), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Location = new Point(20, 65), Size = new Size(940, 370) };

            grpTicket.Controls.AddRange(new Control[] { lblTicketAuditado, lblDetalleAuditoria });
            pnlVistaAuditoria.Controls.AddRange(new Control[] { lblTit, btnAuditoriaAtras, btnAuditoriaAdelante, grpTicket });
            pnlContenedorPrincipal.Controls.Add(pnlVistaAuditoria);
        }

        // Construye la tarjeta promocional, el formulario de alta y la inspección del anillo.
        private void ConstruirVistaCarrusel()
        {
            pnlVistaCarrusel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(25), Visible = false };

            Label lblTit = new Label
            {
                Text = "CARRUSEL DE CAMPAÑAS Y PROMOCIONES ",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(25, 15),
                AutoSize = true
            };
            Label lblSub = new Label
            {
                //Text = "Estructura en anillo: cola.Siguiente enlaza directamente con cabeza, garantizando rotación en tiempo constante O(1).",
                //Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                //ForeColor = Color.FromArgb(127, 140, 141),
                //Location = new Point(27, 40),
                //AutoSize = true
            };

            pnlTarjetonAnuncio = new Panel
            {
                Location = new Point(25, 70),
                Size = new Size(540, 240),
                BackColor = Color.FromArgb(41, 128, 185),
                BorderStyle = BorderStyle.FixedSingle
            };

            lblCardTitulo = new Label { Text = "CAMPAÑA: Cargando...", Font = new Font("Segoe UI", 13F, FontStyle.Bold), ForeColor = Color.White, Location = new Point(20, 15), Size = new Size(500, 35) };
            lblCardBeneficio = new Label { Text = "Beneficio: ...", Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.FromArgb(241, 196, 15), Location = new Point(20, 55), Size = new Size(500, 35) };
            lblCardCanalYVigencia = new Label { Text = "Canal: ... | Vigencia: ...", Font = new Font("Segoe UI", 9.5F), ForeColor = Color.FromArgb(236, 240, 241), Location = new Point(20, 100), Size = new Size(500, 45) };

            btnAvanzarPromoManual = new Button
            {
                Text = "Siguiente Anuncio -> ",
                Location = new Point(20, 170),
                Size = new Size(260, 45),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(41, 128, 185),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnPausarReanudarTimer = new Button
            {
                Text = " Pausar ",
                Location = new Point(290, 170),
                Size = new Size(150, 45),
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            pnlTarjetonAnuncio.Controls.AddRange(new Control[] {
                lblCardTitulo, lblCardBeneficio, lblCardCanalYVigencia, btnAvanzarPromoManual, btnPausarReanudarTimer
            });

            GroupBox grpNuevaPromo = new GroupBox
            {
                Text = "Añadir Nueva Campaña al Anillo Circular",
                Location = new Point(585, 70),
                Size = new Size(420, 240)
            };

            Label lp1 = new Label { Text = "Título Campaña:", Location = new Point(15, 20), AutoSize = true };
            txtNuevaPromoTitulo = new TextBox { Location = new Point(15, 38), Size = new Size(385, 23) };

            Label lp2 = new Label { Text = "Beneficio / Descuento:", Location = new Point(15, 65), AutoSize = true };
            txtNuevaPromoBeneficio = new TextBox { Location = new Point(15, 83), Size = new Size(385, 23) };

            Label lp3 = new Label { Text = "Canal Cobro:", Location = new Point(15, 110), AutoSize = true };
            cmbNuevaPromoCanal = new ComboBox { Location = new Point(15, 128), Size = new Size(185, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbNuevaPromoCanal.Items.AddRange(new object[] { "Yape", "BCP QR", "Agente BCP", "General" });
            cmbNuevaPromoCanal.SelectedIndex = 0;

            Label lp4 = new Label { Text = "Válido Hasta:", Location = new Point(215, 110), AutoSize = true };
            dtpVigenciaPromo = new DateTimePicker
            {
                Location = new Point(215, 128),
                Size = new Size(185, 23),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now.AddDays(15)
            };

            btnAgregarPromoAnillo = new Button
            {
                Text = " + Insertar en el Anillo Circular",
                Location = new Point(15, 175),
                Size = new Size(385, 45),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };

            grpNuevaPromo.Controls.AddRange(new Control[] {
                lp1, txtNuevaPromoTitulo, lp2, txtNuevaPromoBeneficio,
                lp3, cmbNuevaPromoCanal, lp4, dtpVigenciaPromo, btnAgregarPromoAnillo
            });

            Label lblTrazas = new Label
            {
                Text = "Inspección de Enlaces",
                Location = new Point(25, 325),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };
            lstTrazasAnillo = new ListBox
            {
                Location = new Point(25, 350),
                Size = new Size(980, 230),
                Font = new Font("Consolas", 10F),
                BackColor = Color.White
            };

            pnlVistaCarrusel.Controls.AddRange(new Control[] {
                lblTit, lblSub, pnlTarjetonAnuncio, grpNuevaPromo, lblTrazas, lstTrazasAnillo
            });
            pnlContenedorPrincipal.Controls.Add(pnlVistaCarrusel);
        }

        // Carga clientes, productos en el ABB, turnos iniciales en Cola FIFO, red logística (Grafo) y campañas.
        private void CargarDatosSemilla()
        {
            listaClientes.Add(new Cliente("72458912", "Rosmery Mendoza"));
            listaClientes.Add(new Cliente("43891245", "Carlos Alcantara"));
            listaClientes.Add(new Cliente("70123456", "Maria Fernandez"));
            RefrescarComboClientes();

            // Se insertan en orden balanceado (raíz MED-04) para inicializar el Árbol Binario de Búsqueda equilibrado.
            arbolInventario.Insertar(new Producto("MED-04", "Alcohol 70° 500ml", 6.00m, 50, "Antisépticos"));
            arbolInventario.Insertar(new Producto("MED-02", "Amoxicilina Jarabe", 18.50m, 30, "Antibióticos"));
            arbolInventario.Insertar(new Producto("MED-06", "Vitamina C 1000mg", 14.00m, 35, "Vitaminas"));
            arbolInventario.Insertar(new Producto("MED-01", "Paracetamol 500mg", 12.00m, 60, "Analgésicos"));
            arbolInventario.Insertar(new Producto("MED-03", "Ibuprofeno 400mg", 8.50m, 45, "Analgésicos"));
            arbolInventario.Insertar(new Producto("MED-05", "Omeprazol 20mg", 15.00m, 25, "Gastro"));
            arbolInventario.Insertar(new Producto("MED-07", "Suero Oral Pediátrico", 7.50m, 18, "Pediatría"));
            RefrescarComboProductos();
            txtAbbCodigo.Text = GenerarSiguienteCodigoProducto();

            // Se encolan los primeros turnos de espera en la Cola FIFO.
            colaAtencion.Enqueue(new TurnoAtencion(contadorTurnosCola++, listaClientes[0], "Farmacia / Medicamentos"));
            colaAtencion.Enqueue(new TurnoAtencion(contadorTurnosCola++, listaClientes[1], "Agente BCP (Depósito / Retiro)"));
            colaAtencion.Enqueue(new TurnoAtencion(contadorTurnosCola++, listaClientes[2], "Farmacia + Agente BCP"));

            // Se carga el grafo ponderado de sedes y centros médicos de Botica MarFarma en Cajamarca.
            grafoRutas.AgregarVertice(new PuntoRed("SED-01", "Almacén Central", "Almacén Central"));
            grafoRutas.AgregarVertice(new PuntoRed("SED-02", "Botica Plaza Armas", "Sucursal MarFarma"));
            grafoRutas.AgregarVertice(new PuntoRed("SED-03", "Agente BCP Mercado", "Agente BCP"));
            grafoRutas.AgregarVertice(new PuntoRed("SED-04", "Clínica Los Baños", "Centro Médico"));
            grafoRutas.AgregarVertice(new PuntoRed("SED-05", "Sucursal Qhapaq Ñan", "Sucursal MarFarma"));
            grafoRutas.AgregarVertice(new PuntoRed("SED-06", "Hospital Regional", "Centro Médico"));

            grafoRutas.AgregarArista("SED-01", "SED-02", 1.4, 6);
            grafoRutas.AgregarArista("SED-01", "SED-03", 1.1, 5);
            grafoRutas.AgregarArista("SED-02", "SED-03", 1.6, 7);
            grafoRutas.AgregarArista("SED-02", "SED-04", 2.8, 11);
            grafoRutas.AgregarArista("SED-02", "SED-05", 2.2, 9);
            grafoRutas.AgregarArista("SED-03", "SED-05", 1.9, 8);
            grafoRutas.AgregarArista("SED-04", "SED-06", 2.4, 10);
            grafoRutas.AgregarArista("SED-05", "SED-06", 1.5, 6);

            // Se precargan 3 operaciones históricas para inicializar Lista Simple, Lista Doble y Árbol ABB de operaciones.
            var op1 = new TransaccionVenta("OP-1038", listaClientes[0], "Efectivo Mostrador", 14.16m, "1x Paracetamol 500mg;");
            var op2 = new TransaccionVenta("OP-1039", listaClientes[1], "Yape (Billetera Móvil)", 21.83m, "1x Amoxicilina Jarabe;");
            var op3 = new TransaccionVenta("OP-1040", listaClientes[2], "BCP QR Interoperable", 17.70m, "1x Omeprazol 20mg;");
            RegistrarTransaccionEnEstructuras(op1, insertarAbbBalanceado: false);
            RegistrarTransaccionEnEstructuras(op2, insertarAbbBalanceado: false);
            RegistrarTransaccionEnEstructuras(op3, insertarAbbBalanceado: false);
            // Inserción en orden equilibrado (raíz OP-1039) dentro del ABB de operaciones.
            arbolOperaciones.Insertar(op2);
            arbolOperaciones.Insertar(op1);
            arbolOperaciones.Insertar(op3);

            carruselCampanas.InsertarAlFinal(new CampanaSalud("CAMP-01", "Despistaje de Presión Arterial", "Medición gratuita por compras > S/ 20", "General", DateTime.Now.AddDays(7)));
            carruselCampanas.InsertarAlFinal(new CampanaSalud("CAMP-02", "Miércoles de Yape", "15% descuento en jarabes pediátricos", "Yape", DateTime.Now.AddDays(14)));
            carruselCampanas.InsertarAlFinal(new CampanaSalud("CAMP-03", "Campaña Dental MarFarma", "2x1 en pastas y cepillos dentales", "BCP QR", DateTime.Now.AddDays(30)));
            carruselCampanas.InsertarAlFinal(new CampanaSalud("CAMP-04", "Botiquín Seguro Cajamarca", "10% desc. en alcohol y gasas", "General", DateTime.Now.AddDays(60)));
        }

        // Vuelve a cargar los selectores de clientes en Nueva Venta y en Cola FIFO.
        private void RefrescarComboClientes()
        {
            Cliente? seleccionadoVenta = cmbClientes.SelectedItem as Cliente;
            cmbClientes.Items.Clear();
            foreach (var c in listaClientes) cmbClientes.Items.Add(c);
            if (seleccionadoVenta != null && cmbClientes.Items.Contains(seleccionadoVenta))
                cmbClientes.SelectedItem = seleccionadoVenta;
            else if (cmbClientes.Items.Count > 0)
                cmbClientes.SelectedIndex = 0;

            if (cmbColaClientes != null)
            {
                Cliente? seleccionadoCola = cmbColaClientes.SelectedItem as Cliente;
                cmbColaClientes.Items.Clear();
                foreach (var c in listaClientes) cmbColaClientes.Items.Add(c);
                if (seleccionadoCola != null && cmbColaClientes.Items.Contains(seleccionadoCola))
                    cmbColaClientes.SelectedItem = seleccionadoCola;
                else if (cmbColaClientes.Items.Count > 0)
                    cmbColaClientes.SelectedIndex = 0;
            }
        }

        // Sincroniza los selectores de productos desde el recorrido InOrden del Árbol ABB.
        private void RefrescarComboProductos()
        {
            string? codPrevio = (cmbProductos.SelectedItem as Producto)?.Codigo;
            var ordenados = arbolInventario.RecorridoInOrden();

            cmbProductos.Items.Clear();
            foreach (var p in ordenados) cmbProductos.Items.Add(p);

            if (!string.IsNullOrEmpty(codPrevio))
            {
                foreach (var item in cmbProductos.Items)
                {
                    if (item is Producto p && string.Equals(p.Codigo, codPrevio, StringComparison.OrdinalIgnoreCase))
                    {
                        cmbProductos.SelectedItem = item;
                        break;
                    }
                }
            }
            if (cmbProductos.SelectedIndex < 0 && cmbProductos.Items.Count > 0)
            {
                cmbProductos.SelectedIndex = 0;
            }

            if (cmbGrafoProducto != null)
            {
                string? codGrafo = (cmbGrafoProducto.SelectedItem as Producto)?.Codigo;
                cmbGrafoProducto.Items.Clear();
                foreach (var p in ordenados) cmbGrafoProducto.Items.Add(p);
                if (!string.IsNullOrEmpty(codGrafo))
                {
                    foreach (var item in cmbGrafoProducto.Items)
                    {
                        if (item is Producto p && string.Equals(p.Codigo, codGrafo, StringComparison.OrdinalIgnoreCase))
                        {
                            cmbGrafoProducto.SelectedItem = item;
                            break;
                        }
                    }
                }
                if (cmbGrafoProducto.SelectedIndex < 0 && cmbGrafoProducto.Items.Count > 0)
                {
                    cmbGrafoProducto.SelectedIndex = 0;
                }
            }

            ActualizarEtiquetaStockVenta();
        }

        // Muestra el stock disponible en el ABB para el medicamento seleccionado en Nueva Venta.
        private void ActualizarEtiquetaStockVenta()
        {
            if (lblStockProductoVenta == null) return;
            if (cmbProductos.SelectedItem is Producto prod)
            {
                lblStockProductoVenta.Text = prod.Stock > 0
                    ? $"Stock disponible en ABB: {prod.Stock} unidades ({prod.Categoria})"
                    : $"AGOTADO en ABB (0 unidades disponibles)";
                lblStockProductoVenta.ForeColor = prod.Stock == 0
                    ? Color.FromArgb(192, 57, 43)
                    : (prod.Stock <= 10 ? Color.FromArgb(211, 84, 0) : Color.FromArgb(22, 160, 133));
            }
            else
            {
                lblStockProductoVenta.Text = "Stock en ABB: Sin productos cargados";
            }
        }

        // Suma cuántas unidades de un producto ya fueron apiladas en el carrito actual.
        private int ObtenerCantidadEnCarrito(string codigoProducto)
        {
            int acumulado = 0;
            foreach (var d in pilaCarrito.ObtenerElementos())
            {
                if (string.Equals(d.ProductoItem.Codigo, codigoProducto, StringComparison.OrdinalIgnoreCase))
                {
                    acumulado += d.Cantidad;
                }
            }
            return acumulado;
        }

        // Asocia acciones de botones y cambios de selección con las operaciones de la aplicación.
        private void VincularEventos()
        {
            btnNavVenta.Click += (s, e) => CambiarVista(pnlVistaVenta, btnNavVenta);
            btnNavHistorial.Click += (s, e) => { ActualizarGrillaHistorial(); CambiarVista(pnlVistaHistorial, btnNavHistorial); };
            btnNavAuditoria.Click += (s, e) => { RefrescarAuditoriaVisual(); CambiarVista(pnlVistaAuditoria, btnNavAuditoria); };
            btnNavCarrusel.Click += (s, e) => { RefrescarVistaCarrusel(); CambiarVista(pnlVistaCarrusel, btnNavCarrusel); };
            btnNavCola.Click += (s, e) => { RefrescarVistaCola(); CambiarVista(pnlVistaCola, btnNavCola); };
            btnNavInventario.Click += (s, e) => { RefrescarVistaInventario(); CambiarVista(pnlVistaInventario, btnNavInventario); };
            btnNavGrafos.Click += (s, e) => { RefrescarVistaGrafos(); CambiarVista(pnlVistaGrafos, btnNavGrafos); };

            cmbProductos.SelectedIndexChanged += (s, e) => ActualizarEtiquetaStockVenta();

            btnLlamarSiguienteEnVenta.Click += (s, e) =>
            {
                TurnoAtencion? atendido = DesencolarSiguienteTurno();
                if (atendido != null)
                {
                    CargarTurnoEnNuevaVenta(atendido);
                }
            };

            rbClienteExistente.CheckedChanged += (s, e) =>
            {
                bool modo = rbClienteExistente.Checked;
                cmbClientes.Visible = modo;
                txtNuevoClienteNombre.Visible = !modo;
                txtNuevoClienteDni.Visible = !modo;
                btnGuardarCliente.Visible = !modo;
            };

            cmbClientes.SelectedIndexChanged += (s, e) =>
            {
                if (cmbClientes.SelectedItem is Cliente c)
                    lblClienteActivo.Text = $"Atendiendo a: {c.NombreCompleto} | DNI: {c.Dni}";
            };

            btnGuardarCliente.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtNuevoClienteNombre.Text) || string.IsNullOrWhiteSpace(txtNuevoClienteDni.Text))
                {
                    MessageBox.Show("Complete el nombre y DNI del paciente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Cliente nuevo = new Cliente(txtNuevoClienteDni.Text.Trim(), txtNuevoClienteNombre.Text.Trim());
                listaClientes.Add(nuevo);
                RefrescarComboClientes();
                cmbClientes.SelectedItem = nuevo;
                rbClienteExistente.Checked = true;
                txtNuevoClienteNombre.Clear();
                txtNuevoClienteDni.Clear();
            };

            rbProdExistente.CheckedChanged += (s, e) =>
            {
                bool modo = rbProdExistente.Checked;
                cmbProductos.Visible = modo;
                txtNuevoProdNombre.Visible = !modo;
                txtNuevoProdPrecio.Visible = !modo;
                btnGuardarProducto.Visible = !modo;
            };

            btnGuardarProducto.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtNuevoProdNombre.Text) ||
                    !IntentarLeerDecimal(txtNuevoProdPrecio.Text, out decimal precio) ||
                    precio <= 0)
                {
                    MessageBox.Show("Ingrese un medicamento válido y su precio mayor a 0.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string nuevoCodigo = GenerarSiguienteCodigoProducto();
                Producto nuevo = new Producto(nuevoCodigo, txtNuevoProdNombre.Text.Trim(), precio, 20, "Medicamento");
                arbolInventario.Insertar(nuevo);

                RefrescarComboProductos();
                RefrescarVistaInventario();
                ActualizarResumenSidebar();
                cmbProductos.SelectedItem = nuevo;
                rbProdExistente.Checked = true;
                txtNuevoProdNombre.Clear();
                txtNuevoProdPrecio.Clear();
            };

            nudDescuentoPct.ValueChanged += (s, e) => ActualizarCarritoVisual();

            btnBuscarOpAbb.Click += (s, e) =>
            {
                string codigo = txtBuscarOpAbb.Text.Trim().ToUpper();
                if (string.IsNullOrWhiteSpace(codigo))
                {
                    MessageBox.Show("Ingrese un código de operación (ej. OP-1039).", "Consulta ABB", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var nodo = arbolOperaciones.BuscarNodo(new TransaccionVenta(codigo, listaClientes[0], "", 0m, ""));
                if (nodo == null)
                {
                    MessageBox.Show($"No se encontró la operación '{codigo}' en el Árbol ABB.", "No Encontrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                TransaccionVenta t = nodo.Dato;
                foreach (DataGridViewRow fila in dgvHistorialCompleto.Rows)
                {
                    if (string.Equals(fila.Cells["Cod"].Value?.ToString(), t.CodigoOperacion, StringComparison.OrdinalIgnoreCase))
                    {
                        fila.Selected = true;
                        dgvHistorialCompleto.FirstDisplayedScrollingRowIndex = fila.Index;
                        break;
                    }
                }

                MessageBox.Show(
                    $"Operación encontrada en el Árbol ABB:\n\n" +
                    $"Código   : {t.CodigoOperacion}\n" +
                    $"Paciente : {t.ClienteAsociado.NombreCompleto} (DNI: {t.ClienteAsociado.Dni})\n" +
                    $"Canal    : {t.CanalPago}\n" +
                    $"Detalle  : {t.ResumenProductos}\n" +
                    $"Total    : S/ {t.MontoTotal:F2}",
                    "Consulta O(log n) Exitosa",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            };

            btnAgregarAlCarrito.Click += (s, e) =>
            {
                if (cmbProductos.SelectedItem is Producto prod)
                {
                    int cant = (int)nudCantidad.Value;
                    int enCarrito = ObtenerCantidadEnCarrito(prod.Codigo);

                    // Verifica disponibilidad real de stock almacenado en el ABB antes de apilar.
                    if (enCarrito + cant > prod.Stock)
                    {
                        MessageBox.Show(
                            $"Stock insuficiente para '{prod.Nombre}'.\nDisponible en ABB: {prod.Stock} | Ya en carrito: {enCarrito}.",
                            "Control de Inventario",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }

                    DetalleVenta item = new DetalleVenta(prod, cant);
                    pilaCarrito.Push(item); // O(1)
                    ActualizarCarritoVisual();
                }
            };

            btnDeshacerPila.Click += (s, e) =>
            {
                DetalleVenta? retirado = pilaCarrito.Pop(); // O(1)
                if (retirado != null)
                {
                    MessageBox.Show($"Se anuló '{retirado.ProductoItem.Nombre}' del carrito.", "Deshacer Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ActualizarCarritoVisual();
                }
                else
                {
                    MessageBox.Show("No hay medicamentos para deshacer.", "Pila Vacía", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            btnLimpiarCarrito.Click += (s, e) => { pilaCarrito.Limpiar(); nudDescuentoPct.Value = 0; ActualizarCarritoVisual(); };

            btnProcesarCobro.Click += (s, e) =>
            {
                if (pilaCarrito.EstaVacia())
                {
                    MessageBox.Show("El carrito de venta se encuentra vacío.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Cliente? cli = cmbClientes.SelectedItem as Cliente;
                if (cli == null)
                {
                    MessageBox.Show("Seleccione o registre un paciente antes de procesar el cobro.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Revalida el stock consolidado por producto en caso de haberse modificado desde la vista ABB.
                var acumuladosPorCodigo = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                foreach (var d in pilaCarrito.ObtenerElementos())
                {
                    acumuladosPorCodigo[d.ProductoItem.Codigo] = acumuladosPorCodigo.GetValueOrDefault(d.ProductoItem.Codigo) + d.Cantidad;
                    if (acumuladosPorCodigo[d.ProductoItem.Codigo] > d.ProductoItem.Stock)
                    {
                        MessageBox.Show($"Stock insuficiente para '{d.ProductoItem.Nombre}' (Disponible: {d.ProductoItem.Stock}).", "Inventario ABB", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                string canal = cmbCanalCobro.SelectedItem?.ToString() ?? "Efectivo Mostrador";
                decimal total = CalcularTotal();

                string resumen = "";
                foreach (var d in pilaCarrito.ObtenerElementos())
                {
                    resumen += $"{d.Cantidad}x {d.ProductoItem.Nombre}; ";
                    d.ProductoItem.Stock -= d.Cantidad; // Descuenta stock directamente en el nodo del ABB.
                }

                string codigoOp = $"OP-{contadorOperaciones++}";
                TransaccionVenta nueva = new TransaccionVenta(codigoOp, cli, canal, total, resumen);

                // Registra la operación en Lista Simple (historial), Lista Doble (auditoría) y ABB (búsqueda por código).
                RegistrarTransaccionEnEstructuras(nueva, insertarAbbBalanceado: true);
                MessageBox.Show($"Transacción {codigoOp} procesada vía {canal}.\nMonto: S/ {total:F2}", "Venta Confirmada", MessageBoxButtons.OK, MessageBoxIcon.Information);

                pilaCarrito.Limpiar();
                nudDescuentoPct.Value = 0;
                ActualizarCarritoVisual();
                RefrescarComboProductos();
                RefrescarVistaInventario();
                ActualizarResumenSidebar();
            };

            btnAuditoriaAtras.Click += (s, e) => {
                TransaccionVenta? anterior = listaHistorial.Retroceder();
                RefrescarAuditoriaVisual();
                if (anterior == null) MessageBox.Show("Inicio del registro alcanzado.", "Auditoría", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            btnAuditoriaAdelante.Click += (s, e) => {
                TransaccionVenta? siguiente = listaHistorial.Avanzar();
                RefrescarAuditoriaVisual();
                if (siguiente == null) MessageBox.Show("Última operación alcanzada.", "Auditoría", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            btnAvanzarPromoManual.Click += (s, e) => AvanzarCarruselCircular();

            btnPausarReanudarTimer.Click += (s, e) =>
            {
                timerActivo = !timerActivo;
                if (timerActivo)
                {
                    timerCarrusel.Start();
                    btnPausarReanudarTimer.Text = " Pausar Timer";
                    btnPausarReanudarTimer.BackColor = Color.FromArgb(52, 73, 94);
                }
                else
                {
                    timerCarrusel.Stop();
                    btnPausarReanudarTimer.Text = " Reanudar Timer";
                    btnPausarReanudarTimer.BackColor = Color.FromArgb(39, 174, 96);
                }
            };

            btnAgregarPromoAnillo.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtNuevaPromoTitulo.Text) || string.IsNullOrWhiteSpace(txtNuevaPromoBeneficio.Text))
                {
                    MessageBox.Show("Complete el título y beneficio de la campaña.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                CampanaSalud nueva = new CampanaSalud(
                    $"CAMP-{contadorCampanas++:D2}",
                    txtNuevaPromoTitulo.Text.Trim(),
                    txtNuevaPromoBeneficio.Text.Trim(),
                    cmbNuevaPromoCanal.SelectedItem?.ToString() ?? "General",
                    dtpVigenciaPromo.Value
                );

                carruselCampanas.InsertarAlFinal(nueva); // O(1)
                RefrescarVistaCarrusel();

                txtNuevaPromoTitulo.Clear();
                txtNuevaPromoBeneficio.Clear();
                MessageBox.Show($"Campaña '{nueva.Titulo}' añadida al anillo circular con vigencia hasta {nueva.FechaVigenciaFin:dd/MM/yyyy}.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
        }

        // Inserta una venta en Lista Simple (historial general), Lista Doble (auditoría) y opcionalmente en el ABB de operaciones.
        private void RegistrarTransaccionEnEstructuras(TransaccionVenta transaccion, bool insertarAbbBalanceado)
        {
            listaSimpleHistorial.InsertarAlFinal(transaccion);
            listaHistorial.InsertarAlFinal(transaccion);
            if (insertarAbbBalanceado)
            {
                arbolOperaciones.Insertar(transaccion);
            }
        }

        // Lee valores decimales aceptando punto o coma como separador según el teclado del usuario.
        private static bool IntentarLeerDecimal(string texto, out decimal valor)
        {
            string normalizado = (texto ?? string.Empty).Trim().Replace(',', '.');
            return decimal.TryParse(normalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out valor);
        }

        // Mueve la campaña actual al siguiente nodo del anillo y refresca sus representaciones.
        private void AvanzarCarruselCircular()
        {
            CampanaSalud? promo = carruselCampanas.AvanzarSiguiente(); // O(1)
            ActualizarBannerEnPantalla();
            if (pnlVistaCarrusel.Visible)
            {
                RefrescarVistaCarrusel();
            }
        }

        // Copia los datos de la campaña actual a los banners de venta y del carrusel.
        private void ActualizarBannerEnPantalla()
        {
            CampanaSalud? promo = carruselCampanas.ObtenerActual();
            if (promo != null)
            {
                lblBannerPromocionVenta.Text = $" [{promo.CanalAsociado}] {promo.Titulo}: {promo.BeneficioDescuento} (Válido hasta: {promo.FechaVigenciaFin:dd/MM/yyyy})";
                lblCardTitulo.Text = $"CAMPAÑA: {promo.Titulo}";
                lblCardBeneficio.Text = $"Beneficio: {promo.BeneficioDescuento}";
                lblCardCanalYVigencia.Text = $"Canal: {promo.CanalAsociado} | Registrado: {promo.FechaRegistro:dd/MM/yyyy HH:mm} | Vence: {promo.FechaVigenciaFin:dd/MM/yyyy}";
            }
        }

        // Actualiza la tarjeta actual y reconstruye la lista de enlaces del anillo.
        private void RefrescarVistaCarrusel()
        {
            ActualizarBannerEnPantalla();
            lstTrazasAnillo.Items.Clear();
            foreach (var traza in carruselCampanas.ObtenerTrazasDelAnillo())
                lstTrazasAnillo.Items.Add(traza);
        }

        // Reconstruye la tabla del carrito y evalúa subtotal, descuento e IGV usando notación postfija con Pila.
        private void ActualizarCarritoVisual()
        {
            dgvCarrito.Rows.Clear();
            decimal sub = 0;
            foreach (var d in pilaCarrito.ObtenerElementos())
            {
                dgvCarrito.Rows.Add(d.ProductoItem.Nombre, d.Cantidad, $"S/ {d.ProductoItem.PrecioUnitario:F2}", $"S/ {d.Subtotal:F2}");
                sub += d.Subtotal;
            }

            decimal descuento = sub * (nudDescuentoPct.Value / 100m);
            decimal baseImponible = sub - descuento;
            decimal igv = baseImponible * 0.18m;

            // Convierte y evalúa la fórmula aritmética mediante Pila LIFO en notación postfija.
            string infija = string.Format(CultureInfo.InvariantCulture, "( {0:F2} - {1:F2} ) * 1.18", sub, descuento);
            string postfija = CalculadoraPostfija.ConvertirInfijaAPostfija(infija);
            decimal tot = sub > 0 ? Math.Round(CalculadoraPostfija.EvaluarPostfija(postfija), 2) : 0m;

            lblFormulaPostfija.Text = sub > 0
                ? $"Infija  : {infija}\nPostfija: {postfija}"
                : "Postfija (Pila): 0.00";

            lblSubtotalVenta.Text = $"Subtotal: S/ {sub:F2}";
            lblIgvVenta.Text = $"IGV (18%): S/ {igv:F2}";
            lblTotalVenta.Text = $"TOTAL: S/ {tot:F2}";
        }

        // Evalúa el total a cobrar aplicando descuento e IGV mediante la Pila de notación postfija.
        private decimal CalcularTotal()
        {
            decimal sub = 0;
            foreach (var d in pilaCarrito.ObtenerElementos()) sub += d.Subtotal;
            if (sub <= 0) return 0m;

            decimal descuento = sub * (nudDescuentoPct.Value / 100m);
            string infija = string.Format(CultureInfo.InvariantCulture, "( {0:F2} - {1:F2} ) * 1.18", sub, descuento);
            string postfija = CalculadoraPostfija.ConvertirInfijaAPostfija(infija);
            return Math.Round(CalculadoraPostfija.EvaluarPostfija(postfija), 2);
        }

        // Vuelca las transacciones de la Lista Simple en la tabla y actualiza las métricas.
        private void ActualizarGrillaHistorial()
        {
            dgvHistorialCompleto.Rows.Clear();
            decimal recaudacion = 0;
            var historial = listaSimpleHistorial.ObtenerHistorialCompleto();
            foreach (var t in historial)
            {
                dgvHistorialCompleto.Rows.Add(t.CodigoOperacion, t.FechaHora.ToString("HH:mm:ss"), t.ClienteAsociado.NombreCompleto, t.CanalPago, $"S/ {t.MontoTotal:F2}", t.Estado, t.ResumenProductos);
                recaudacion += t.MontoTotal;
            }
            lblMetricaTotalOperaciones.Text = $"Operaciones Totales: {historial.Length}";
            lblMetricaRecaudacionCaja.Text = $"Caja Total: S/ {recaudacion:F2}";
        }

        // Presenta los datos de la transacción seleccionada en la Lista Doble o el mensaje de historial vacío.
        private void RefrescarAuditoriaVisual()
        {
            TransaccionVenta? actual = listaHistorial.ObtenerActual();
            if (actual != null)
            {
                lblTicketAuditado.Text = $"AUDITANDO: {actual.CodigoOperacion} | {actual.CanalPago} | Estado: {actual.Estado}";
                lblDetalleAuditoria.Text =
                    $"========================================================\n" +
                    $" COMPROBANTE ELECTRÓNICO - BOTICA MARFARMA\n" +
                    $"========================================================\n" +
                    $" Código Operación    : {actual.CodigoOperacion}\n" +
                    $" Fecha y Hora        : {actual.FechaHora:dd/MM/yyyy HH:mm:ss}\n" +
                    $" Paciente            : {actual.ClienteAsociado.NombreCompleto}\n" +
                    $" DNI                 : {actual.ClienteAsociado.Dni}\n" +
                    $" Canal de Pago       : {actual.CanalPago}\n" +
                    $" Estado              : {actual.Estado}\n" +
                    $"--------------------------------------------------------\n" +
                    $" Medicamentos:\n {actual.ResumenProductos}\n" +
                    $"--------------------------------------------------------\n" +
                    $" TOTAL REGISTRADO    : S/ {actual.MontoTotal:F2}\n" +
                    $"========================================================";
            }
            else
            {
                lblTicketAuditado.Text = "Sin transacciones auditadas.";
                lblDetalleAuditoria.Text = "Aún no se han procesado operaciones .";
            }
        }
    }
}