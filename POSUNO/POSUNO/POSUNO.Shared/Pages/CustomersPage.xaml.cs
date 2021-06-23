using POSUNO.Components;
using POSUNO.Dialogs;
using POSUNO.Helpers;
using POSUNO.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            Response response = await ApiService.GetListAsync<Customer>("customers",MainPage.GetInstance().TokenResponse.Token);
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

        private async void AddCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = new Customer();
            CustomerDialog dialog = new CustomerDialog(customer);
            await dialog.ShowAsync();
            if(!customer.WasSaved)
            {
                return;
            }


            Loader loader = new Loader("Por favor espere...");
            loader.Show();
            Response response = await ApiService.PostAsync("Customers", customer, MainPage.GetInstance().TokenResponse.Token);

            loader.Close();

            if(!response.IsSuccess)
            {
                MessageDialog dialogMessage = new MessageDialog(response.Message, "Error");
                await dialogMessage.ShowAsync();
                return;
            }

            Customer newCustomer = (Customer)response.Result;
            Customers.Add(newCustomer);
            RefreshList();
        }

        private async void EditImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Customer customer = Customers[CustomersListView.SelectedIndex];
            customer.IsEdit = true;
            CustomerDialog dialog = new CustomerDialog(customer);
            await dialog.ShowAsync();

            if (!customer.WasSaved)
            {
                return;
            }

            Loader loader = new Loader("Por favor espere...");
            loader.Show();
            Response response = await ApiService.PutAsync("Customers", customer,customer.Id, MainPage.GetInstance().TokenResponse.Token);

            loader.Close();

            if (!response.IsSuccess)
            {
                MessageDialog dialogMessage = new MessageDialog(response.Message, "Error");
                await dialogMessage.ShowAsync();
                return;
            }

            Customer newCustomer = (Customer)response.Result;
            Customer oldCustomer = Customers.FirstOrDefault(c => c.Id == newCustomer.Id);
            oldCustomer = newCustomer;
            RefreshList();
        }

        private async void DeleteImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ContentDialogResult result = await ConfirmDeleteAsync();
            if(result != ContentDialogResult.Primary)
            {
                return;
            }

            Loader loader = new Loader("Por favor espere...");
            loader.Show();
            Customer customer = Customers[CustomersListView.SelectedIndex];
            Response response = await ApiService.DeleteAsync("Customers",customer.Id, MainPage.GetInstance().TokenResponse.Token);
            loader.Close();
            if (!response.IsSuccess)
            {
                MessageDialog dialogMessage = new MessageDialog(response.Message, "Error");
                await dialogMessage.ShowAsync();
                return;
            }
            List<Customer> customers = Customers.Where(c => c.Id != customer.Id).ToList();
            Customers = new ObservableCollection<Customer>(customers);
            RefreshList();
        }

        private async Task<ContentDialogResult> ConfirmDeleteAsync()
        {
            ContentDialog confirmDialog = new ContentDialog
            {
                Title = "Confimación",
                Content = "¿Está seguro de querer borrar el registro?",
                PrimaryButtonText = "Si",
                CloseButtonText = "No"
            };

            return await confirmDialog.ShowAsync();
        }
    }
}
