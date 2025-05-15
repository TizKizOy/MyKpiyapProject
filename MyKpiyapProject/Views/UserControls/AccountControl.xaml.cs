using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MyKpiyapProject.NewModels;

namespace MyKpiyapProject.UserControls
{
    /// <summary>
    /// Логика взаимодействия для AccountControl.xaml
    /// </summary>
    public partial class AccountControl : UserControl
    {
        public AccountControl()
        {
            InitializeComponent();
        }

        public AccountControl(tbEmployee user)
        {
            InitializeComponent();
            LoadUserData(user);
            LoadUserPhoto(user.Photo);
        }

        private void LoadUserData(tbEmployee user)
        {
            string Name = user.FullName.Split(' ')[0];
            string LastName = user.FullName.Split(' ')[1];
            textBoxName.Text = Name;
            textBoxLastName.Text = LastName;
            textBoxEmail.Text = user.Email;
            textBoxPosition.Text = user.PositionAndRole;
            textBlockName.Text = Name.ToUpper();
            textBlockLastName.Text = LastName.ToUpper();
        }

        private void LoadUserPhoto(byte[] photoData)
        {
            if (photoData != null && photoData.Length > 0)
            {
                using (var ms = new System.IO.MemoryStream(photoData))
                {
                    var image = new System.Windows.Media.Imaging.BitmapImage();
                    image.BeginInit();
                    image.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();

                    userImage.Source = image;
                }
            }
            else
            {
                userImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("../Image/default.png", UriKind.Relative));
            }
        }
    }
}
