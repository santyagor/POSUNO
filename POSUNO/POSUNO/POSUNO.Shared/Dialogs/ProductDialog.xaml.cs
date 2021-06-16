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

namespace POSUNO.Dialogs
{
	public sealed partial class ProductDialog : ContentDialog
	{
		public Product Product {get; set; }
		public ProductDialog(Product product)
		{
			this.InitializeComponent();
			Product = product;
			if (Product.IsEdit)
			{
				TitleTextBlock.Text = $"Editar el producto: {Product.Name}";
			}
			else
			{
				TitleTextBlock.Text = $"Nuevo producto";
			}
		}

		private void CloseImage_Tapped(object sender, TappedRoutedEventArgs e)
		{
			Product.WasSaved = false;
			Hide();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			Product.WasSaved = false;
			Hide();
		}
		private async void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			bool isValid = await ValidateFormAsync();
			if (!isValid)
			{
				return;
			}

			Product.WasSaved = true;

			Hide();
		}

		private async Task<bool> ValidateFormAsync()
		{
			MessageDialog messageDialog;
			if (string.IsNullOrEmpty(Product.Name))
			{
				messageDialog = new MessageDialog("Debes ingresar nombre del producto.", "Error");
				await messageDialog.ShowAsync();
				return false;
			}

			if (string.IsNullOrEmpty(Product.Description))
			{
				messageDialog = new MessageDialog("Debes ingresar descripción del producto.", "Error");
				await messageDialog.ShowAsync();
				return false;
			}

			if (string.IsNullOrEmpty(Product.Price.ToString()))
			{
				messageDialog = new MessageDialog("Debes ingresar precio del producto.", "Error");
				await messageDialog.ShowAsync();
				return false;
			}

			if (string.IsNullOrEmpty(Product.Stock.ToString()))
			{
				messageDialog = new MessageDialog("Debes ingresar stock del producto.", "Error");
				await messageDialog.ShowAsync();
				return false;
			}

			return true;
		}
}
}
