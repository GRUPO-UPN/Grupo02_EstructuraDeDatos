using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BoticaMarFarmaTransaccional.Entidades;

namespace BoticaMarFarmaTransaccional
{
    public partial class Form1
    {
        // Panel principal y controles de la vista de Cola de Atención (FIFO).
        private Panel pnlVistaCola = null!;
        private RadioButton rbColaCliExistente = null!;
        private RadioButton rbColaCliNuevo = null!;
        private ComboBox cmbColaClientes = null!;
        private TextBox txtColaNuevoNombre = null!;
        private TextBox txtColaNuevoDni = null!;
        private ComboBox cmbColaServicio = null!;
        private Button btnEncolarPaciente = null!;
        private Label lblColaTotalEspera = null!;
        private Label lblColaProximoFrente = null!;
        private Label lblColaCadenaTexto = null!;
        private Label lblColaUltimoAtendido = null!;
        private Button btnColaAtenderYVenta = null!;
        private Button btnColaDequeue = null!;
        private Button btnColaVaciar = null!;
        private DataGridView dgvColaTurnos = null!;

        // Controles integrados dentro de Nueva Venta para llamar turnos y ver stock.
        private Label lblEstadoColaEnVenta = null!;
        private Button btnLlamarSiguienteEnVenta = null!;
        private Label lblStockProductoVenta = null!;

        // Construye la interfaz de gestión de turnos con Cola FIFO (sin dibujos).
        private void ConstruirVistaCola()
        {
            pnlVistaCola = new Panel { Dock = DockStyle.Fill, Padding = new Padding(25), Visible = false };

            Label lblTit = new Label
            {
                Text = "COLA DE ATENCIÓN AL PACIENTE (ESTRUCTURA FIFO)",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(25, 15),
                AutoSize = true
            };
            Label lblSub = new Label
            {
                Text = "Atención por orden de llegada: Enqueue inserta al Final y Dequeue atiende desde el Frente en tiempo constante O(1).",
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(27, 40),
                AutoSize = true
            };

            // Resumen superior en texto del estado actual de los punteros frente y final.
            GroupBox grpEstadoCola = new GroupBox
            {
                Text = "Estado de los Enlaces (Frente -> Final)",
                Location = new Point(25, 65),
                Size = new Size(980, 110)
            };

            lblColaTotalEspera = new Label
            {
                Text = "En Espera: 0",
                Location = new Point(15, 25),
                Size = new Size(165, 36),
                BackColor = Color.FromArgb(240, 243, 244),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                TextAlign = ContentAlignment.MiddleCenter
            };

            lblColaProximoFrente = new Label
            {
                Text = "Frente (Front): Cola vacía",
                Location = new Point(190, 25),
                Size = new Size(775, 36),
                BackColor = Color.FromArgb(232, 246, 243),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(22, 160, 133),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 10, 0)
            };

