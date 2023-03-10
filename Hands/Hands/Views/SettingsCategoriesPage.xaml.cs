using System;
using System.Collections.Generic;
using ReactiveUI.XamForms;
using Xamarin.Forms;
using Hands.ViewModels;

namespace Hands.Views
{
    public partial class SettingsCategoriesPage : ReactiveContentPage<SettingsCategoriesViewModel>
    {
        public SettingsCategoriesPage()
        {
            InitializeComponent();

            ViewModel = new SettingsCategoriesViewModel();

            BindingContext = ViewModel;
        }
    }
}
