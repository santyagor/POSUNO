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
    public sealed partial class ProductsPage : Page
    {
        public ObservableCollection<Product> Products { get; set; }
        public ProductsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadProductssAsync();
        }

        private async void LoadProductssAsync()
        {
            Loader loader = new Loader("Por favor espere...");
            loader.Show();
            Response response = await ApiService.GetListAsync<Product>("products");
            loader.Close();
            if (!response.IsSuccess)
            {
                MessageDialog dialog = new MessageDialog(response.Message, "Error");
                await dialog.ShowAsync();
                return;
            }
            List<Product> products = (List<Product>)response.Result;
            Products = new ObservableCollection<Product>(products);
            RefreshList();
        }

        private void RefreshList()
        {
            ProductsListView.ItemsSource = null;
            ProductsListView.Items.Clear();
            ProductsListView.ItemsSource = Products;
        }


        private async void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            Product product = new Product();
            ProductDialog dialog = new ProductDialog(product);
            await dialog.ShowAsync();
            if (!product.WasSaved)
            {
                return;
            }

            Loader loader = new Loader("Por favor espere...");
            loader.Show();
            Response response = await ApiService.PostAsync("Products", product);

            loader.Close();

            if (!response.IsSuccess)
            {
                MessageDialog dialogMessage = new MessageDialog(response.Message, "Error");
                await dialogMessage.ShowAsync();
                return;
            }

            Product newProduct = (Product)response.Result;
            Products.Add(newProduct);
            RefreshList();
        }

        private async void EditImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Product product = Products[ProductsListView.SelectedIndex];
            product.IsEdit = true;
            ProductDialog dialog = new ProductDialog(product);
            await dialog.ShowAsync();

            if (!product.WasSaved)
            {
                return;
            }

            Loader loader = new Loader("Por favor espere...");
            loader.Show();
            Response response = await ApiService.PutAsync("Products", product, product.Id);

            loader.Close();

            if (!response.IsSuccess)
            {
                MessageDialog dialogMessage = new MessageDialog(response.Message, "Error");
                await dialogMessage.ShowAsync();
                return;
            }

            Product newProduct = (Product)response.Result;
            Product oldProduct = Products.FirstOrDefault(c => c.Id == newProduct.Id);
            oldProduct = newProduct;
            RefreshList();
        }

        private async void DeleteImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ContentDialogResult result = await ConfirmDeleteAsync();
            if (result != ContentDialogResult.Primary)
            {
                return;
            }

            Loader loader = new Loader("Por favor espere...");
            loader.Show();
            Product product = Products[ProductsListView.SelectedIndex];
            Response response = await ApiService.DeleteAsync("Products", product.Id);
            loader.Close();
            if (!response.IsSuccess)
            {
                MessageDialog dialogMessage = new MessageDialog(response.Message, "Error");
                await dialogMessage.ShowAsync();
                return;
            }
            List<Product> products = Products.Where(c => c.Id != product.Id).ToList();
            Products = new ObservableCollection<Product>(products);
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
