using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu;
using Emgu.CV;
using Emgu.Util;
using System.Threading;
using System.Runtime.InteropServices;
using Emgu.CV.Structure;

namespace TFM
{
    public partial class DoubleImageControl : UserControl
    {
        public DoubleImageControl()
        {
            InitializeComponent();

            rdBtnByN.Checked = true;
        }

        #region Parámetros

        public int posicionX;
        public int posicionY;
        int cntScale = 0;
        bool firstScale = true;
        SizeF scale = new SizeF();
        Point _puntoInicial;
        Point _puntoFinal;
        int[] histograma = new int[256];
        //MIplImage _imagenMostrada;
        Image<Gray, Byte> _imagenMostrada;
        Image<Rgb, Byte> _imagenColorMostrada;
        IntPtr _imagenIntPtr;
        ImageType imgType;

        #endregion

        #region Propiedades Get-Set

        public Point PuntoInicial
        {
            get
            {
                return _puntoInicial;
            }
            set
            {
                _puntoInicial = value;
            }
        }
        public Point PuntoFinal
        {
            get
            {
                return _puntoFinal;
            }
            set
            {
                _puntoFinal = value;
            }
        }
        public Image<Gray, Byte> ImagenMostrada
        {
            get
            {
                return _imagenMostrada;
            }
            set
            {
                _imagenMostrada = value;
            }
        }
        public Image<Rgb, Byte> ImagenColorMostrada
        {
            get
            {
                return _imagenColorMostrada;
            }
            set
            {
                _imagenColorMostrada = value;
            }
        }
        public Image ptbImagenGS
        {
            get
            {
                return ptbImagen.Image;
            }
            set
            {
                ptbImagen.Image = value;
            }
        }
        public IntPtr ImagenIntPtr
        {
            get
            {
                return _imagenIntPtr;
            }
            set
            {
                _imagenIntPtr = value;
            }
        }
        public int[] Histograma
        {
            get
            {
                return histograma;
            }
            set
            {
                histograma = value;
            }
        }
        public ImageType imageType
        {
            get
            {
                return imgType;
            }
            set
            {
                imgType = value;
            }
        }
        public bool RadBtnChecked
        {
            get { return radBtnColor.Checked; }
            set { radBtnColor.Checked = value; }
        }

        #endregion

        // MOSTRAR COORDENADAS
        private void ptbImagen_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                // Se guardan las coordenadas del puntero del ratón y se muestran en los labels correspondientes
                posicionX = (ptbImagen.PointToClient(Cursor.Position).X);
                posicionY = (ptbImagen.PointToClient(Cursor.Position).Y);

                //Se transforma a coordenadas en px:
                if (_imagenMostrada != null)
                {
                    posicionX *= (_imagenMostrada.Width / ptbImagen.Width);
                    posicionY *= (_imagenMostrada.Height / ptbImagen.Height);
                }

                lblCrdX.Text = posicionX.ToString();
                lblCrdY.Text = posicionY.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Error", ex);
            }
        }

        // CLICK EN BOTÓN DE AMPLIAR
        private void btnAmpliar_Click(object sender, EventArgs e)
        {
            try
            {
                // Se modifica el factor de escala para aplicarlo posteriormente al picturebox
                if (cntScale < 4)
                {
                    scale.Height = (float)1.25;
                    scale.Width = (float)1.25;
                    //splitContainer1.Panel2.Scale(scale);
                    ptbImagen.Scale(scale);
                    cntScale++;
                    //checkedDibujar = false;

                    this.Refresh();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error", ex);
            }
        }

        // CLICK EN BOTÓN DE REDUCIR
        private void btnReducir_Click(object sender, EventArgs e)
        {
            try
            {
                // Se modifica el factor de escala para aplicarlo posteriormente al picturebox
                if (cntScale > -4)
                {

                    scale.Height = (float)0.8;
                    scale.Width = (float)0.8;
                    ptbImagen.Scale(scale);
                    cntScale--;
                    //checkedDibujar = false;

                    this.Refresh();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error", ex);
            }
        }

        // CLICK EN BOTÓN DE FIT
        private void btnFit_Click(object sender, EventArgs e)
        {
            try
            {
                // Llevando la cuenta del nivel de zoom alcanzado, al pulsar fit se recupera el tamaño inicial
                float InicScale = 1;
                if (cntScale >= 0)
                {
                    InicScale = (float)Math.Pow(0.8, (double)cntScale);
                }
                else if (cntScale < 0)
                {
                    cntScale *= (-1);
                    InicScale = (float)Math.Pow(1.25, (double)cntScale);
                }
                scale.Height = InicScale;
                scale.Width = InicScale;
                ptbImagen.Scale(scale);
                cntScale = 0;
                //checkedDibujar = false;
            }
            catch (Exception ex)
            {
                throw new Exception("Error", ex);
            }
        }

        
        private void rdBtnByN_CheckedChanged(object sender, EventArgs e)
        {
            if (rdBtnByN.Checked == true)
                imgType = ImageType.Gray;
            else
                imgType = ImageType.Color;

            FuncionCambioImagen();
        }

        //EVENTO
        public event EventHandler ImageTypeChanged;
        private void FuncionCambioImagen()
        {
            //Null check makes sure the main page is attached to the event
            if (this.ImageTypeChanged != null)
                this.ImageTypeChanged(this, new EventArgs());
        }
    }
}
