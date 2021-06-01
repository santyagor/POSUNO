using POSUNO.Components;
using POSUNO.Helpers;
using POSUNO.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class CustomersPage : Page
    {
        public ObservableCollection<Customer> Customers { get; set; }
        public CustomersPage()
        {
            this.InitializeComponent();
        }        

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadCustomersAsync();
        }

        private async void LoadCustomersAsync()
        {
            Loader loader = new Loader("Por favor espere...");
            loader.Show();
            Response response = await ApiService.GetListAsync<Customer>("customers");
            loader.Close();
            if(!response.IsSuccess)
            {
                MessageDialog dialog = new MessageDialog(response.Message, "Error");
                await dialog.ShowAsync();
                return;
            }

            List<Customer> customers = (List<Customer>)response.Result;
            Customers = new ObservableCollection<Customer>(customers);
            RefreshList();
        }

        private void RefreshList()
        {
            CustomersListView.ItemsSource = null;
            CustomersListView.Items.Clear();
            CustomersListView.ItemsSource = Customers;
        }
    }
}
