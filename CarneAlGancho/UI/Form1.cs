using BE;
using BLL;
using Servicios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI
{
    public partial class Form1 : Form
    {
        UsuarioBLL usuarioBLL;
        public Form1()
        {
            InitializeComponent();
            usuarioBLL = new UsuarioBLL();
            this.Load += Form1_Load;
            this.FormClosing += Form1_FormClosing;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            UpdateDVV();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                bool integrityOK = usuarioBLL.CheckUsersDVH();

                if (integrityOK)
                {
                    lblIntegridad.Text = "Integridad: OK";
                }
                else
                {
                    MessageBox.Show("¡ADVERTENCIA! Se detectaron inconsistencias en la tabla de usuarios. Contacte al administrador.", "Alerta de Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error inesperado al iniciar la aplicación: {ex.Message}\nLa aplicación se cerrará.", "Error General", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private void btnIniciarSesion_Click(object sender, EventArgs e)
        {
            /*ServicioEncriptador.CreateHash(txtClave.Text, out string hash, out string salt);
            MessageBox.Show($"Hash: {hash} - Salt: {salt}");
            return;*/

            try
            {
                string Email = txtUsuario.Text;
                string PasswordHash = txtClave.Text;

                var resultado = usuarioBLL.Login(Email, PasswordHash);

                if (resultado == LoginResult.ValidUser)
                {
                    MessageBox.Show("Iniciaste con exito");
                    btnCerrarSesion.Visible = true;
                }
            }
            catch (LoginException ex)
            {
                switch (ex.Result)
                {
                    case LoginResult.InvalidUsername:
                        MessageBox.Show("Usuario no encontrado");
                        break;
                    case LoginResult.InvalidPassword:
                        MessageBox.Show("Clave incorrecta");
                        break;
                    case LoginResult.BloquedUser:
                        MessageBox.Show("Usuario bloqueado, cagaste pa");
                        btnDesbloquear.Visible = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdateDVV()
        {
            try
            {
                usuarioBLL.CalculateDVV();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al registrar el DVV al cerrar: {ex.Message}");

            }
        }

        private void btnCerrarSesion_Click_1(object sender, EventArgs e)
        {
            try
            {
                UpdateDVV();
                usuarioBLL.Logout();
                MessageBox.Show("Sesion cerrada");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDesbloquear_Click_1(object sender, EventArgs e)
        {
            if (usuarioBLL.UnblockUser(txtUsuario.Text) > 0)
            {
                btnDesbloquear.Visible = false;
                MessageBox.Show("Usuario desbloqueado");
            }
        }
    }
}