            lblColaCadenaTexto = new Label
            {
                Text = "Cadena FIFO: (vacía)",
                Location = new Point(15, 68),
                Size = new Size(950, 30),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 9.5F),
                ForeColor = Color.FromArgb(44, 62, 80),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 8, 0)
            };

            grpEstadoCola.Controls.AddRange(new Control[] { lblColaTotalEspera, lblColaProximoFrente, lblColaCadenaTexto });

            // Panel izquierdo: registrar llegada (Enqueue).
            GroupBox grpEncolar = new GroupBox
            {
                Text = "Registrar Llegada (Enqueue)",
                Location = new Point(25, 185),
                Size = new Size(420, 215)
            };

            rbColaCliExistente = new RadioButton { Text = "Cliente Registrado", Location = new Point(15, 25), AutoSize = true, Checked = true };
            rbColaCliNuevo = new RadioButton { Text = "Nuevo Cliente", Location = new Point(180, 25), AutoSize = true };

            cmbColaClientes = new ComboBox { Location = new Point(15, 52), Size = new Size(390, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            txtColaNuevoNombre = new TextBox { Location = new Point(15, 52), Size = new Size(245, 23), PlaceholderText = "Nombre completo", Visible = false };
            txtColaNuevoDni = new TextBox { Location = new Point(265, 52), Size = new Size(140, 23), PlaceholderText = "DNI", Visible = false };

            Label lblServ = new Label { Text = "Servicio solicitado:", Location = new Point(15, 86), AutoSize = true };
            cmbColaServicio = new ComboBox { Location = new Point(15, 106), Size = new Size(390, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbColaServicio.Items.AddRange(new object[]
            {
                "Farmacia / Medicamentos",
                "Agente BCP (Depósito / Retiro)",
                "Yape / Billetera Móvil",
                "Farmacia + Agente BCP"
            });
            cmbColaServicio.SelectedIndex = 0;

            btnEncolarPaciente = new Button
            {
                Text = "+ Encolar Turno (Enqueue)",
                Location = new Point(15, 148),
                Size = new Size(390, 45),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            grpEncolar.Controls.AddRange(new Control[]
            {
                rbColaCliExistente, rbColaCliNuevo, cmbColaClientes,
                txtColaNuevoNombre, txtColaNuevoDni, lblServ, cmbColaServicio, btnEncolarPaciente
            });

            // Panel derecho: atención de turnos (Dequeue) y vaciado (Clear).
            GroupBox grpControlCola = new GroupBox
            {
                Text = "Despacho de Turnos y Control FIFO",
                Location = new Point(460, 185),
                Size = new Size(545, 215)
            };

            lblColaUltimoAtendido = new Label
            {
                Text = "Último atendido: Aún no se han despachado turnos.",
                Location = new Point(15, 32),
                Size = new Size(515, 45),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(52, 73, 94),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 10, 0)
            };

            btnColaAtenderYVenta = new Button
            {
                Text = "Atender Siguiente -> Ir a Venta",
                Location = new Point(15, 95),
                Size = new Size(515, 45),
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            btnColaDequeue = new Button
            {
                Text = "Desencolar Aquí (Dequeue)",
                Location = new Point(15, 152),
                Size = new Size(320, 42),
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            btnColaVaciar = new Button
            {
                Text = "Vaciar (Clear)",
                Location = new Point(345, 152),
                Size = new Size(185, 42),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            grpControlCola.Controls.AddRange(new Control[]
            {
                lblColaUltimoAtendido, btnColaAtenderYVenta, btnColaDequeue, btnColaVaciar
            });

            // Tabla inferior con el listado completo de turnos en espera.
            dgvColaTurnos = new DataGridView
            {
                Location = new Point(25, 412),
                Size = new Size(980, 235),
                BackgroundColor = Color.White,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvColaTurnos.Columns.Add("Posicion", "Posición");
            dgvColaTurnos.Columns.Add("Turno", "N° Turno");
            dgvColaTurnos.Columns.Add("Paciente", "Paciente / Cliente");
            dgvColaTurnos.Columns.Add("Dni", "DNI");
            dgvColaTurnos.Columns.Add("Servicio", "Servicio Solicitado");
            dgvColaTurnos.Columns.Add("Hora", "Hora Llegada");

            pnlVistaCola.Controls.AddRange(new Control[]
            {
                lblTit, lblSub, grpEstadoCola, grpEncolar, grpControlCola, dgvColaTurnos
            });
            pnlContenedorPrincipal.Controls.Add(pnlVistaCola);
        }

        // Vincula los botones de encolar, desencolar, pasar a venta y vaciar la cola.
        private void VincularEventosCola()
        {
            rbColaCliExistente.CheckedChanged += (s, e) =>
            {
                bool modo = rbColaCliExistente.Checked;
                cmbColaClientes.Visible = modo;
                txtColaNuevoNombre.Visible = !modo;
                txtColaNuevoDni.Visible = !modo;
            };

            btnEncolarPaciente.Click += (s, e) =>
            {
                Cliente? clienteTurno;

                if (rbColaCliExistente.Checked)
                {
                    clienteTurno = cmbColaClientes.SelectedItem as Cliente;
                    if (clienteTurno == null)
                    {
                        MessageBox.Show("Seleccione un cliente registrado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(txtColaNuevoNombre.Text) || string.IsNullOrWhiteSpace(txtColaNuevoDni.Text))
                    {
                        MessageBox.Show("Complete nombre y DNI del nuevo paciente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    clienteTurno = new Cliente(txtColaNuevoDni.Text.Trim(), txtColaNuevoNombre.Text.Trim());
                    listaClientes.Add(clienteTurno);
                    RefrescarComboClientes();
                    txtColaNuevoNombre.Clear();
                    txtColaNuevoDni.Clear();
                    rbColaCliExistente.Checked = true;
                    cmbColaClientes.SelectedItem = clienteTurno;
                }

                string servicio = cmbColaServicio.SelectedItem?.ToString() ?? "Farmacia / Medicamentos";
                TurnoAtencion nuevoTurno = new TurnoAtencion(contadorTurnosCola++, clienteTurno, servicio);
                colaAtencion.Enqueue(nuevoTurno);

                RefrescarVistaCola();
                ActualizarResumenSidebar();
            };

            btnColaDequeue.Click += (s, e) =>
            {
                TurnoAtencion? atendido = DesencolarSiguienteTurno();
                if (atendido != null)
                {
                    MessageBox.Show(
                        $"Turno {atendido.EtiquetaCorta} atendido: {atendido.Paciente.NombreCompleto}\nServicio: {atendido.TipoServicio}",
                        "Atención Completada (Dequeue)",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            };

            btnColaAtenderYVenta.Click += (s, e) =>
            {
                TurnoAtencion? atendido = DesencolarSiguienteTurno();
                if (atendido != null)
                {
                    CargarTurnoEnNuevaVenta(atendido);
                    CambiarVista(pnlVistaVenta, btnNavVenta);
                }
            };

            btnColaVaciar.Click += (s, e) =>
            {
                if (colaAtencion.EstaVacia())
                {
                    MessageBox.Show("La cola ya está vacía.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                colaAtencion.Clear();
                RefrescarVistaCola();
                ActualizarResumenSidebar();
            };
        }

        // Extrae el primer turno en espera (FIFO) y actualiza la pantalla.
        private TurnoAtencion? DesencolarSiguienteTurno()
        {
            if (colaAtencion.EstaVacia())
            {
                MessageBox.Show("No hay pacientes esperando en la cola.", "Cola Vacía", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            TurnoAtencion atendido = colaAtencion.Dequeue();
            ultimoTurnoAtendido = atendido;
            RefrescarVistaCola();
            ActualizarResumenSidebar();
            return atendido;
        }

        // Carga el paciente desencolado directamente en la pestaña Nueva Venta.
        private void CargarTurnoEnNuevaVenta(TurnoAtencion turno)
        {
            rbClienteExistente.Checked = true;
            if (!cmbClientes.Items.Contains(turno.Paciente))
            {
                listaClientes.Add(turno.Paciente);
                RefrescarComboClientes();
            }

            cmbClientes.SelectedItem = turno.Paciente;
            lblClienteActivo.Text = $"Atendiendo Turno {turno.EtiquetaCorta}: {turno.Paciente.NombreCompleto} | DNI: {turno.Paciente.Dni}";

            if (turno.TipoServicio.Contains("Yape"))
            {
                cmbCanalCobro.SelectedIndex = 0;
            }
            else if (turno.TipoServicio.Contains("Agente BCP"))
            {
                cmbCanalCobro.SelectedIndex = 2;
            }
        }

        // Actualiza los indicadores de texto y la tabla de turnos en espera.
        private void RefrescarVistaCola()
        {
            List<TurnoAtencion> elementos = colaAtencion.ObtenerElementos();

            lblColaTotalEspera.Text = $"En Espera: {colaAtencion.Longitud()}";
            if (!colaAtencion.EstaVacia())
            {
                TurnoAtencion frente = colaAtencion.Front();
                lblColaProximoFrente.Text = $"Frente (Front): {frente.EtiquetaCorta} - {frente.Paciente.NombreCompleto} ({frente.TipoServicio})";
                lblEstadoColaEnVenta.Text = $"En cola: {colaAtencion.Longitud()} | Próximo: {frente.EtiquetaCorta} {frente.Paciente.NombreCompleto}";

                List<string> cadena = new List<string>();
                for (int i = 0; i < elementos.Count; i++)
                {
                    string nombre = elementos[i].Paciente.NombreCompleto.Split(' ')[0];
                    cadena.Add($"{elementos[i].EtiquetaCorta} {nombre}");
                }
                lblColaCadenaTexto.Text = $"FRENTE -> {string.Join(" -> ", cadena)} -> null (FINAL)";
            }
            else
            {
                lblColaProximoFrente.Text = "Frente (Front): Cola vacía";
                lblEstadoColaEnVenta.Text = "Cola FIFO vacía: no hay pacientes esperando.";
                lblColaCadenaTexto.Text = "FRENTE -> null (sin pacientes en espera)";
            }

            lblColaUltimoAtendido.Text = ultimoTurnoAtendido != null
                ? $"Último atendido: {ultimoTurnoAtendido.EtiquetaCorta} - {ultimoTurnoAtendido.Paciente.NombreCompleto} ({ultimoTurnoAtendido.TipoServicio})"
                : "Último atendido: Aún no se han despachado turnos.";

            dgvColaTurnos.Rows.Clear();
            for (int i = 0; i < elementos.Count; i++)
            {
                TurnoAtencion t = elementos[i];
                string pos = i == 0 && elementos.Count == 1
                    ? "FRENTE / FINAL"
                    : (i == 0 ? "FRENTE" : (i == elementos.Count - 1 ? "FINAL" : $"Posición {i + 1}"));

                dgvColaTurnos.Rows.Add(
                    pos,
                    t.EtiquetaCorta,
                    t.Paciente.NombreCompleto,
                    t.Paciente.Dni,
                    t.TipoServicio,
                    t.HoraLlegada.ToString("HH:mm:ss"));
            }
        }
    }
}
