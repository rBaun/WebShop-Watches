﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DesktopClient {
    /// <summary>
    /// Interaction logic for AddImagesWindow.xaml
    /// </summary>
    public partial class AddImagesWindow : Window {
        public AddImagesWindow() {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void CreateImageButton_Click(object sender, RoutedEventArgs e) {

            if(ImageURLTextBox.Text != "" && ImageNameTextBox.Text != "") {
                CrudOverview.ImageURL = ImageURLTextBox.Text;
                CrudOverview.ImageName = ImageNameTextBox.Text;

                Close();
            }
            else {
                MessageBox.Show("Ugyldig tekst indsat");
            }
        }
    }
}
