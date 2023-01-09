using System.Reactive.Disposables;
using ReactiveUI;
using ReactiveUI.XamForms;
using Hands.ViewModels;
using Hands.Models;
using System;

namespace Hands.Views
{
    public partial class SettingsPage : ReactiveContentPage<SettingsViewModel>
    {
        public SettingsPage()
        {
            InitializeComponent();
            ViewModel = new SettingsViewModel(Navigation);
            BindingContext = ViewModel;
        }
    }
}
