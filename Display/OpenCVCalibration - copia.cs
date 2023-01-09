using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using bcnvision.Data;
using bcnvision.Tools;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace Display
{
    public class OpenCVCalibration
    {
        #region Enums

        public enum Estado
        {
            NOT_CALIBRATED = 0,
            CALIBRATED = 1
        }

        #endregion

        #region Fields

        #endregion

        #region Properties

        /// <summary>
        /// Imagen d la hoja de calibracion
        /// </summary>
        public Image<Gray, byte> CalibrationImage { get; set; }

        /// <summary>
        /// Imagen extra para calibracion compleja
        /// </summary>
        public Image<Gray, byte> Plane2CalibrationImage { get; set; }

        public Estado CalibResult { get; set; }

        public Estado Plane2CalibResult { get; set; }

        /// <summary>
        /// Coordenadas puntos reales
        /// </summary>
        public MCvPoint3D32f[][] CornersObjectList { get; set; }

        /// <summary>
        /// Coordenadas puntos checkerboard
        /// </summary>
        public PointF[][] CornersPointsList { get; set; }

        /// <summary>
        /// Matriz Intrínseca de calibración de la cam
        /// </summary>
        public static Mat CameraMatrix { get; set; }

        /// <summary>
        /// Matriz Intrínseca de calibración de la cam
        /// </summary>
        public double[,] CameraMatrixD { get; set; }

        /// <summary>
        /// Coeficioentes de distorsión
        /// </summary>
        public Mat DistCoeffs { get; set; }

        /// <summary>
        /// Coeficioentes de distorsión
        /// </summary>
        public double[] DistCoeffsD { get; set; }

        /// <summary>
        /// Matriz de rotación de calibración extrínseca
        /// </summary>
        public  double[,] RotationMatrix { get; set; }

        /// <summary>
        /// Vector de traslación de calibración extrínseca
        /// </summary>
        public  double[] TranslationVector { get; set; }

        /// <summary>
        /// Altura de la base de calibracion
        /// </summary>
        public double BaseZ { get; set; }

        #region 2Plane Parameters

        /// <summary>
        /// Coordenadas puntos reales
        /// </summary>
        public MCvPoint3D32f[][] CornersObjectList2Plane { get; set; }

        /// <summary>
        /// Coordenadas puntos checkerboard
        /// </summary>
        public PointF[][] CornersPointsList2Plane { get; set; }

        /// <summary>
        /// Matriz Intrínseca de calibración de la cam
        /// </summary>
        public Mat CameraMatrix2Plane { get; set; }

        /// <summary>
        /// Matriz Intrínseca de calibración de la cam
        /// </summary>
        public double[,] CameraMatrix2PlaneD { get; set; }

        /// <summary>
        /// Coeficioentes de distorsión
        /// </summary>
        public Mat DistCoeffs2Plane { get; set; }

        /// <summary>
        /// Coeficioentes de distorsión
        /// </summary>
        public double[] DistCoeffs2PlaneD { get; set; }

        /// <summary>
        /// Matriz de rotación de calibración extrínseca
        /// </summary>
        public double[,] RotationMatrix2Plane { get; set; }

        /// <summary>
        /// Vector de traslación de calibración extrínseca
        /// </summary>
        public double[] TranslationVector2Plane { get; set; }

        /// <summary>
        /// Altura del plano 2 de calibracion
        /// </summary>
        public double Plane2Z { get; set; }

        #endregion

        #endregion

        #region Constructor

        public OpenCVCalibration()
        {
            //Calib params
            CornersObjectList = new MCvPoint3D32f[1][];
            CornersPointsList = new PointF[1][];
            CameraMatrix = new Mat(3, 3, DepthType.Cv64F, 1);
            DistCoeffs = new Mat(8, 1, DepthType.Cv64F, 1);
            CameraMatrixD = new double[3, 3];
            DistCoeffsD = new double[7];
            TranslationVector = new double[3];
            RotationMatrix = new double[3, 3];

            //Extra Image Calib params
            CornersObjectList2Plane = new MCvPoint3D32f[1][];
            CornersPointsList2Plane = new PointF[1][];
            CameraMatrix2Plane = new Mat(3, 3, DepthType.Cv64F, 1);
            DistCoeffs2Plane = new Mat(8, 1, DepthType.Cv64F, 1);
            CameraMatrix2PlaneD = new double[3, 3];
            DistCoeffs2PlaneD = new double[7];
            TranslationVector2Plane = new double[3];
            RotationMatrix2Plane = new double[3, 3];
        }

        public OpenCVCalibration(Image<Gray,byte> CalibImage)
        {
            //Imagen
            CalibrationImage = CalibImage;

            //Estado
            CalibResult = new Estado(); CalibResult = Estado.NOT_CALIBRATED;
            Plane2CalibResult = new Estado(); CalibResult = Estado.NOT_CALIBRATED;

            //Calib params
            CornersObjectList = new MCvPoint3D32f[1][];
            CornersPointsList = new PointF[1][];
            CameraMatrix = new Mat(3, 3, DepthType.Cv64F, 1);
            DistCoeffs = new Mat(8, 1, DepthType.Cv64F, 1);
            CameraMatrixD = new double[3,3];
            DistCoeffsD = new double[7];
            TranslationVector = new double[3];
            RotationMatrix = new double[3, 3];

            //Extra Image Calib params
            CornersObjectList2Plane = new MCvPoint3D32f[1][];
            CornersPointsList2Plane = new PointF[1][];
            CameraMatrix2Plane = new Mat(3, 3, DepthType.Cv64F, 1);
            DistCoeffs2Plane = new Mat(8, 1, DepthType.Cv64F, 1);
            CameraMatrix2PlaneD = new double[3, 3];
            DistCoeffs2PlaneD = new double[7];
            TranslationVector2Plane = new double[3];
            RotationMatrix2Plane = new double[3, 3];
        }

        public OpenCVCalibration(Image<Gray, byte> CalibImage, Image<Gray, byte> ExtraCalibImage)
        {
            //Images
            CalibrationImage = CalibImage;
            Plane2CalibrationImage = ExtraCalibImage;

            //Estados
            CalibResult = new Estado(); CalibResult = Estado.NOT_CALIBRATED;
            Plane2CalibResult = new Estado(); CalibResult = Estado.NOT_CALIBRATED;

            //Calib params
            CornersObjectList = new MCvPoint3D32f[1][];
            CornersPointsList = new PointF[1][];
            CameraMatrix = new Mat(3, 3, DepthType.Cv64F, 1);
            DistCoeffs = new Mat(8, 1, DepthType.Cv64F, 1);
            CameraMatrixD = new double[3, 3];
            DistCoeffsD = new double[7];
            TranslationVector = new double[3];
            RotationMatrix = new double[3, 3];

            //Extra Image Calib params
            CornersObjectList2Plane = new MCvPoint3D32f[1][];
            CornersPointsList2Plane = new PointF[1][];
            CameraMatrix2Plane = new Mat(3, 3, DepthType.Cv64F, 1);
            DistCoeffs2Plane = new Mat(8, 1, DepthType.Cv64F, 1);
            CameraMatrix2PlaneD = new double[3, 3];
            DistCoeffs2PlaneD = new double[7];
            TranslationVector2Plane = new double[3];
            RotationMatrix2Plane = new double[3, 3];

        }

        #endregion

        #region Methods

        /// <summary>
        /// función para calibrar la imagen
        /// </summary>
        /// <param name="patternSize"></param>
        /// <param name="squareSize"></param>
        /// <param name="origenX"></param>
        /// <param name="origenY"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool Calibrate(Size patternSize, float squareSize, float origenX, float origenY, out double error)
        {
            bool res = false;
            error = 0;

            try
            {
                CalibResult = Estado.NOT_CALIBRATED;

                //Creamos las imagenes
                Image<Gray, byte> InputImage = CalibrationImage;
                Image<Gray, byte> OutputImage = new Image<Gray, byte>(InputImage.Size);

                //Modificamos el pattern size
                patternSize.Width--;
                patternSize.Height--;

                //Creamos el array de los corners
                var cornerPoints = new VectorOfPointF();

                //Obtenemos los puntos de la hoja de calibracion
                res = Emgu.CV.CvInvoke.FindChessboardCorners(InputImage, patternSize, cornerPoints, CalibCbType.AdaptiveThresh);

                if (res) CalibResult = Estado.CALIBRATED;

                //Resultados más exactos
                CvInvoke.CornerSubPix(InputImage, cornerPoints, new Size(11, 11), new Size(-1, -1), new MCvTermCriteria(30, 0.1));

                //Emgu.CV.CvInvoke.DrawChessboardCorners(OutputImage, patternSize, cornerPoints, res);

                //Puntos reales
                var objectList = new List<MCvPoint3D32f>();

                ////Populating real world coordinates of the chess board corners            
                for (int j = 0; j < patternSize.Height; j++)
                    for (int i = 0; i < patternSize.Width; i++)
                        objectList.Add(new MCvPoint3D32f(j * squareSize + origenX, i * squareSize + origenY, 0.0f));

                //Calibrate camera
                CornersObjectList[0] = objectList.ToArray();
                CornersPointsList[0] = cornerPoints.ToArray();
                Mat[] _rvecs;
                Mat[] _tvecs;

                error = CvInvoke.CalibrateCamera(CornersObjectList, CornersPointsList, InputImage.Size, CameraMatrix, DistCoeffs, CalibType.RationalModel, new MCvTermCriteria(30, 0.1), out _rvecs, out _tvecs);


                #region  Matrix y distorsion data extraction
                CameraMatrixD = new double[3,3];
                CameraMatrixD[0, 0] = ((double[])CameraMatrix.GetData(false))[0];
                CameraMatrixD[0, 1] = ((double[])CameraMatrix.GetData(false))[1];
                CameraMatrixD[0, 2] = ((double[])CameraMatrix.GetData(false))[2];
                CameraMatrixD[1, 0] = ((double[])CameraMatrix.GetData(false))[3];
                CameraMatrixD[1, 1] = ((double[])CameraMatrix.GetData(false))[4];
                CameraMatrixD[1, 2] = ((double[])CameraMatrix.GetData(false))[5];
                CameraMatrixD[2, 0] = ((double[])CameraMatrix.GetData(false))[6];
                CameraMatrixD[2, 1] = ((double[])CameraMatrix.GetData(false))[7];
                CameraMatrixD[2, 2] = ((double[])CameraMatrix.GetData(false))[8];

                DistCoeffsD = new double[7];
                DistCoeffsD[0] = ((double[])DistCoeffs.GetData(false))[0];
                DistCoeffsD[1] = ((double[])DistCoeffs.GetData(false))[1];
                DistCoeffsD[2] = ((double[])DistCoeffs.GetData(false))[2];
                DistCoeffsD[3] = ((double[])DistCoeffs.GetData(false))[3];
                DistCoeffsD[4] = ((double[])DistCoeffs.GetData(false))[4];
                DistCoeffsD[5] = ((double[])DistCoeffs.GetData(false))[5];
                DistCoeffsD[6] = ((double[])DistCoeffs.GetData(false))[6];
                #endregion

                //vector de traslacion
                TranslationVector = new double[] { GetDoubleValue(_tvecs[0], 0, 0), GetDoubleValue(_tvecs[0], 1, 0), GetDoubleValue(_tvecs[0], 2, 0) };

                //matriz de rotacion
                var MR = new Matrix<double>(3, 3);
                CvInvoke.Rodrigues(_rvecs[0], MR, null);

                RotationMatrix = new double[3, 3] { { MR[0, 0], MR[0, 1], MR[0, 2] }, { MR[1, 0], MR[1, 1], MR[1, 2] }, { MR[2, 0], MR[2, 1], MR[2, 2] } };

                //Solve PnP
                IOutputArray rvecs = _rvecs[0];
                IOutputArray tvecs = _tvecs[0];
                bool r = CvInvoke.SolvePnP(CornersObjectList[0], CornersPointsList[0], CameraMatrix, DistCoeffs, rvecs, tvecs);

                IOutputArray jacobian = null;

                //Undistort Points

                double[] k = new double[] { GetDoubleValue(DistCoeffs, 0, 0), GetDoubleValue(DistCoeffs, 1, 0), GetDoubleValue(DistCoeffs, 2, 0), GetDoubleValue(DistCoeffs, 3, 0), GetDoubleValue(DistCoeffs, 4, 0), GetDoubleValue(DistCoeffs, 5, 0), GetDoubleValue(DistCoeffs, 6, 0), GetDoubleValue(DistCoeffs, 7, 0) };
                double fx = GetDoubleValue(CameraMatrix, 0, 0);
                double fy = GetDoubleValue(CameraMatrix, 1, 1);
                double ifx = 1 / fx;
                double ify = 1 / fy;
                double cx = GetDoubleValue(CameraMatrix, 0, 2);
                double cy = GetDoubleValue(CameraMatrix, 1, 2);

                #region método iterativo

                /*PointF[] distortedPoints = new PointF[] { new PointF(100, 100) };
                PointF[] undistortedPoints = new PointF[] { new PointF(0, 0) };

                for (int i = 0; i < distortedPoints.Length; i++)
                {
                    double x, y, x0, y0;
                    x = distortedPoints[i].X;
                    y = distortedPoints[i].Y;

                    x0 = x = (x - cx) * ifx;
                    y0 = y = (y - cy) * ify;

                    // compensate distortion iteratively
                    for (int j = 0; j < 5; j++)
                    {
                        double r2 = x * x + y * y;
                        double icdist = 1 / (1 + ((k[4] * r2 + k[1]) * r2 + k[0]) * r2);
                        double deltaX = 2 * k[2] * x * y + k[3] * (r2 + 2 * x * x);
                        double deltaY = k[2] * (r2 + 2 * y * y) + 2 * k[3] * x * y;
                        x = (x0 - deltaX) * icdist;
                        y = (y0 - deltaY) * icdist;
                    }
                    x = (x * fx) + cx;
                    y = (y * fy) + cy;
                    undistortedPoints[i] = new PointF((float)x, (float)y);
                }*/
                #endregion

            }
            catch (Exception ex)
            {

            }            

            return res;
        }

        /// <summary>
        /// Función para calibrar la imagen
        /// </summary>
        /// <param name="patternSize"></param>
        /// <param name="squareSize"></param>
        /// <param name="origenX"></param>
        /// <param name="origenY"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool Calibrate2PlaneImage(Size patternSize, float squareSize, float origenX, float origenY, out double error)
        {
            bool res = false;
            error = 0;

            try
            {
                Plane2CalibResult = Estado.NOT_CALIBRATED;

                //Creamos las imagenes
                Image<Gray, byte> InputImage = Plane2CalibrationImage;
                Image<Gray, byte> OutputImage = new Image<Gray, byte>(InputImage.Size);


                //Modificamos el pattern size
                patternSize.Width--;
                patternSize.Height--;

                //Creamos el array de los corners
                var cornerPoints = new VectorOfPointF();

                //Obtenemos los puntos de la hoja de calibracion
                res = Emgu.CV.CvInvoke.FindChessboardCorners(InputImage, patternSize, cornerPoints, CalibCbType.AdaptiveThresh);

                if (res) Plane2CalibResult = Estado.CALIBRATED;

                //Resultados más exactos
                CvInvoke.CornerSubPix(InputImage, cornerPoints, new Size(11, 11), new Size(-1, -1), new MCvTermCriteria(30, 0.1));

                //Emgu.CV.CvInvoke.DrawChessboardCorners(OutputImage, patternSize, cornerPoints, res);

                //Puntos reales
                var objectList = new List<MCvPoint3D32f>();

                ////Populating real world coordinates of the chess board corners            
                for (int j = 0; j < patternSize.Height; j++)
                    for (int i = 0; i < patternSize.Width; i++)
                        objectList.Add(new MCvPoint3D32f(j * squareSize + origenX, i * squareSize + origenY, 0.0f));

                //Calibrate camera
                CornersObjectList2Plane[0] = objectList.ToArray();
                CornersPointsList2Plane[0] = cornerPoints.ToArray();
                Mat[] _rvecs;
                Mat[] _tvecs;

                error = CvInvoke.CalibrateCamera(CornersObjectList2Plane, CornersPointsList2Plane, InputImage.Size, CameraMatrix2Plane, DistCoeffs2Plane, CalibType.RationalModel, new MCvTermCriteria(30, 0.1), out _rvecs, out _tvecs);


                #region  Matrix y distorsion data extraction
                CameraMatrix2PlaneD = new double[3, 3];
                CameraMatrix2PlaneD[0, 0] = ((double[])CameraMatrix2Plane.GetData(false))[0];
                CameraMatrix2PlaneD[0, 1] = ((double[])CameraMatrix2Plane.GetData(false))[1];
                CameraMatrix2PlaneD[0, 2] = ((double[])CameraMatrix2Plane.GetData(false))[2];
                CameraMatrix2PlaneD[1, 0] = ((double[])CameraMatrix2Plane.GetData(false))[3];
                CameraMatrix2PlaneD[1, 1] = ((double[])CameraMatrix2Plane.GetData(false))[4];
                CameraMatrix2PlaneD[1, 2] = ((double[])CameraMatrix2Plane.GetData(false))[5];
                CameraMatrix2PlaneD[2, 0] = ((double[])CameraMatrix2Plane.GetData(false))[6];
                CameraMatrix2PlaneD[2, 1] = ((double[])CameraMatrix2Plane.GetData(false))[7];
                CameraMatrix2PlaneD[2, 2] = ((double[])CameraMatrix2Plane.GetData(false))[8];

                DistCoeffs2PlaneD = new double[7];
                DistCoeffs2PlaneD[0] = ((double[])DistCoeffs2Plane.GetData(false))[0];
                DistCoeffs2PlaneD[1] = ((double[])DistCoeffs2Plane.GetData(false))[1];
                DistCoeffs2PlaneD[2] = ((double[])DistCoeffs2Plane.GetData(false))[2];
                DistCoeffs2PlaneD[3] = ((double[])DistCoeffs2Plane.GetData(false))[3];
                DistCoeffs2PlaneD[4] = ((double[])DistCoeffs2Plane.GetData(false))[4];
                DistCoeffs2PlaneD[5] = ((double[])DistCoeffs2Plane.GetData(false))[5];
                DistCoeffs2PlaneD[6] = ((double[])DistCoeffs2Plane.GetData(false))[6];
                #endregion

                //vector de traslacion
                TranslationVector2Plane = new double[] { GetDoubleValue(_tvecs[0], 0, 0), GetDoubleValue(_tvecs[0], 1, 0), GetDoubleValue(_tvecs[0], 2, 0) };

                //matriz de rotacion
                var MR = new Matrix<double>(3, 3);
                CvInvoke.Rodrigues(_rvecs[0], MR, null);

                RotationMatrix2Plane = new double[3, 3] { { MR[0, 0], MR[0, 1], MR[0, 2] }, { MR[1, 0], MR[1, 1], MR[1, 2] }, { MR[2, 0], MR[2, 1], MR[2, 2] } };

                //Solve PnP
                IOutputArray rvecs = _rvecs[0];
                IOutputArray tvecs = _tvecs[0];
                bool r = CvInvoke.SolvePnP(CornersObjectList2Plane[0], CornersPointsList2Plane[0], CameraMatrix2Plane, DistCoeffs2Plane, rvecs, tvecs);

                IOutputArray jacobian = null;

                //Undistort Points

                double[] k = new double[] { GetDoubleValue(DistCoeffs2Plane, 0, 0), GetDoubleValue(DistCoeffs2Plane, 1, 0), GetDoubleValue(DistCoeffs2Plane, 2, 0), GetDoubleValue(DistCoeffs2Plane, 3, 0),
                                                        GetDoubleValue(DistCoeffs2Plane, 4, 0), GetDoubleValue(DistCoeffs2Plane, 5, 0), GetDoubleValue(DistCoeffs2Plane, 6, 0), GetDoubleValue(DistCoeffs2Plane, 7, 0) };
                double fx = GetDoubleValue(CameraMatrix2Plane, 0, 0);
                double fy = GetDoubleValue(CameraMatrix2Plane, 1, 1);
                double ifx = 1 / fx;
                double ify = 1 / fy;
                double cx = GetDoubleValue(CameraMatrix2Plane, 0, 2);
                double cy = GetDoubleValue(CameraMatrix2Plane, 1, 2);

                #region método iterativo

                /*PointF[] distortedPoints = new PointF[] { new PointF(100, 100) };
                PointF[] undistortedPoints = new PointF[] { new PointF(0, 0) };

                for (int i = 0; i < distortedPoints.Length; i++)
                {
                    double x, y, x0, y0;
                    x = distortedPoints[i].X;
                    y = distortedPoints[i].Y;

                    x0 = x = (x - cx) * ifx;
                    y0 = y = (y - cy) * ify;

                    // compensate distortion iteratively
                    for (int j = 0; j < 5; j++)
                    {
                        double r2 = x * x + y * y;
                        double icdist = 1 / (1 + ((k[4] * r2 + k[1]) * r2 + k[0]) * r2);
                        double deltaX = 2 * k[2] * x * y + k[3] * (r2 + 2 * x * x);
                        double deltaY = k[2] * (r2 + 2 * y * y) + 2 * k[3] * x * y;
                        x = (x0 - deltaX) * icdist;
                        y = (y0 - deltaY) * icdist;
                    }
                    x = (x * fx) + cx;
                    y = (y * fy) + cy;
                    undistortedPoints[i] = new PointF((float)x, (float)y);
                }*/
                #endregion


            }
            catch (Exception ex)
            {

            }
            
            return res;
        }

        /// <summary>
        /// Función para obtener las coordenadas World de un punto en la imagen con Z real conocida
        /// </summary>
        /// <param name="imagePoint"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public PointF MapPointProyected(PointF imagePoint, double z)
        {
            SharpDX.Vector3[] world_points = new SharpDX.Vector3[1];

            try
            {

                //Punto 
                float X = imagePoint.X;
                float Y = imagePoint.Y;

                Mat distPts = new Mat(2, 1, DepthType.Cv64F, 1);
                SetDoubleValue(distPts, 0, 0, X);
                SetDoubleValue(distPts, 0, 1, Y);

                Mat undistPts = new Mat(2, 1, DepthType.Cv64F, 1);

                CvInvoke.UndistortPoints(distPts, undistPts, CameraMatrix, DistCoeffs, null, CameraMatrix);

                SharpDX.Vector3[] undistortedPoints = new SharpDX.Vector3[] { new SharpDX.Vector3((float)((double[])undistPts.GetData(false))[0], (float)((double[])undistPts.GetData(false))[1], 0) };

                //Convertir un punto de coordenadas en imagen a reales

                //Se invierte la matriz camera_matrix de la camara
                SharpDX.Matrix3x3 M = new SharpDX.Matrix3x3((float)GetDoubleValue(CameraMatrix, 0, 0), (float)GetDoubleValue(CameraMatrix, 0, 1), (float)GetDoubleValue(CameraMatrix, 0, 2),
                    (float)GetDoubleValue(CameraMatrix, 1, 0), (float)GetDoubleValue(CameraMatrix, 1, 1), (float)GetDoubleValue(CameraMatrix, 1, 2),
                    (float)GetDoubleValue(CameraMatrix, 2, 0), (float)GetDoubleValue(CameraMatrix, 2, 1), (float)GetDoubleValue(CameraMatrix, 2, 2));

                SharpDX.Matrix3x3 M_Inverted = M;
                M_Inverted.Invert();

                //Se invierte la matriz de rotación
                SharpDX.Matrix3x3 R = new SharpDX.Matrix3x3((float)RotationMatrix[0, 0], (float)RotationMatrix[0, 1], (float)RotationMatrix[0, 2],
                    (float)RotationMatrix[1, 0], (float)RotationMatrix[1, 1], (float)RotationMatrix[1, 2],
                    (float)RotationMatrix[2, 0], (float)RotationMatrix[2, 1], (float)RotationMatrix[2, 2]);

                SharpDX.Matrix3x3 R_Inverted = R;
                R_Inverted.Invert();

                //Se calcula el vector de translacion inverso Tinv=-Rint*T
                SharpDX.Vector3 T = new SharpDX.Vector3((float)TranslationVector[0], (float)TranslationVector[1], (float)TranslationVector[2]);
                SharpDX.Vector3 T_Inverted = (-1) * multiplyMatrix3x3ByVector3(R_Inverted, T);


                //Para cada pixel se calculan las coordenadas X,Y reales
                //double z = 0;
                world_points = new SharpDX.Vector3[undistortedPoints.Length];

                for (int i = 0; i < undistortedPoints.Length; i++)
                {
                    //Se calculan las coordenadas del Pixel en el Stma camara
                    SharpDX.Vector3 PpixelH = new SharpDX.Vector3(undistortedPoints[i].X, undistortedPoints[i].Y, 1);
                    SharpDX.Vector3 Pcamera = multiplyMatrix3x3ByVector3(M_Inverted, PpixelH);
                    SharpDX.Vector3 Pworld = multiplyMatrix3x3ByVector3(R_Inverted, (Pcamera - T));

                    double l = (z - T_Inverted.Z) / (Pworld.Z - T_Inverted.Z);
                    //Una vez obtenida l(lambda) se calculan las coordenadas X,Y
                    world_points[i] = new SharpDX.Vector3();
                    world_points[i].X = T_Inverted.X + (float)l * (Pworld.X - T_Inverted.X);
                    world_points[i].Y = T_Inverted.Y + (float)l * (Pworld.Y - T_Inverted.Y);
                    world_points[i].Z = T_Inverted.Z + (float)l * (Pworld.Z - T_Inverted.Z); //Esto es lo mismo que igualarlo a z

                }

                //PointF[] punto = CvInvoke.ProjectPoints(newPoint, rvecs, tvecs, _cameraMatrix, _distCoeffs, jacobian, 0.0);

            }
            catch (Exception ex)
            {

            }


            return new PointF(world_points[0].X, world_points[0].Y);
        }

        /// <summary>
        /// Función para obtener las coordenadas reales de un punto en la imagen
        /// </summary>
        /// <param name="imagePoint"></param>
        /// <returns></returns>
        public PointF MapPoint(PointF imagePoint)
        {
            SharpDX.Vector3[] world_points = new SharpDX.Vector3[1];

            try
            {

                //Punto 
                float X = imagePoint.X;
                float Y = imagePoint.Y;

                Mat distPts = new Mat(2, 1, DepthType.Cv64F, 1);
                SetDoubleValue(distPts, 0, 0, X);
                SetDoubleValue(distPts, 0, 1, Y);

                Mat undistPts = new Mat(2, 1, DepthType.Cv64F, 1);

                CvInvoke.UndistortPoints(distPts, undistPts, CameraMatrix, DistCoeffs, null, CameraMatrix);

                SharpDX.Vector3[] undistortedPoints = new SharpDX.Vector3[] { new SharpDX.Vector3((float)((double[])undistPts.GetData(false))[0], (float)((double[])undistPts.GetData(false))[1], 0) };

                //Convertir un punto de coordenadas en imagen a reales

                //Se invierte la matriz camera_matrix de la camara
                SharpDX.Matrix3x3 M = new SharpDX.Matrix3x3((float)GetDoubleValue(CameraMatrix, 0, 0), (float)GetDoubleValue(CameraMatrix, 0, 1), (float)GetDoubleValue(CameraMatrix, 0, 2),
                    (float)GetDoubleValue(CameraMatrix, 1, 0), (float)GetDoubleValue(CameraMatrix, 1, 1), (float)GetDoubleValue(CameraMatrix, 1, 2),
                    (float)GetDoubleValue(CameraMatrix, 2, 0), (float)GetDoubleValue(CameraMatrix, 2, 1), (float)GetDoubleValue(CameraMatrix, 2, 2));

                SharpDX.Matrix3x3 M_Inverted = M;
                M_Inverted.Invert();

                //Se invierte la matriz de rotación
                SharpDX.Matrix3x3 R = new SharpDX.Matrix3x3((float)RotationMatrix[0, 0], (float)RotationMatrix[0, 1], (float)RotationMatrix[0, 2],
                    (float)RotationMatrix[1, 0], (float)RotationMatrix[1, 1], (float)RotationMatrix[1, 2],
                    (float)RotationMatrix[2, 0], (float)RotationMatrix[2, 1], (float)RotationMatrix[2, 2]);

                SharpDX.Matrix3x3 R_Inverted = R;
                R_Inverted.Invert();

                //Se calcula el vector de translacion inverso Tinv=-Rint*T
                SharpDX.Vector3 T = new SharpDX.Vector3((float)TranslationVector[0], (float)TranslationVector[1], (float)TranslationVector[2]);
                SharpDX.Vector3 T_Inverted = (-1) * multiplyMatrix3x3ByVector3(R_Inverted, T);


                //Para cada pixel se calculan las coordenadas X,Y reales
                double z = 0;
                world_points = new SharpDX.Vector3[undistortedPoints.Length];

                for (int i = 0; i < undistortedPoints.Length; i++)
                {
                    //Se calculan las coordenadas del Pixel en el Stma camara
                    SharpDX.Vector3 PpixelH = new SharpDX.Vector3(undistortedPoints[i].X, undistortedPoints[i].Y, 1);
                    SharpDX.Vector3 Pcamera = multiplyMatrix3x3ByVector3(M_Inverted, PpixelH);
                    SharpDX.Vector3 Pworld = multiplyMatrix3x3ByVector3(R_Inverted, (Pcamera - T));

                    double l = (-z - T_Inverted.Z) / (Pworld.Z - T_Inverted.Z);
                    //Una vez obtenida l(lambda) se calculan las coordenadas X,Y
                    world_points[i] = new SharpDX.Vector3();
                    world_points[i].X = T_Inverted.X + (float)l * (Pworld.X - T_Inverted.X);
                    world_points[i].Y = T_Inverted.Y + (float)l * (Pworld.Y - T_Inverted.Y);
                    world_points[i].Z = T_Inverted.Z + (float)l * (Pworld.Z - T_Inverted.Z);

                    //Esto es lo mismo que igualarlo a z
                }

                //PointF[] punto = CvInvoke.ProjectPoints(newPoint, rvecs, tvecs, _cameraMatrix, _distCoeffs, jacobian, 0.0);

            }
            catch (Exception ex)
            {

            }


            return new PointF(world_points[0].X, world_points[0].Y);
        }

        /// <summary>
        /// Función para obtener las coordenadas reales de un punto en la imagen usando la calibración del segundo plano
        /// </summary>
        /// <param name="imagePoint"></param>
        /// <returns></returns>
        public PointF MapPoint2Plane(PointF imagePoint)
        {
            SharpDX.Vector3[] world_points = new SharpDX.Vector3[1];

            try
            {
                //Punto 
                float X = imagePoint.X;
                float Y = imagePoint.Y;

                Mat distPts = new Mat(2, 1, DepthType.Cv64F, 1);
                SetDoubleValue(distPts, 0, 0, X);
                SetDoubleValue(distPts, 0, 1, Y);

                Mat undistPts = new Mat(2, 1, DepthType.Cv64F, 1);

                CvInvoke.UndistortPoints(distPts, undistPts, CameraMatrix2Plane, DistCoeffs2Plane, null, CameraMatrix2Plane);

                //SharpDX.Vector3[] undistortedPoints = new SharpDX.Vector3[] { new SharpDX.Vector3(672, 934, 0) };
                SharpDX.Vector3[] undistortedPoints = new SharpDX.Vector3[] { new SharpDX.Vector3((float)((double[])undistPts.GetData(false))[0], (float)((double[])undistPts.GetData(false))[1], 0) };

                //Convertir un punto de coordenadas en imagen a reales

                //Se invierte la matriz camera_matrix de la camara
                SharpDX.Matrix3x3 M = new SharpDX.Matrix3x3((float)GetDoubleValue(CameraMatrix2Plane, 0, 0), (float)GetDoubleValue(CameraMatrix2Plane, 0, 1), (float)GetDoubleValue(CameraMatrix2Plane, 0, 2),
                    (float)GetDoubleValue(CameraMatrix2Plane, 1, 0), (float)GetDoubleValue(CameraMatrix2Plane, 1, 1), (float)GetDoubleValue(CameraMatrix2Plane, 1, 2),
                    (float)GetDoubleValue(CameraMatrix2Plane, 2, 0), (float)GetDoubleValue(CameraMatrix2Plane, 2, 1), (float)GetDoubleValue(CameraMatrix2Plane, 2, 2));

                SharpDX.Matrix3x3 M_Inverted = M;
                M_Inverted.Invert();

                //Se invierte la matriz de rotación
                SharpDX.Matrix3x3 R = new SharpDX.Matrix3x3((float)RotationMatrix2Plane[0, 0], (float)RotationMatrix2Plane[0, 1], (float)RotationMatrix2Plane[0, 2], (float)RotationMatrix2Plane[1, 0], (float)RotationMatrix2Plane[1, 1], (float)RotationMatrix2Plane[1, 2], (float)RotationMatrix2Plane[2, 0], (float)RotationMatrix2Plane[2, 1], (float)RotationMatrix2Plane[2, 2]);

                SharpDX.Matrix3x3 R_Inverted = R;
                R_Inverted.Invert();

                //Se calcula el vector de translacion inverso Tinv=-Rint*T
                //T_Inverted = R_Inverted * T * (-1);
                SharpDX.Vector3 T = new SharpDX.Vector3((float)TranslationVector2Plane[0], (float)TranslationVector2Plane[1], (float)TranslationVector2Plane[2]);
                SharpDX.Vector3 T_Inverted = (-1) * multiplyMatrix3x3ByVector3(R_Inverted, T);


                //Para cada pixel se calculan las coordenadas X,Y reales
                double z = 0;
                world_points = new SharpDX.Vector3[undistortedPoints.Length];

                for (int i = 0; i < undistortedPoints.Length; i++)
                {
                    //Se calculan las coordenadas del Pixel en el Stma camara
                    SharpDX.Vector3 PpixelH = new SharpDX.Vector3(undistortedPoints[i].X, undistortedPoints[i].Y, 1);
                    SharpDX.Vector3 Pcamera = multiplyMatrix3x3ByVector3(M_Inverted, PpixelH);
                    SharpDX.Vector3 Pworld = multiplyMatrix3x3ByVector3(R_Inverted, (Pcamera - T));

                    double l = (-z - T_Inverted.Z) / (Pworld.Z - T_Inverted.Z);
                    //Una vez obtenida l(lambda) se calculan las coordenadas X,Y
                    world_points[i] = new SharpDX.Vector3();
                    world_points[i].X = T_Inverted.X + (float)l * (Pworld.X - T_Inverted.X);
                    world_points[i].Y = T_Inverted.Y + (float)l * (Pworld.Y - T_Inverted.Y);
                    world_points[i].Z = T_Inverted.Z + (float)l * (Pworld.Z - T_Inverted.Z);//Esto es lo mismo que igualarlo a z
                }

                //PointF[] punto = CvInvoke.ProjectPoints(newPoint, rvecs, tvecs, _cameraMatrix, _distCoeffs, jacobian, 0.0);

            }
            catch (Exception ex)
            {

            }                       

            return new PointF(world_points[0].X, world_points[0].Y);

        }

        /// <summary>
        /// Función para calcular un punto en coordenadas reales que está a una distancia conocida del plano ppal de calibración
        /// </summary>
        /// <param name="imagePoint"></param> Coordenadas del punto en la imagen
        /// <param name="Plane2Distance"></param> Distancia entre plano de calibración ppal y segundo plano paralelo
        /// <param name="worldDistance"></param> Distancia real del punto a obtener al plano de calibración ppal
        /// <returns></returns>
        public PointF GetPoint3D(PointF imagePoint, double worldDistance, double plane2Distance)
        {
            PointF worldPoint = new PointF();

            Plane2Z = plane2Distance;

            try
            {
                //Punto 
                float X = imagePoint.X;
                float Y = imagePoint.Y;

                //Puntos reales
                PointF puntoBase = MapPoint(imagePoint);
                PointF puntoAltura = MapPoint2Plane(imagePoint);

                //Interpolación
                double Z_desdeArriba = Convert.ToDouble(Plane2Z) - worldDistance;

                //Ponderación de las coordenadas X e Y en función de la Z objetivo                
                worldPoint.X = ((float)Z_desdeArriba / (float)Plane2Z) * (puntoBase.X - puntoAltura.X) + puntoAltura.X;
                worldPoint.Y = ((float)Z_desdeArriba / (float)Plane2Z) * (puntoBase.Y - puntoAltura.Y) + puntoAltura.Y;

            }
            catch(Exception ex)
            {

            }

            return worldPoint;

        }

        /// <summary>
        /// Función para guardar los parámetros de calibración
        /// </summary>
        /// <param name="xmlPath"></param>
        public void ExportCalibration(string xmlPath)
        {
            try
            {
                //Creamos el objeto para la serialización
                CalibrationParameters CalibS = new CalibrationParameters(new Calibration(new ExtrinsicCalibration(RotationMatrix, TranslationVector), new IntrinsicCalibration(CameraMatrixD, DistCoeffsD)),
                new Calibration(new ExtrinsicCalibration(RotationMatrix2Plane, TranslationVector2Plane), new IntrinsicCalibration(CameraMatrix2PlaneD, DistCoeffs2PlaneD)));

                //Serializamos
                CalibS.SaveCalibrationParams(xmlPath);
            }
            catch(Exception ex)
            {

            }
        }

        /// <summary>
        /// Función para cargar los parámetros de calibración
        /// </summary>
        /// <param name="xmlPath"></param>
        public void ImportCalibration(string xmlPath)
        {
            try
            {
                //Creamos el objeto vacío
                CalibrationParameters CalibL = new CalibrationParameters();

                //Deserializamos
                CalibL.LoadCalibrationParams(xmlPath);

                //Rellenamos las variables
                Calibration calibBase = CalibL.BasePlaneCalibration;
                Calibration calibPlane = CalibL.Plane2Calibration;


                //Calib params
                CameraMatrixD = convertStringMat2DoubleMat(calibBase.CameraMatrix);
                DistCoeffsD = convertStringArray2DoubleArray(calibBase.DistortionCoeffs);
                TranslationVector = convertStringArray2DoubleArray(calibBase.TraslationVector);
                RotationMatrix = convertStringMat2DoubleMat(calibBase.RotationMatrix);

                //Extra Image Calib params
                CameraMatrix2PlaneD = convertStringMat2DoubleMat(calibPlane.CameraMatrix);
                DistCoeffs2PlaneD = convertStringArray2DoubleArray(calibPlane.DistortionCoeffs);
                TranslationVector2Plane = convertStringArray2DoubleArray(calibPlane.TraslationVector);
                RotationMatrix2Plane = convertStringMat2DoubleMat(calibPlane.RotationMatrix);

                //Estados de calibración
                if(CameraMatrixD[0,0] != 0.0)
                    CalibResult = Estado.CALIBRATED;
                if (CameraMatrix2PlaneD[0, 0] != 0.0)
                    Plane2CalibResult = Estado.CALIBRATED;


            }
            catch(Exception ex)
            {

            }
        }
               
        /// <summary>
        /// Función para convertir de array de strings a array de doubles
        /// </summary>
        /// <param name="stringArray"></param>
        /// <returns></returns>
        private double[] convertStringArray2DoubleArray(string[] stringArray)
        {
            double[] doubleArray = new double[stringArray.Length];

            for (int i = 0; i < stringArray.Length; i++)
                doubleArray[i] = Convert.ToDouble(stringArray[i]);

            return doubleArray;
        }

        /// <summary>
        /// Función para convertir de matriz de strings a matriz de doubles
        /// </summary>
        /// <param name="stringArray"></param>
        /// <returns></returns>
        private double[,] convertStringMat2DoubleMat(string[][] stringArray)
        {
            double[,] doubleArray = new double[3, 3];

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    doubleArray[i, j] = Convert.ToDouble(stringArray[i][j]);

            return doubleArray;
        }

        #endregion

        #region Mat managing methods

        public static double GetDoubleValue(Mat mat, int row, int col)
        {
            var value = new double[1];
            Marshal.Copy(mat.DataPointer + (row * mat.Cols + col) * mat.ElementSize, value, 0, 1);
            return value[0];
        }

        public static double GetDoubleValue(Mat mat, int row, int col, int chn)
        {
            var value = new double[1];
            Marshal.Copy(mat.DataPointer + (row * mat.Cols * mat.NumberOfChannels + col * mat.NumberOfChannels + chn) * mat.ElementSize, value, 0, 1);
            return value[0];
        }

        public static void SetDoubleValue(Mat mat, int row, int col, double value)
        {
            var target = new[] { value };
            Marshal.Copy(target, 0, mat.DataPointer + (row * mat.Cols + col) * mat.ElementSize, 1);
        }

        public static SharpDX.Vector3 multiplyVector3ByMatrix3x3(SharpDX.Vector3 vector, SharpDX.Matrix3x3 matrix)
        {
            SharpDX.Vector3 vectorResult = new SharpDX.Vector3();

            vectorResult[0] = vector[0] * matrix.M11 + vector[1] * matrix.M21 + vector[2] * matrix.M31;
            vectorResult[1] = vector[0] * matrix.M12 + vector[1] * matrix.M22 + vector[2] * matrix.M32;
            vectorResult[2] = vector[0] * matrix.M13 + vector[1] * matrix.M23 + vector[2] * matrix.M33;

            return vectorResult;
        }
        public static SharpDX.Vector3 multiplyMatrix3x3ByVector3(SharpDX.Matrix3x3 matrix, SharpDX.Vector3 vector)
        {
            SharpDX.Vector3 vectorResult = new SharpDX.Vector3();

            vectorResult[0] = vector[0] * matrix.M11 + vector[1] * matrix.M12 + vector[2] * matrix.M13;
            vectorResult[1] = vector[0] * matrix.M21 + vector[1] * matrix.M22 + vector[2] * matrix.M23;
            vectorResult[2] = vector[0] * matrix.M31 + vector[1] * matrix.M32 + vector[2] * matrix.M33;

            return vectorResult;
        }
        #endregion

    }

    #region Clases Serializables

    public class CalibrationParameters
    {

        #region Fields
        private string xmlCalibParamsPath;
        #endregion

        #region Properties

        /// <summary>
        /// Calibración principal
        /// </summary>
        public Calibration BasePlaneCalibration { get; set; }

        /// <summary>
        /// Calibración principal
        /// </summary>
        public Calibration Plane2Calibration { get; set; }

        #endregion

        #region Constructor
        public CalibrationParameters()
        {
            BasePlaneCalibration = new Calibration();
            Plane2Calibration = new Calibration();
        }

        public CalibrationParameters(Calibration BaseCalib, Calibration Plane2Calib)
        {
            BasePlaneCalibration = new Calibration(new ExtrinsicCalibration(convertStringMat2DoubleMat(BaseCalib.RotationMatrix), convertStringArray2DoubleArray(BaseCalib.TraslationVector)),
                new IntrinsicCalibration(convertStringMat2DoubleMat(BaseCalib.CameraMatrix) ,convertStringArray2DoubleArray(BaseCalib.DistortionCoeffs)));

            Plane2Calibration = new Calibration(new ExtrinsicCalibration(convertStringMat2DoubleMat(Plane2Calib.RotationMatrix), convertStringArray2DoubleArray(Plane2Calib.TraslationVector)),
                new IntrinsicCalibration(convertStringMat2DoubleMat(Plane2Calib.CameraMatrix), convertStringArray2DoubleArray(Plane2Calib.DistortionCoeffs)));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Metodo para serializar 
        /// </summary>
        /// <param name="XmlPath">Archivo xml</param>
        /// <returns></returns>
        public async void LoadCalibrationParams(string xmlPath)
        {
            
            //Creamos el file stream con permisos de solo lectura
            if (File.Exists(xmlPath))
            {
                //Ruta de deserialización
                xmlCalibParamsPath = xmlPath;

                //Lista calibraciones
                List<Calibration> calib = new List<Calibration>();

                var fs = new System.IO.FileStream(xmlPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                XmlSerializer xs = new XmlSerializer(typeof(List<Calibration>));

                using (StreamReader sr = new StreamReader(fs))
                {
                    //Var para indicar si hay error de lectura
                    bool IsReadError = true;

                    //Hacemos varios intentos de leer el archivo, alguna vez da error si el archivo esta abierto por diferentes procesos
                    for (int i = 0; i < 3; i++)
                    {
                        try
                        {
                            //Deserializo
                            calib = (List<Calibration>)xs.Deserialize(sr);

                            //Rellenar datos
                            BasePlaneCalibration = calib[0];
                            Plane2Calibration = calib[1];

                            IsReadError = false;

                            break;
                        }
                        catch (Exception)
                        {
                            await Task.Delay(1000);
                        }
                    }

                    if (IsReadError) throw new Exception("Error on load Calibration info =>" + xmlPath);
                }
            }

        }
        
        /// <summary>
        /// Metodo para guardar la info
        /// </summary>
        /// <returns></returns>
        public void SaveCalibrationParams(string xmlCalib)
        {
            //Creamos la lista de calibs
            List<Calibration> calib = new List<Calibration>();

            calib.Add(BasePlaneCalibration);
            calib.Add(Plane2Calibration);

            XmlSerializer xs = new XmlSerializer(typeof(List<Calibration>));
            using (StreamWriter sw = new StreamWriter(xmlCalib))
            {
                try
                {
                    //setParams();
                    xs.Serialize(sw, calib);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
                
        /// <summary>
        /// Función para convertir de array de strings a array de doubles
        /// </summary>
        /// <param name="stringArray"></param>
        /// <returns></returns>
        private double[] convertStringArray2DoubleArray(string[] stringArray)
        {
            double[] doubleArray = new double[stringArray.Length];

            for (int i = 0; i < stringArray.Length; i++)
                doubleArray[i] = Convert.ToDouble(stringArray[i]);

            return doubleArray;
        }

        /// <summary>
        /// Función para convertir de matriz de strings a matriz de doubles
        /// </summary>
        /// <param name="stringArray"></param>
        /// <returns></returns>
        private double[,] convertStringMat2DoubleMat(string[][] stringArray)
        {
            double[,] doubleArray = new double[3, 3];

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    doubleArray[i,j] = Convert.ToDouble(stringArray[i][j]);

            return doubleArray;
        }

        /*
        #region Old Methods
        /// <summary>
        /// Metodo para serializar 
        /// </summary>
        /// <param name="XmlPath">Archivo xml</param>
        /// <returns></returns>
        public async void LoadIntrinsicParams(string xmlPath)
        {
            xmlCalibParamsPath = xmlPath;

            //Creamos el file stream con permisos de solo lectura
            if (File.Exists(xmlPath))
            {
                var fs = new System.IO.FileStream(xmlPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                XmlSerializer xs = new XmlSerializer(typeof(IntrinsicCalibration));

                using (StreamReader sr = new StreamReader(fs))
                {
                    //Var para indicar si hay error de lectura
                    bool IsReadError = true;

                    //Hacemos varios intentos de leer el archivo, alguna vez da error si el archivo esta abierto por diferentes procesos
                    for (int i = 0; i < 3; i++)
                    {
                        try
                        {
                            //Deserealizo
                            IntrinsicParams = (IntrinsicCalibration)xs.Deserialize(sr);

                            IsReadError = false;

                            break;
                        }
                        catch (Exception)
                        {
                            await Task.Delay(1000);
                        }
                    }

                    if (IsReadError) throw new Exception("Error on load Intrinsic Calibration info =>" + xmlIntrinsicCalibration);
                }
            }

        }

        /// <summary>
        /// Metodo para guardar la info
        /// </summary>
        /// <returns></returns>
        public void SaveIntrinsicParams()
        {
            XmlSerializer xs = new XmlSerializer(typeof(IntrinsicCalibration));
            using (StreamWriter sw = new StreamWriter(xmlIntrinsicCalibration))
            {
                try
                {
                    //setParams();
                    xs.Serialize(sw, IntrinsicParams);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Metodo para serializar
        /// </summary>
        /// <param name="XmlPath">Archivo xml</param>
        /// <returns></returns>
        public async void LoadExtrinsicParams(string xmlPath)
        {

            xmlExtrinsicCalibration = xmlPath;

            //Creamos el file stream con permisos de solo lectura

            if (File.Exists(xmlExtrinsicCalibration))
            {
                var fs = new System.IO.FileStream(xmlPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                XmlSerializer xs = new XmlSerializer(typeof(ExtrinsicCalibration));

                using (StreamReader sr = new StreamReader(fs))
                {
                    //Var para indicar si hay error de lectura
                    bool IsReadError = true;

                    //Hacemos varios intentos de leer el archivo, alguna vez da error si el archivo esta abierto por diferentes procesos
                    for (int i = 0; i < 3; i++)
                    {
                        try
                        {
                            //Deserealizo
                            ExtrinsicParams = (ExtrinsicCalibration)xs.Deserialize(sr);

                            IsReadError = false;

                            break;
                        }
                        catch (Exception)
                        {
                            await Task.Delay(1000);
                        }
                    }

                    if (IsReadError) throw new Exception("Error on load models info =>" + xmlExtrinsicCalibration);
                }
            }

        }

        /// <summary>
        /// Metodo para guardar la info
        /// </summary>
        /// <returns></returns>
        public void SaveExtrinsicParams()
        {
            XmlSerializer xs = new XmlSerializer(typeof(ExtrinsicCalibration));
            using (StreamWriter sw = new StreamWriter(xmlExtrinsicCalibration))
            {
                try
                {
                    //setParams();
                    xs.Serialize(sw, ExtrinsicParams);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        #endregion

        */

        #endregion

    }
       
    [Serializable]
    public class Calibration
    {
        #region Properties
        /// <summary>
        /// Coeficientes de distorsion de la lente
        /// </summary>
        public string[] TraslationVector { get; set; }
        /// <summary>
        /// Coeficientes de distorsion de la lente
        /// </summary>
        public string[][] RotationMatrix { get; set; }
        /// <summary>
        /// Coeficientes de distorsion de la lente
        /// </summary>
        public string[] DistortionCoeffs { get; set; }
        /// <summary>
        /// Coeficientes de distorsion de la lente
        /// </summary>
        public string[][] CameraMatrix { get; set; }

        #endregion

        #region Constructor
        public Calibration()
        {
            TraslationVector = new string[3];
            RotationMatrix = new string[3][];
            DistortionCoeffs = new string[7];
            CameraMatrix = new string[3][];
        }

        public Calibration(ExtrinsicCalibration extrCalib, IntrinsicCalibration intrCalib)
        {
            TraslationVector = new string[3];
            for (int i = 0; i < 3; i++)
                TraslationVector[i] = extrCalib.TraslationVector[i].ToString("0.000000000");

            RotationMatrix = new string[3][];
            RotationMatrix[0] = new string[3];
            RotationMatrix[1] = new string[3];
            RotationMatrix[2] = new string[3];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    RotationMatrix[i][j] = extrCalib.RotationMatrix[i, j].ToString("0.000000000");
                       

            DistortionCoeffs = new string[7];
            for (int i = 0; i < 7; i++)
                DistortionCoeffs[i] = intrCalib.DistortionCoeffs[i].ToString("0.000000000");

            CameraMatrix = new string[3][];
            CameraMatrix[0] = new string[3];
            CameraMatrix[1] = new string[3];
            CameraMatrix[2] = new string[3];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    CameraMatrix[i][j] = intrCalib.CameraMatrix[i, j].ToString("0.000000000");
        }

        #endregion

    }

    [Serializable]
    public class IntrinsicCalibration
    {
        #region Properties
        /// <summary>
        /// Coeficientes de distorsion de la lente
        /// </summary>
        public double[] DistortionCoeffs { get; set; }
        /// <summary>
        /// Coeficientes de distorsion de la lente
        /// </summary>
        public double[,] CameraMatrix { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public IntrinsicCalibration(double[,] cameraMatrix, double[] distCoeffs)
        {
            DistortionCoeffs = new double[7];
            DistortionCoeffs = distCoeffs;
            CameraMatrix = new double[3, 3];
            CameraMatrix = cameraMatrix;
        }

        public IntrinsicCalibration()
        {
            DistortionCoeffs = new double[7];
            CameraMatrix = new double[3, 3];
        }
        #endregion
    }

    [Serializable]
    public class ExtrinsicCalibration
    {
        #region Properties
        /// <summary>
        /// Coeficientes de distorsion de la lente
        /// </summary>
        public double[] TraslationVector { get; set; }
        /// <summary>
        /// Coeficientes de distorsion de la lente
        /// </summary>
        public double[,] RotationMatrix { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public ExtrinsicCalibration(double[,] rotationMatrix, double[] traslationVector)
        {
            TraslationVector = new double[3];
            TraslationVector = traslationVector;
            RotationMatrix = new double[3, 3];
            RotationMatrix = rotationMatrix;
        }

        public ExtrinsicCalibration()
        {
            TraslationVector = new double[3];
            RotationMatrix = new double[3, 3];
        }
        #endregion
    }

    #endregion
}





