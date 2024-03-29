﻿using POSUNO.Components;
using POSUNO.Helpers;
using POSUNO.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace POSUNO.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = await ValidForm();
#if DEBUG
            EmailTextBox.Text = "santyagor@outlook.com";
            PasswordPasswordBox.Password = "123456";
#endif
            if (!isValid)
            {
                return;
            }

            Loader loader = new Loader("Por favor espere...");
            loader.Show();

            Response response = await ApiService.LoginAsync(new LoginRequest
            {
                Email = EmailTextBox.Text,
                Password = PasswordPasswordBox.Password,
            });

            loader.Close();
            MessageDialog messageDialog;
            if (!response.IsSuccess)
            {
                messageDialog = new MessageDialog("usuario o contraseña incorrectos", "Error");
                await messageDialog.ShowAsync();
                return;
            }

            TokenResponse tokenResponse = (TokenResponse)response.Result;
            Frame.Navigate(typeof(MainPage), tokenResponse);
        }

        private async Task<bool> ValidForm()
        {
            MessageDialog messageDialog;
            if(string.IsNullOrEmpty(EmailTextBox.Text))
            {
                messageDialog = new MessageDialog("Debes ingresar tu email","Error");
                await messageDialog.ShowAsync();
                return false;
            }
            if (!RegexUtilities.IsValidEmail(EmailTextBox.Text))
            {
                messageDialog = new MessageDialog("Debes ingresar un email válido", "Error");
                await messageDialog.ShowAsync();
                return false;
            }

            if (PasswordPasswordBox.Password.Length<6)
            {
                messageDialog = new MessageDialog("Debes ingresar tu contraseña de al menos (6) caracteres", "Error");
                await messageDialog.ShowAsync();
                return false;
            }

            return true;
        }
    }
}
