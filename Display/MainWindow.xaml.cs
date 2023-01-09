using bcnvision.Data;
using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using Cognex.VisionPro.ImageProcessing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.CvEnum;
using Cognex.VisionPro.Implementation;
using Path = System.IO.Path;
using bcnvision.Data;
using bcnvision.Tools;
using Size = System.Drawing.Size;
using Orientation = System.Windows.Controls.Orientation;

namespace Display
{
    /// <summary>
    /// ScreenHandler: Clase para controlar en que pantalla situamos la apl
    /// </summary>
    public static class ScreenHandler
    {
        public static Screen GetCurrentScreen(Window window)
        {
            var parentArea = new System.Drawing.Rectangle((int)window.Left, (int)window.Top, (int)window.Width, (int)window.Height);
            return Screen.FromRectangle(parentArea);
        }

        public static Screen GetScreen(int requestedScreen)
        {
            var screens = Screen.AllScreens;
            var mainScreen = 0;
            if (screens.Length > 1 && mainScreen < screens.Length)
            {
                return screens[requestedScreen];
            }
            return screens[0];
        }
    }

    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window//, INotifyPropertyChanged
    {
        #region Fields

        //Displays de las im´çagenes en la aplicación
        private CogRecordsDisplay recordsDisplay1 = new CogRecordsDisplay();
        private CogRecordsDisplay recordsDisplay2 = new CogRecordsDisplay();
        private CogRecordsDisplay recordsDisplay3 = new CogRecordsDisplay();
        DateTime LastDate = DateTime.Now;

        //Calibración para tab calib 2D
        private OpenCVCalibration Calibration2D;
        private Bitmap CalibrationImage2D;
        private string ImageFileName2D;

        //Calibracion para tab 3d 3planes
        private OpenCVCalibration Calibration3D2Planes;
        private Bitmap CalibrationImage3DBasePlane;
        private string ImageFileName3DBasePlane;
        private Bitmap CalibrationImage3D2Plane;
        private string ImageFileName3D2Plane;

        //Calibracoin para TEST
        private OpenCVCalibration CalibrationTest;

        #endregion

        #region Properties
        public string ImagesDirectory1 { get; set; }
        public string ImagesDirectory2 { get; set; }
        public int Monitor { get; set; } = 0;
        public DateTime LastDate1 { get; set; } = DateTime.Now;
        public DateTime LastDate2 { get; set; } = DateTime.Now;

        protected internal BcnFolderManager FolderManager { get; set; }

        #endregion

        #region Constructor
        public MainWindow()
        {

            InitializeComponent();
            
            this.DataContext = this;

            //Comprobamos que los argumentos de entrada sean los correctos
            string[] args = Environment.GetCommandLineArgs();

            //Los argumentos de entrada definen los puertos a utilizar
            if (args.Length > 3)
            {
                ImagesDirectory1 = args[1];
                ImagesDirectory1 = args[2];
                Monitor = Convert.ToInt32(args[3]);
            }
            this.WindowStyle = WindowStyle.SingleBorderWindow;

            ShowOnMonitor(Monitor, this);

            //Instancia el recordsDisplay
            wfh0.Child = recordsDisplay1;
            wfh1.Child = recordsDisplay2;
            wfh2.Child = recordsDisplay3;

            //Cambiamos los ajustes de los displays
            recordsDisplay1.Display.Fit();
            recordsDisplay1.Display.BackColor = System.Drawing.Color.Gray;
            recordsDisplay2.Display.Fit();
            recordsDisplay2.Display.BackColor = System.Drawing.Color.Gray;
            recordsDisplay3.Display.Fit();
            recordsDisplay3.Display.BackColor = System.Drawing.Color.Gray;

            //Si recibimos el cierre desde el header cerramos la app
            Header.OnClose += () => Close();
#pragma warning disable CS4014 // Ya que no se esperaba esta llamada, la ejecución del método actual continúa antes de que se complete la llamada
            RunPeriodicAsync(OnTick, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(10000), CancellationToken.None);
#pragma warning restore CS4014 // Ya que no se esperaba esta llamada, la ejecución del método actual continúa antes de que se complete la llamada
            //OnTick();


            #region  Form
            cmbOriginRotation.Items.Add("0");
            cmbOriginRotation.Items.Add("90");
            cmbOriginRotation.Items.Add("180");
            cmbOriginRotation.Items.Add("270");
            cmbOriginRotation.SelectedIndex = 0;

            cmbInvertAxis.Items.Add("Not Inverted");
            cmbInvertAxis.Items.Add("Inverted");
            cmbInvertAxis.SelectedIndex = 0;

            cmbOriginRotation1.Items.Add("0");
            cmbOriginRotation1.Items.Add("90");
            cmbOriginRotation1.Items.Add("180");
            cmbOriginRotation1.Items.Add("270");
            cmbOriginRotation1.SelectedIndex = 0;

            cmbInvertAxis1.Items.Add("Not Inverted");
            cmbInvertAxis1.Items.Add("Inverted");
            cmbInvertAxis1.SelectedIndex = 0;
            #endregion
        }

        #endregion

        #region Window Methods

        private void ShowOnMonitor(int monitor, Window window)
        {
            var screen = ScreenHandler.GetScreen(monitor);
            //var currentScreen = ScreenHandler.GetCurrentScreen(this);
            window.WindowState = WindowState.Normal;
            window.Left = screen.WorkingArea.Left;
            window.Top = screen.WorkingArea.Top;
            window.Width = screen.WorkingArea.Width;
            window.Height = screen.WorkingArea.Height;
            window.Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var senderWindow = sender as Window;
            senderWindow.WindowState = WindowState.Maximized;
        }

        // The `onTick` method will be called periodically unless cancelled.
        private static async Task RunPeriodicAsync(Action onTick, TimeSpan dueTime, TimeSpan interval, CancellationToken token)
        {
            // Initial wait time before we begin the periodic loop.
            if (dueTime > TimeSpan.Zero)
                await Task.Delay(dueTime, token);

            // Repeat this loop until cancelled.
            while (!token.IsCancellationRequested)
            {
                // Call our onTick function.
                onTick?.Invoke();

                // Wait to repeat again.
                if (interval > TimeSpan.Zero)
                    await Task.Delay(interval, token);
            }
        }

        private void OnTick()
        {
            try
            {
            }
            catch { }
        }

        #endregion

        #region 2D Calibration METHODS

        /// <summary>
        /// Botón CARGAR IMAGEN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLoadImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Lanzamos el diálogo de selección de imagen
                using (OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Title = "Open Image";

                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        //Cargamos la imagen seleccionada
                        ImageFileName2D = dlg.FileName;

                        CalibrationImage2D = new Bitmap(ImageFileName2D);

                        CogRecord record = new CogRecord("Image", typeof(ICogImage), CogRecordUsageConstants.Input, false, new CogImage8Grey(CalibrationImage2D), "");

                        //Lo pasamos al display
                        recordsDisplay1.Subject = record;
                        recordsDisplay1.Display.InteractiveGraphicTipsEnabled = true;
                        recordsDisplay1.Display.CreateGraphics();

                        string msg = "Loaded image: " + ImageFileName2D + "\r\n\r\n"; 
                                                
                        UpdateTextBlock(msg, true, 0);

                    }
                }
                
            }
            catch (Exception ex)
            {
                               
            }
        
        }
        
        /// <summary>
        /// Botón CALIBRAR
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCalibrate_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if(CalibrationImage2D != null)
                {
                    //Cargamos la imagen
                    Image<Gray, byte> calibImg = new Image<Gray, byte>(ImageFileName2D);

                    //Creamos el objeto de la calibracion
                    Calibration2D = new OpenCVCalibration(calibImg);

                    //Recogemos los valores de los settings
                    int patternSizeW = Convert.ToInt32(txtPAtternSizeW.Text);
                    int patternSizeH = Convert.ToInt32(txtPAtternSizeH.Text);
                    Size patSize = new Size(patternSizeW, patternSizeH);
                    double squareSize = Convert.ToDouble(txtSquareSize.Text);
                    double originX = Convert.ToDouble(txtOriginX.Text);
                    double originY = Convert.ToDouble(txtOriginY.Text);
                    
                    //Calibramos y obtenemos el error
                    double error; DateTime antes = DateTime.Now;

                    bool res = Calibration2D.Calibrate(patSize, (float)squareSize, (float)originX, (float)originY, out error);

                    DateTime despues = DateTime.Now; double elapsedTime = 0;

                    elapsedTime = (1000.0 * ((despues.Second - antes.Second) == 0 ? 0 : (despues.Second - antes.Second) - 1)) + ((despues.Millisecond > antes.Millisecond) ? despues.Millisecond - antes.Millisecond : 1000 - (antes.Millisecond - despues.Millisecond));

                    //Image points in picturebox
                    DrawCorners(ImageFileName2D, Calibration2D.CornersPointsList, 0);

                    //Image points in picturebox
                    DrawCornersAndOrigin(ImageFileName2D, Calibration2D.CornersPointsList, new PointF((float)(originX/squareSize), (float)(originY / squareSize)), patSize, 0);

                    //Actualiza el textblock
                    if (res)
                    {
                        string msg = "---------- CALIBRATION SUCCESS! ----------\r\n";
                        msg += "\r\nCalibration error = " + error.ToString("0.0000");
                        msg += "\r\nElapsed time = " + elapsedTime.ToString("00") + "ms";
                        msg += "\r\n" + "\r\n";
                        msg += "Intrinsic Params.:\r\n";
                        msg += "Camera Matrix = [" + Calibration2D.CameraMatrixD[0, 0].ToString("0.000") + "], [" + Calibration2D.CameraMatrixD[0, 1].ToString("0.000") + "], [" + Calibration2D.CameraMatrixD[0, 2].ToString("0.000") + "]" + "\r\n";
                        msg += "                [" + Calibration2D.CameraMatrixD[1, 0].ToString("0.000") + "], [" + Calibration2D.CameraMatrixD[1, 1].ToString("0.000") + "], [" + Calibration2D.CameraMatrixD[1, 2].ToString("0.000") + "]" + "\r\n";
                        msg += "                [" + Calibration2D.CameraMatrixD[2, 0].ToString("0.000") + "], [" + Calibration2D.CameraMatrixD[2, 1].ToString("0.000") + "], [" + Calibration2D.CameraMatrixD[2, 2].ToString("0.000") + "]" + "\r\n";
                        msg += "Distortion Coeffs. = [" + Calibration2D.DistCoeffsD[0].ToString("0.000") + "]\r\n";
                        msg += "                     [" + Calibration2D.DistCoeffsD[1].ToString("0.000") + "]\r\n";
                        msg += "                     [" + Calibration2D.DistCoeffsD[2].ToString("0.000") + "]\r\n";
                        msg += "                     [" + Calibration2D.DistCoeffsD[3].ToString("0.000") + "]\r\n";
                        msg += "                     [" + Calibration2D.DistCoeffsD[4].ToString("0.000") + "]\r\n";
                        msg += "                     [" + Calibration2D.DistCoeffsD[5].ToString("0.000") + "]\r\n";
                        msg += "                     [" + Calibration2D.DistCoeffsD[6].ToString("0.000") + "]\r\n\r\n";
                        msg += "Extrinsic Params.:\r\n";
                        msg += "Traslation Vector = [" + Calibration2D.TranslationVector[0].ToString("0.000") + "], [" + Calibration2D.TranslationVector[1].ToString("0.000") + "], [" + Calibration2D.TranslationVector[2].ToString("0.000") + "]" + "\r\n";
                        msg += "Rotation Matrix = [" + Calibration2D.RotationMatrix[0,0].ToString("0.000") + "], [" + Calibration2D.RotationMatrix[0,1].ToString("0.000") + "], [" + Calibration2D.RotationMatrix[0,2].ToString("0.000") + "]" + "\r\n";
                        msg += "                  [" + Calibration2D.RotationMatrix[1,0].ToString("0.000") + "], [" + Calibration2D.RotationMatrix[1,1].ToString("0.000") + "], [" + Calibration2D.RotationMatrix[1,2].ToString("0.000") + "]" + "\r\n";
                        msg += "                  [" + Calibration2D.RotationMatrix[2, 0].ToString("0.000") + "], [" + Calibration2D.RotationMatrix[2, 1].ToString("0.000") + "], [" + Calibration2D.RotationMatrix[2, 2].ToString("0.000") + "]" + "\r\n\r\n";

                        UpdateTextBlock(msg, false, 0);

                    }
                    else
                        UpdateTextBlock("Calibration Failed!\r\n\r\n", false, 0);
                    
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Botón EXPORTAR
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Lanzamos el diálogo de selección de imagen
                using (SaveFileDialog dlg = new SaveFileDialog())
                {
                    dlg.Title = "Save Calibration";

                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        //Exportamos calibracion
                        Calibration2D.ExportCalibration(dlg.FileName + ".xml");

                        UpdateTextBlock("Calibration Data Saved!\r\n\r\n", false, 0);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region 2-plane 3D Calibration METHODS

        /// <summary>
        /// Botón CARGAR IMAGEN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLoadImage1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Lanzamos el diálogo de selección de imagen
                using (OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Title = "Open Image";

                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        //Cargamos la imagen seleccionada
                        ImageFileName3DBasePlane = dlg.FileName;

                        CalibrationImage3DBasePlane = new Bitmap(ImageFileName3DBasePlane);

                        CogRecord record = new CogRecord("Image", typeof(ICogImage), CogRecordUsageConstants.Input, false, new CogImage8Grey(CalibrationImage3DBasePlane), "");

                        //Lo pasamos al display
                        recordsDisplay2.Subject = record;
                        recordsDisplay2.Display.InteractiveGraphicTipsEnabled = true;
                        recordsDisplay2.Display.CreateGraphics();

                        string msg = "Loaded image: " + ImageFileName3DBasePlane + "\r\n\r\n";

                        UpdateTextBlock(msg, false, 1);

                    }
                }

            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Botón CARGAR IMAGEN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLoadImage2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Lanzamos el diálogo de selección de imagen
                using (OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Title = "Open Image";

                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        //Cargamos la imagen seleccionada
                        ImageFileName3D2Plane = dlg.FileName;

                        CalibrationImage3D2Plane = new Bitmap(ImageFileName3D2Plane);

                        CogRecord record = new CogRecord("Image", typeof(ICogImage), CogRecordUsageConstants.Input, false, new CogImage8Grey(CalibrationImage3D2Plane), "");

                        //Lo pasamos al display
                        recordsDisplay3.Subject = record;
                        recordsDisplay3.Display.InteractiveGraphicTipsEnabled = true;
                        recordsDisplay3.Display.CreateGraphics();

                        //mostramos res en el cuadro de texto
                        string msg = "Loaded image: " + ImageFileName3D2Plane + "\r\n\r\n";

                        UpdateTextBlock(msg, false, 1);

                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Botón CALIBRAR
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCalibrate2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Calibramos la primera imagen
                if (CalibrationImage3DBasePlane != null)
                {
                    //Cargamos la imagen
                    Image<Gray, byte> calibImg1 = new Image<Gray, byte>(ImageFileName3DBasePlane);

                    //Creamos el objeto de la calibracion
                    Calibration3D2Planes = new OpenCVCalibration(calibImg1);

                    //Recogemos los valores de los settings
                    int patternSizeW1 = Convert.ToInt32(txtPAtternSizeW1.Text);
                    int patternSizeH1 = Convert.ToInt32(txtPAtternSizeH1.Text);
                    Size patSize1 = new Size(patternSizeW1, patternSizeH1);
                    double squareSize1 = Convert.ToDouble(txtSquareSize1.Text);
                    double originX1 = Convert.ToDouble(txtOriginX1.Text);
                    double originY1 = Convert.ToDouble(txtOriginY1.Text);

                    //Calibramos y obtenemos el error
                    double error1; DateTime antes = DateTime.Now;

                    bool res = Calibration3D2Planes.Calibrate(patSize1, (float)squareSize1, (float)originX1, (float)originY1, out error1);

                    DateTime despues = DateTime.Now; double elapsedTime = 0;

                    elapsedTime = (1000.0 * ((despues.Second - antes.Second) == 0 ? 0 : (despues.Second - antes.Second) - 1)) + ((despues.Millisecond > antes.Millisecond) ? despues.Millisecond - antes.Millisecond : 1000 - (antes.Millisecond - despues.Millisecond));

                    //Image points in picturebox
                    DrawCorners(ImageFileName3DBasePlane, Calibration3D2Planes.CornersPointsList, 1);

                    //Image points in picturebox
                    DrawCornersAndOrigin(ImageFileName3DBasePlane, Calibration3D2Planes.CornersPointsList, new PointF((float)(originX1 / squareSize1), (float)(originY1 / squareSize1)), patSize1, 1);

                    //Actualiza el textblock
                    if (res)
                    {
                        string msg = "---------- BASE PLANE CALIBRATION SUCCESS! ----------\r\n";
                        msg += "\r\nCalibration error = " + error1.ToString("0.0000");
                        msg += "\r\nElapsed time = " + elapsedTime.ToString("00") + "ms";
                        msg += "\r\n" + "\r\n";
                        msg += "Intrinsic Params.:\r\n";
                        msg += "Camera Matrix = [" + Calibration3D2Planes.CameraMatrixD[0, 0].ToString("0.000") + "], [" + Calibration3D2Planes.CameraMatrixD[0, 1].ToString("0.000") + "], [" + Calibration3D2Planes.CameraMatrixD[0, 2].ToString("0.000") + "]" + "\r\n";
                        msg += "                [" + Calibration3D2Planes.CameraMatrixD[1, 0].ToString("0.000") + "], [" + Calibration3D2Planes.CameraMatrixD[1, 1].ToString("0.000") + "], [" + Calibration3D2Planes.CameraMatrixD[1, 2].ToString("0.000") + "]" + "\r\n";
                        msg += "                [" + Calibration3D2Planes.CameraMatrixD[2, 0].ToString("0.000") + "], [" + Calibration3D2Planes.CameraMatrixD[2, 1].ToString("0.000") + "], [" + Calibration3D2Planes.CameraMatrixD[2, 2].ToString("0.000") + "]" + "\r\n";
                        msg += "Distortion Coeffs. = [" + Calibration3D2Planes.DistCoeffsD[0].ToString("0.000") + "]\r\n";
                        msg += "                     [" + Calibration3D2Planes.DistCoeffsD[1].ToString("0.000") + "]\r\n";
                        msg += "                     [" + Calibration3D2Planes.DistCoeffsD[2].ToString("0.000") + "]\r\n";
                        msg += "                     [" + Calibration3D2Planes.DistCoeffsD[3].ToString("0.000") + "]\r\n";
                        msg += "                     [" + Calibration3D2Planes.DistCoeffsD[4].ToString("0.000") + "]\r\n";
                        msg += "                     [" + Calibration3D2Planes.DistCoeffsD[5].ToString("0.000") + "]\r\n";
                        msg += "                     [" + Calibration3D2Planes.DistCoeffsD[6].ToString("0.000") + "]\r\n\r\n";
                        msg += "Extrinsic Params.:\r\n";
                        msg += "Traslation Vector = [" + Calibration3D2Planes.TranslationVector[0].ToString("0.000") + "], [" + Calibration3D2Planes.TranslationVector[1].ToString("0.000") + "], [" + Calibration3D2Planes.TranslationVector[2].ToString("0.000") + "]" + "\r\n";
                        msg += "Rotation Matrix = [" + Calibration3D2Planes.RotationMatrix[0, 0].ToString("0.000") + "], [" + Calibration3D2Planes.RotationMatrix[0, 1].ToString("0.000") + "], [" + Calibration3D2Planes.RotationMatrix[0, 2].ToString("0.000") + "]" + "\r\n";
                        msg += "                  [" + Calibration3D2Planes.RotationMatrix[1, 0].ToString("0.000") + "], [" + Calibration3D2Planes.RotationMatrix[1, 1].ToString("0.000") + "], [" + Calibration3D2Planes.RotationMatrix[1, 2].ToString("0.000") + "]" + "\r\n";
                        msg += "                  [" + Calibration3D2Planes.RotationMatrix[2, 0].ToString("0.000") + "], [" + Calibration3D2Planes.RotationMatrix[2, 1].ToString("0.000") + "], [" + Calibration3D2Planes.RotationMatrix[2, 2].ToString("0.000") + "]" + "\r\n\r\n";

                        UpdateTextBlock(msg, false, 1);

                    }
                    else
                        UpdateTextBlock("Calibration Failed!\r\n\r\n", false, 1);

                }

                //Calibramos la segunda imagen
                if (CalibrationImage3DBasePlane != null)
                {
                    //Cargamos la imagen
                    Image<Gray, byte> calibImg2 = new Image<Gray, byte>(ImageFileName3D2Plane);

                    //Creamos el objeto de la calibracion
                    if(Calibration3D2Planes != null)
                    {
                        Calibration3D2Planes.Plane2CalibrationImage = calibImg2;
                    }

                    //Recogemos los valores de los settings
                    int patternSizeW2 = Convert.ToInt32(txtPatternSizeW2.Text);
                    int patternSizeH2 = Convert.ToInt32(txtPatternSizeH2.Text);
                    Size patSize2 = new Size(patternSizeW2, patternSizeH2);
                    double squareSize2 = Convert.ToDouble(txtSquareSize2.Text);
                    double originX2 = Convert.ToDouble(txtOriginX2.Text);
                    double originY2 = Convert.ToDouble(txtOriginY2.Text);

                    //Calibramos y obtenemos el error
                    double error2; DateTime antes = DateTime.Now;

                    bool res = Calibration3D2Planes.Calibrate2PlaneImage(patSize2, (float)squareSize2, (float)originX2, (float)originY2, out error2);

                    DateTime despues = DateTime.Now; double elapsedTime = 0;

                    elapsedTime = (1000.0 * ((despues.Second - antes.Second) == 0 ? 0 : (despues.Second - antes.Second) - 1)) + ((despues.Millisecond > antes.Millisecond) ? despues.Millisecond - antes.Millisecond : 1000 - (antes.Millisecond - despues.Millisecond));

                    //Image points in picturebox
                    DrawCorners(ImageFileName3D2Plane, Calibration3D2Planes.CornersPointsList2Plane, 2);
                    
                    //Image points in picturebox
                    DrawCornersAndOrigin(ImageFileName3D2Plane, Calibration3D2Planes.CornersPointsList2Plane, new PointF((float)(originX2 / squareSize2), (float)(originY2 / squareSize2)), patSize2, 2);

                    //Actualiza el textblock
                    if (res)
                    {
                        string msg = "----------2nd PLANE CALIBRATION SUCCESS! ----------\r\n";
                        msg += "\r\nCalibration error = " + error2.ToString("0.0000");
                        msg += "\r\nElapsed time = " + elapsedTime.ToString("00") + "ms";
                        msg += "\r\n" + "\r\n";
                        msg += "Intrinsic Params.:\r\n";
                        msg += "Camera Matrix = [" + Calibration3D2Planes.CameraMatrix2PlaneD[0, 0].ToString("0.000") + "], [" + Calibration3D2Planes.CameraMatrix2PlaneD[0, 1].ToString("0.000") + "], [" + Calibration3D2Planes.CameraMatrix2PlaneD[0, 2].ToString("0.000") + "]" + "\r\n";
                        msg += "                [" + Calibration3D2Planes.CameraMatrix2PlaneD[1, 0].ToString("0.000") + "], [" + Calibration3D2Planes.CameraMatrix2PlaneD[1, 1].ToString("0.000") + "], [" + Calibration3D2Planes.CameraMatrix2PlaneD[1, 2].ToString("0.000") + "]" + "\r\n";
                        msg += "                [" + Calibration3D2Planes.CameraMatrix2PlaneD[2, 0].ToString("0.000") + "], [" + Calibration3D2Planes.CameraMatrix2PlaneD[2, 1].ToString("0.000") + "], [" + Calibration3D2Planes.CameraMatrix2PlaneD[2, 2].ToString("0.000") + "]" + "\r\n";
                        msg += "Distortion Coeffs. = [" + Calibration3D2Planes.DistCoeffs2PlaneD[0].ToString("0.000") + "]\r\n";
                        msg += "                     [" + Calibration3D2Planes.DistCoeffs2PlaneD[1].ToString("0.000") + "]\r\n";
                        msg += "                     [" + Calibration3D2Planes.DistCoeffs2PlaneD[2].ToString("0.000") + "]\r\n";
                        msg += "                     [" + Calibration3D2Planes.DistCoeffs2PlaneD[3].ToString("0.000") + "]\r\n";
                        msg += "                     [" + Calibration3D2Planes.DistCoeffs2PlaneD[4].ToString("0.000") + "]\r\n";
                        msg += "                     [" + Calibration3D2Planes.DistCoeffs2PlaneD[5].ToString("0.000") + "]\r\n";
                        msg += "                     [" + Calibration3D2Planes.DistCoeffs2PlaneD[6].ToString("0.000") + "]\r\n\r\n";
                        msg += "Extrinsic Params.:\r\n";
                        msg += "Traslation Vector = [" + Calibration3D2Planes.TranslationVector2Plane[0].ToString("0.000") + "], [" + Calibration3D2Planes.TranslationVector2Plane[1].ToString("0.000") + "], [" + Calibration3D2Planes.TranslationVector2Plane[2].ToString("0.000") + "]" + "\r\n";
                        msg += "Rotation Matrix = [" + Calibration3D2Planes.RotationMatrix2Plane[0, 0].ToString("0.000") + "], [" + Calibration3D2Planes.RotationMatrix2Plane[0, 1].ToString("0.000") + "], [" + Calibration3D2Planes.RotationMatrix2Plane[0, 2].ToString("0.000") + "]" + "\r\n";
                        msg += "                  [" + Calibration3D2Planes.RotationMatrix2Plane[1, 0].ToString("0.000") + "], [" + Calibration3D2Planes.RotationMatrix2Plane[1, 1].ToString("0.000") + "], [" + Calibration3D2Planes.RotationMatrix2Plane[1, 2].ToString("0.000") + "]" + "\r\n";
                        msg += "                  [" + Calibration3D2Planes.RotationMatrix2Plane[2, 0].ToString("0.000") + "], [" + Calibration3D2Planes.RotationMatrix2Plane[2, 1].ToString("0.000") + "], [" + Calibration3D2Planes.RotationMatrix2Plane[2, 2].ToString("0.000") + "]" + "\r\n\r\n";

                        UpdateTextBlock(msg, false, 1);

                    }
                    else
                        UpdateTextBlock("Calibration Failed!\r\n\r\n", false, 1);

                }




            }
            catch(Exception ex)
            {

            }
        }

        /// <summary>
        /// Botón EXPORTAR
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExport2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Lanzamos el diálogo de selección de ruta
                using (SaveFileDialog dlg = new SaveFileDialog())
                {
                    dlg.Title = "Save Calibration";

                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        //Exportamos calibracion
                        Calibration3D2Planes.ExportCalibration(dlg.FileName + ".xml");

                        UpdateTextBlock("Calibration Data Saved!\r\n\r\n", false, 1);
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }

        #endregion
        
        #region Test METHODS
        
        private void BtnLoadCalibTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadTestCalibration();
            }
            catch (Exception ex)
            {

            }
        }

        public void LoadTestCalibration()
        {
            try
            {
                //Lanzamos el diálogo de selección de imagen
                using (OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Title = "Open Calibration XML";

                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        //Cargamos la imagen seleccionada
                        string xml = dlg.FileName;

                        CalibrationTest = new OpenCVCalibration();
                        
                        CalibrationTest.ImportCalibration(xml);
                        
                        string msg = "Loaded calibration: " + ImageFileName2D + "\r\n\r\n";

                        UpdateTextBlock(msg, true, 0);

                    }
                }

            }
            catch (Exception ex)
            {

            }
        }


        private void BtnMapPoint_Click(object sender, RoutedEventArgs e)
        {

            //Map point 3D
            if (Calibration3D2Planes != null)
            {
                PointF pt3D = Calibration3D2Planes.GetPoint3D(new PointF((float)Convert.ToDouble(txtImagePointX3D.Text), (float)Convert.ToDouble(txtImagePointY3D.Text)), Convert.ToDouble(txtWorldPointZ3D.Text), Convert.ToDouble(txtWorld2PlaneDist.Text));

                txtWorldPointX3D.Text = pt3D.X.ToString("0.000");
                txtWorldPointY3D.Text = pt3D.Y.ToString("0.000");
            }

            //Map point 2D
            if (Calibration2D != null)
            {
                //PointF pt2D = Calibration2D.MapPoint(new PointF((float)Convert.ToDouble(txtImagePointX2D.Text), (float)Convert.ToDouble(txtImagePointY2D.Text)));
                PointF pt2D = Calibration2D.MapPointProyected(new PointF((float)Convert.ToDouble(txtImagePointX2D.Text), (float)Convert.ToDouble(txtImagePointY2D.Text)), Convert.ToDouble(txtTest.Text != "" ? txtTest.Text : "0"));

                txtWorldPointX2D.Text = pt2D.X.ToString("0.000");
                txtWorldPointY2D.Text = pt2D.Y.ToString("0.000");
            }


        }

        #endregion

        #region Methods

        /// <summary>
        /// Función para dibujar los puntos del checkerboard en la imagen
        /// </summary>
        /// <param name="image"></param>
        /// <param name="imageCorners"></param>
        private void DrawCorners(string ImageFileName, PointF[][] imageCorners, int recordDisplayIndex)
        {
            try
            {
                //Cogemos la imagen en color
                Bitmap img = new Bitmap(ImageFileName);
                Image<Rgb, byte> imageCV = new Image<Rgb, byte>(ImageFileName);

                //Creamos los puntos y los printamos en la imagen
                for (int i = 0; i < imageCorners[0].Length; i++)
                {
                    Cross2DF pt = new Cross2DF(new PointF(imageCorners[0][i].X, imageCorners[0][i].Y), 8, 8);

                    imageCV.Draw(pt, new Rgb(System.Drawing.Color.Red), 2);
                }

                //Creamos un record
                CogRecord record = new CogRecord("CalibrationImage", typeof(ICogImage), CogRecordUsageConstants.Input, false, new CogImage24PlanarColor(imageCV.ToBitmap()), "");

                //Lo pasamos al display que toque
                switch (recordDisplayIndex)
                {
                    case 0:
                        recordsDisplay1.Subject = record;
                        recordsDisplay1.Display.InteractiveGraphicTipsEnabled = true;
                        recordsDisplay1.Display.CreateGraphics();
                        recordsDisplay1.Refresh();
                        break;

                    case 1:
                        recordsDisplay2.Subject = record;
                        recordsDisplay2.Display.InteractiveGraphicTipsEnabled = true;
                        recordsDisplay2.Display.CreateGraphics();
                        recordsDisplay2.Refresh();
                        break;

                    case 2:
                        recordsDisplay3.Subject = record;
                        recordsDisplay3.Display.InteractiveGraphicTipsEnabled = true;
                        recordsDisplay3.Display.CreateGraphics();
                        recordsDisplay3.Refresh();
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Función para dibujar los puntos del checkerboard en la imagen
        /// </summary>
        /// <param name="image"></param>
        /// <param name="imageCorners"></param>
        private void DrawCornersAndOrigin(string ImageFileName, PointF[][] imageCorners, PointF origenWorld, Size boardSize, int recordDisplayIndex)
        {
            try
            {
                //Cogemos la imagen en color
                Bitmap img = new Bitmap(ImageFileName);
                Image<Rgb, byte> imageCV = new Image<Rgb, byte>(ImageFileName);

                //Creamos los puntos y los printamos en la imagen
                for (int i = 0; i < imageCorners[0].Length; i++)
                {
                    Cross2DF pt = new Cross2DF(new PointF(imageCorners[0][i].X, imageCorners[0][i].Y), 8, 8);

                    imageCV.Draw(pt, new Rgb(System.Drawing.Color.Red), 2);
                }

                //Eje X
                //Get origin and head
                float rowPerc = (float)(1/((boardSize.Height - 1) / (float)(-origenWorld.Y)));
                float columnPerc = (float)(1 / ((boardSize.Width - 1)/ (float)(-origenWorld.X)));
                int row = (int)Math.Floor((float)(-origenWorld.Y + 1));
                int column = (int)Math.Floor((float)(-origenWorld.X + 1));

                Emgu.CV.Structure.LineSegment2DF ejeX = new LineSegment2DF(new PointF(imageCorners[0][(boardSize.Width - 1) * (row) + column].X, imageCorners[0][(boardSize.Height - 1) * (row) + column].Y),
                                                                            new PointF(imageCorners[0][(boardSize.Width - 1) * (row + 4) + column].X, imageCorners[0][(boardSize.Height - 1) * (row + 4) + column].Y));

                imageCV.Draw(ejeX, new Rgb(System.Drawing.Color.Blue), 5);

                //Eje Y
                Emgu.CV.Structure.LineSegment2DF ejeY = new LineSegment2DF(new PointF(imageCorners[0][(boardSize.Width - 1) * (row) + column].X, imageCorners[0][(boardSize.Height - 1) * (row) + column].Y),
                                                                            new PointF(imageCorners[0][(boardSize.Width - 1) * (row) + column + 6].X, imageCorners[0][(boardSize.Height - 1) * (row) + column + 6].Y));

                imageCV.Draw(ejeY, new Rgb(System.Drawing.Color.Blue), 5);

                //Creamos un record
                CogRecord record = new CogRecord("CalibrationImage", typeof(ICogImage), CogRecordUsageConstants.Input, false, new CogImage24PlanarColor(imageCV.ToBitmap()), "");

                //Lo pasamos al display que toque
                switch (recordDisplayIndex)
                {
                    case 0:
                        recordsDisplay1.Subject = record;
                        recordsDisplay1.Display.InteractiveGraphicTipsEnabled = true;
                        recordsDisplay1.Display.CreateGraphics();
                        recordsDisplay1.Refresh();
                        break;

                    case 1:
                        recordsDisplay2.Subject = record;
                        recordsDisplay2.Display.InteractiveGraphicTipsEnabled = true;
                        recordsDisplay2.Display.CreateGraphics();
                        recordsDisplay2.Refresh();
                        break;

                    case 2:
                        recordsDisplay3.Subject = record;
                        recordsDisplay3.Display.InteractiveGraphicTipsEnabled = true;
                        recordsDisplay3.Display.CreateGraphics();
                        recordsDisplay3.Refresh();
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Función para añadir texto al bloque de texto
        /// </summary>
        /// <param name="newText"></param>
        /// <param name="cleanPrev"></param>
        private void UpdateTextBlock(string newText, bool cleanPrev, int textBlockIndex)
        {
            //Actualizamos el textblock que toque haciendo una limpieza de lo previo o no
            if (!cleanPrev)
                switch (textBlockIndex)
                {
                    case 0:
                        txtBlock.Text += newText;
                        break;
                    case 1:
                        txtBlock2.Text += newText;
                        break;
                }
            else
                switch (textBlockIndex)
                {
                    case 0:
                        txtBlock.Text = newText;
                        break;
                    case 1:
                        txtBlock2.Text = newText;
                        break;
                }
            
            return;
        }
               

        #endregion

    }

}

