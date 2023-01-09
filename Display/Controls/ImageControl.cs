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
    public partial class ImageControl : UserControl
    {
        public ImageControl()
        {
            InitializeComponent();
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
        public bool checkedDibujar
        {
            get
            {
                return chkbDibujar.Checked;
            }
            set
            {
                chkbDibujar.Checked = value;
            }
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
                    checkedDibujar = false;

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
                    checkedDibujar = false;

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
                checkedDibujar = false;
            }
            catch (Exception ex)
            {
                throw new Exception("Error", ex);
            }
        }
        
        private void ptbImagen_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                // Al pulsar con el ratón se selecciona el punto de inicio de la línea
                if (chkbDibujar.Checked == true)
                {
                    _puntoInicial.X = e.X; //(Cursor.Position).X;
                    _puntoInicial.Y = e.Y; //(Cursor.Position).Y;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error", ex);
            }
        }

        private void ptbImagen_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                // Al levantar el click se selecciona el punto final de la línea y se dibuja
                if (chkbDibujar.Checked == true && _puntoInicial != _puntoFinal)
                {
                    _puntoFinal.X = e.X; //(Cursor.Position).X;
                    _puntoFinal.Y = e.Y; //(Cursor.Position).Y;

                    this.Refresh();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error", ex);
            }
        }

        // BORRAR LÍNEA
        private void chkbDibujar_CheckedChanged(object sender, EventArgs e)
        {
            if (chkbDibujar.Checked == false)
            {
                this.Refresh();
                FuncionDibujarDesactivado();
            }
            else if (chkbDibujar.Checked == true)
            {
                FuncionDibujarActivado();
            }

        }

        // DIBUJAR LÍNEA
        private void ptbImagen_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen lapiz = new Pen(Color.Red, 1);

            if (chkbDibujar.Checked == true)
            {
                if (_puntoInicial != null && _puntoFinal != null && _puntoFinal != _puntoInicial && 
                    _puntoFinal.X <= ptbImagen.Width && _puntoFinal.Y <= ptbImagen.Height &&
                    _puntoFinal.X >= 0 && _puntoFinal.Y >= 0)
                {
                    g.DrawLine(lapiz, _puntoInicial, _puntoFinal);

                    histograma = LineaAHistograma(_puntoInicial, _puntoFinal, e, _imagenIntPtr);
                    FuncionHistogramaCalculado();
                }                    
            }

            if ((_imagenMostrada != null || _imagenColorMostrada != null) && cntScale == 0 && firstScale)
            {
                firstScale = false;
                ptbImagen.SizeMode = PictureBoxSizeMode.StretchImage;
                ptbImagen.Size = new Size(splitContainer1.Panel2.Width, splitContainer1.Panel2.Width * ptbImagen.Image.Size.Height / ptbImagen.Image.Size.Width);
            }
        }

        // AÚN POR HACER - OBTENER HISTOGRAMA DE UNA LÍNEA
        public int[] LineaAHistograma(Point PuntoInicial, Point PuntoFinal, PaintEventArgs e, IntPtr ImagenIntPtr)
        {
            try
            {
                // Paso la imagen de la estructura al byteArray[]
                MIplImage Imagen = (MIplImage)Marshal.PtrToStructure(ImagenIntPtr, typeof(MIplImage));
                byte[] imagenArrayDatos = new byte[Imagen.ImageSize];
                //imagenArrayDatos = IntPtrToByteArray(ImagenIntPtr); TODO

                //Puntos que forman parte de la imagen
                PuntoInicial.X = (PuntoInicial.X * Imagen.Width / ptbImagen.Width);
                PuntoInicial.Y = (PuntoInicial.Y * Imagen.Height / ptbImagen.Height);
                PuntoFinal.X = (PuntoFinal.X * Imagen.Width / ptbImagen.Width);
                PuntoFinal.Y = (PuntoFinal.Y * Imagen.Height / ptbImagen.Height);

                int proyXLinea = Math.Abs(PuntoInicial.X - PuntoFinal.X);
                int proYLinea = Math.Abs(PuntoInicial.Y - PuntoFinal.Y);
                int cantidad = (proyXLinea > proYLinea) ? proyXLinea : proYLinea;
                Point[] puntosLinea = getPoints(cantidad, PuntoInicial, PuntoFinal);

                //Completo el array (es otro, en realidad) de dos dimensiones
                int[,] imageArray = new int[3, Imagen.ImageSize];
                for (int i = 0; i < Imagen.ImageSize; i++)
                {
                    imageArray[0, i] = imagenArrayDatos[i];
                }
                int cnt = 0;
                for (int j = 0; j < Imagen.Height; j++)
                {
                    for (int i = 0; i < Imagen.Width; i++)
                    {
                        //Coordenada X en la imagen
                        imageArray[1, cnt] = i;
                        //Coordenada Y en la imagen
                        imageArray[2, cnt] = j;
                        cnt++;
                    }
                }

                //Ya tengo la imagen en Array de 2 dimensiones, con información de las coordenadas de cada pixel y su valor
                for (int i = 0; i < 256; i++)
                    histograma[i] = 0;

                for (int i = 0; i < cantidad; i++)
                {
                    int valorPixel, indice;
                    bool isFin = false;
                    for (indice = (puntosLinea[i].Y * Imagen.Width); indice < imageArray.Length && isFin == false; indice++)
                    {
                        if (imageArray[1, indice] == puntosLinea[i].X && imageArray[2, indice] == puntosLinea[i].Y)
                        {
                            isFin = true;
                        }
                    }
                    indice--;
                    valorPixel = imageArray[0, indice];
                    histograma[valorPixel]++;
                }

                return histograma;
            }
            catch (Exception ex)
            {
                throw new Exception("Error", ex);
            }
        }

        /// <summary>
        /// Función para obtener una determinada cantidad de puntos entre dos puntos.
        /// </summary>
        /// <param name="cantidad"></param>Cantidad de Puntos
        /// <param name="p1"></param>Punto Inicial
        /// <param name="p2"></param>Punto Final
        /// <returns></returns>
        public Point[] getPoints(int cantidad, Point p1, Point p2)
        {
            var points = new Point[cantidad];
            int ydiff = p2.Y - p1.Y, xdiff = p2.X - p1.X;
            double slope = (double)(p2.Y - p1.Y) / (p2.X - p1.X);
            double x, y;

            cantidad--;

            for (double i = 0; i < cantidad; i++)
            {
                y = slope == 0 ? 0 : ydiff * (i / cantidad);
                x = slope == 0 ? xdiff * (i / cantidad) : y / slope;
                points[(int)i] = new Point((int)Math.Round(x) + p1.X, (int)Math.Round(y) + p1.Y);
            }

            points[cantidad] = p2;
            return points;
        }

        //ESTABLECER IMAGEN
        public void EstablecerImagen(IntPtr img)
        {
            if (_imagenIntPtr != IntPtr.Zero)
                CvInvoke.cvReleaseImage(ref _imagenIntPtr);
            if (img != IntPtr.Zero)
                _imagenIntPtr = img;
                    //CvInvoke.cvCopy(img, _imagenIntPtr, img);   //OJO
        }

        //UPDATE
        public void Update(IntPtr imagen)
        {
            EstablecerImagen(imagen);
            ptbImagen.Image = _imagenMostrada.Bitmap;
            //ptbImagen.Image = IntPtrToBmp(imagen); TODO
        }

        //EVENTO
        public event EventHandler HistogramaCalculado;
        private void FuncionHistogramaCalculado()
        {
            //Null check makes sure the main page is attached to the event
            if (this.HistogramaCalculado != null)
                this.HistogramaCalculado(this, new EventArgs());
        }

        public event EventHandler DibujarDesactivado;
        private void FuncionDibujarDesactivado()
        {
            //Null check makes sure the main page is attached to the event
            if (this.DibujarDesactivado != null)
                this.DibujarDesactivado(this, new EventArgs());
        }

        public event EventHandler DibujarActivado;
        private void FuncionDibujarActivado()
        {
            //Null check makes sure the main page is attached to the event
            if (this.DibujarActivado != null)
                this.DibujarActivado(this, new EventArgs());
        }
    }
}
