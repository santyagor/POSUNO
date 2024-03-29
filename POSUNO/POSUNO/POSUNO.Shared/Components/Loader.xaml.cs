﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace POSUNO.Components
{
    public sealed partial class Loader : UserControl
    {
        private Popup _popup;
        public Loader()
        {
            this.InitializeComponent();
        }

        public Loader(string message)
        {
            InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += UWPHUD_BackRequested;
            Window.Current.CoreWindow.SizeChanged += CoreWindow_SizeChanged;
            msg_Txt.Text = message;
        }

        private void CoreWindow_SizeChanged(CoreWindow sender, WindowSizeChangedEventArgs args)
        {
            UpdateUI();
        }

        private void UWPHUD_BackRequested(object sender, BackRequestedEventArgs e)
        {
            Close();
        }

        private void UpdateUI()
        {
            Rect bounds = Window.Current.Bounds;
            Width = bounds.Width;
            Height = bounds.Height;
        }

        public void Show()
        {
            try
            {
                _popup = new Popup
                {
                    Child = this
                };
                progress_R.IsActive = true;
                _popup.IsOpen = true;
                UpdateUI();
            }
            catch { }
        }

        public void Close()
        {
            if (_popup.IsOpen)
            {
                progress_R.IsActive = false;
                _popup.IsOpen = false;
                SystemNavigationManager.GetForCurrentView().BackRequested -= UWPHUD_BackRequested;
                Window.Current.CoreWindow.SizeChanged -= CoreWindow_SizeChanged;
            }
        }
    }
}
