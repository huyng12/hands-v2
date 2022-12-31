using System;
using System.Collections.Generic;
using ReactiveUI.XamForms;
using Hands.ViewModels;

namespace Hands.Views
{
    public partial class EntryPage : ReactiveContentPage<EntryViewModel>
    {
        public EntryPage()
        {
            InitializeComponent();

            ViewModel = new EntryViewModel();
        }
    }
}
