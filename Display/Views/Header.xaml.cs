//using bcnvision.Data;
using NLog;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;

namespace bcnvision.Views
{
    /// <summary>
    /// Interaction logic for Header.xaml
    /// </summary>
    public partial class HeaderView : UserControl, INotifyPropertyChanged
    {
        #region Fields
        private string recipe;
        private bool loginHeader;
        private BitmapImage customerLogo;
        private Logger logger;
        private GridLength closeButtonWidht;
        #endregion

        #region Properties
        public string Recipe
        {
            get { return recipe; }
            set
            {
                recipe = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Recipe"));
            }
        }
        public bool LoginHeader
        {
            get { return loginHeader; }
            set
            {
                loginHeader = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LoginHeader"));
            }
        }
        public BitmapImage CustomerLogo
        {
            get { return customerLogo; }
            set
            {
                customerLogo = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CustomerLogo"));
            }
        }
        public GridLength CloseButtonWidht
        {
            get { return closeButtonWidht; }
            set { closeButtonWidht = value; }
        }

        #endregion

        #region Delegates
        /// <summary>
        /// Envento nuevo mensaje tcp
        /// </summary>
        /// <param name="Tag"></param>
        /// <param name="Value"></param>
        public delegate void OnCloseEvent();
        public event OnCloseEvent OnClose;
        #endregion

        #region Constructor
        public HeaderView()
        {

            logger = LogManager.GetLogger("Header");
            InitializeComponent();
            DataContext = this;

            try
            {
                ////Buscamos si existe el logo del cliente sin tener en cuenta la extension
                //string[] files = Directory.GetFiles(bcnvisionHMI.FolderManager.ConfigFolder).Where(x => Path.GetFileNameWithoutExtension(x) == "CustomerLogo").ToArray();
                ////Cargamos el logo del cliente
                //if (files.Count() > 0)
                //{
                //    CustomerLogo = new BitmapImage(new Uri(files[0]));
                //}
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Evento que se ha echo click en un boton del header
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HeaderButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                //Instanciamos el nombre del boton que se ha pulsado
                string Name = ((ToggleButton)sender).Name;

                //Desmarcamos los botones que no se han clickado
                //switch (Name)
                //{
                //    case "SettingsButton":
                //        MainButton.IsChecked = false;
                //        LoginButton.IsChecked = false;
                //        InfoButton.IsChecked = false;
                //        break;
                //    case "LoginButton":
                //        MainButton.IsChecked = false;
                //        SettingsButton.IsChecked = false;
                //        InfoButton.IsChecked = false;
                //        break;
                //    case "InfoButton":
                //        MainButton.IsChecked = false;
                //        LoginButton.IsChecked = false;
                //        SettingsButton.IsChecked = false;
                //        break;
                //    default:
                //        SettingsButton.IsChecked = false;
                //        LoginButton.IsChecked = false;
                //        InfoButton.IsChecked = false;
                //        break;
                //}
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        /// <summary>
        /// Evento cierre aplicacion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((MessageBox.Show("Do you want to close the application", "Closing", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes))
                {
                    OnClose?.Invoke();
                }

                //Le forzamos que no este checked
                CloseButton.IsChecked = false;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        #endregion

    }
}
