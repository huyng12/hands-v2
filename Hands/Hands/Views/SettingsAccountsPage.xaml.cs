using System;
using System.Collections.Generic;
using ReactiveUI.XamForms;
using Xamarin.Forms;
using Hands.ViewModels;

namespace Hands.Views
{
    public partial class SettingsAccountsPage : ReactiveContentPage<SettingsAccountsViewModel>
    {
        public SettingsAccountsPage()
        {
            InitializeComponent();

            ViewModel = new SettingsAccountsViewModel();

            BindingContext = ViewModel;
        }
    }
}
