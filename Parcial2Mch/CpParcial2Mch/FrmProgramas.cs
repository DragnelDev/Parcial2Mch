using CadParcial2;
using CadParcial2Mch;
using ClnParcial2Mch;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Parcial2Mch
{
    public partial class FrmProgramas : Form
    {
        private bool esNuevo = false;
        public FrmProgramas()
        {
            InitializeComponent();
        }
        private void listar()
        {
            var lista = ProgramaCln.listarPa(txtParametro.Text.Trim());
            dgvLista.DataSource = lista;
            dgvLista.Columns["id"].Visible = false;                     
            dgvLista.Columns["idCanal"].Visible = false;
            dgvLista.Columns["estadoRegistro"].Visible = false;

            dgvLista.Columns["titulo"].HeaderText = "Título";                   
            dgvLista.Columns["descripcion"].HeaderText = "Descripción";
            dgvLista.Columns["duracion"].HeaderText = "Duración (min)";
            dgvLista.Columns["productor"].HeaderText = "Productor";
            dgvLista.Columns["fechaEstreno"].HeaderText = "Fecha Estreno";
            dgvLista.Columns["estado"].HeaderText = "Estado";
            dgvLista.Columns["clasificacion"].HeaderText = "Clasificación";
            dgvLista.Columns["usuarioRegistro"].HeaderText = "Usuario Registro";
            dgvLista.Columns["fechaRegistro"].HeaderText = "Fecha Registro";

            dgvLista.Columns["nombreCanal"].HeaderText = "Canal";

            if (lista.Count > 0) dgvLista.CurrentCell = dgvLista.Rows[0].Cells["Titulo"];
            btnEditar.Enabled = lista.Count > 0;
            btnEliminar.Enabled = lista.Count > 0;
        }

        private void cargarCanal()
        {
            var lista = CanalCln.listar();
            cbxCanal.DataSource = lista;
            cbxCanal.ValueMember = "id";
            cbxCanal.DisplayMember = "nombre"; 
            cbxCanal.SelectedIndex = -1;
        }

        private void FrmProgramas_Load(object sender, EventArgs e)
        {
            Size = new Size(763, 342);
            listar();
            cargarCanal();

            cbxEstado.DataSource = new List<KeyValuePair<int, string>> {
            new KeyValuePair<int, string>(1, "En emisión"),
            new KeyValuePair<int, string>(2, "Finalizado"),
            new KeyValuePair<int, string>(3, "Suspendido")
            };
            cbxEstado.DisplayMember = "Value";
            cbxEstado.ValueMember = "Key";
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            esNuevo = true;
            pnlAcciones.Enabled = false;
            Size = new Size(763, 553);
            txtTitulo.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            esNuevo = false;
            pnlAcciones.Enabled = false;
            Size = new Size(763, 553);

            int id = (int)dgvLista.CurrentRow.Cells["id"].Value;
            var programa = ProgramaCln.obtenerUno(id);

            cbxCanal.SelectedValue = programa.idCanal;
            txtTitulo.Text = programa.titulo;
            txtDescripcion.Text = programa.descripcion;
            nudDuracion.Value = (decimal)programa.duracion;
            txtProductor.Text = programa.productor;
            dtpFechaEstreno.Value = (DateTime)programa.fechaEstreno;
            cbxEstado.SelectedValue = Convert.ToInt32(programa.estado);
            cbxClasificacion.Text = programa.clasificacion;

            txtTitulo.Focus();
        }

        private void limpiar()
        {
            txtTitulo.Clear();
            txtDescripcion.Clear();
            cbxCanal.SelectedIndex = -1;
            nudDuracion.Value = 60;
            txtProductor.Clear();
            dtpFechaEstreno.Value = DateTime.Now;
            cbxEstado.SelectedIndex = -1;   
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Size = new Size(763, 342);
            pnlAcciones.Enabled = true;
            limpiar();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            listar();
        }

        private void txtParametro_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter) listar();
        }

        private bool validar()
        {
            bool esValido = true;

            erpTitulo.Clear();
            erpCanal.Clear();
            erpDescripcion.Clear();
            erpDuracion.Clear();
            erpProductor.Clear();
            erpFechaEstreno.Clear();
            erpEstado.Clear();  
            erpClasificacion.Clear();

            if (string.IsNullOrEmpty(txtTitulo.Text))
            {
                erpTitulo.SetError(txtTitulo, "El título es obligatorio");
                esValido = false;
            }

            if (cbxCanal.SelectedIndex == -1)
            {
                erpCanal.SetError(cbxCanal, "Debe seleccionar un canal");
                esValido = false;
            }

            if (string.IsNullOrEmpty(txtDescripcion.Text))
            {
                erpDescripcion.SetError(txtDescripcion, "La descripción es obligatoria");
                esValido = false;
            }

            if (nudDuracion.Value == 0)
            {
                erpDuracion.SetError(nudDuracion, "La duración debe ser mayor a cero");
                esValido = false;
            }

            if (string.IsNullOrEmpty(txtProductor.Text))
            {
                erpProductor.SetError(txtProductor, "El productor es obligatorio");
                esValido = false;
            }

            if (dtpFechaEstreno.Value == null)
            {
                erpFechaEstreno.SetError(dtpFechaEstreno, "Debe seleccionar fecha de estreno");
                esValido = false;
            }

            if (cbxEstado.SelectedIndex == -1)
            {
                erpEstado.SetError(cbxEstado, "Debe seleccionar un estado");
                esValido = false;
            }

            return esValido;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (validar())
            {
                var programa = new Programa();
                programa.titulo = txtTitulo.Text.Trim();
                programa.descripcion = txtDescripcion.Text.Trim();
                programa.idCanal = (int)cbxCanal.SelectedValue;
                programa.duracion = (int)nudDuracion.Value;
                programa.productor = txtProductor.Text.Trim();
                programa.fechaEstreno = dtpFechaEstreno.Value;
                programa.usuarioRegistro = "admin";

                programa.estado = Convert.ToInt16(cbxEstado.SelectedValue);
                programa.clasificacion = cbxClasificacion.Text;
                programa.estadoRegistro = 1;

                if (esNuevo)
                {
                    programa.fechaRegistro = DateTime.Now;
                    ProgramaCln.insertar(programa);
                }
                else
                {
                    programa.id = (int)dgvLista.CurrentRow.Cells["id"].Value;
                    ProgramaCln.actualizar(programa);
                }

                listar();
                btnCancelar.PerformClick();

                MessageBox.Show("Programa guardado correctamente",
                    "::: Mensaje - Parcial2Mch :::",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvLista.CurrentRow == null || dgvLista.CurrentRow.Cells["id"].Value == null)
            {
                MessageBox.Show("Seleccione un programa para eliminar.",
                                "::: Mensaje - Parcial2Mch :::",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = (int)dgvLista.CurrentRow.Cells["id"].Value;
            string titulo = dgvLista.CurrentRow.Cells["titulo"].Value?.ToString() ?? "(sin título)";

            var confirm = MessageBox.Show(
                $"¿Está seguro de eliminar el programa '{titulo}'?",
                "::: Confirmar eliminación :::",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm != DialogResult.Yes) return;

            try
            {
                ProgramaCln.eliminar(id, "admin");

                listar();

                MessageBox.Show(
                    $"El programa '{titulo}' fue eliminado correctamente.",
                    "::: Mensaje - Parcial2Mch :::",
                    MessageBoxButtons.OK, MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ocurrió un error al eliminar el programa:\n" + ex.Message,
                    "::: Error :::",
                    MessageBoxButtons.OK, MessageBoxIcon.Error
                );
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
