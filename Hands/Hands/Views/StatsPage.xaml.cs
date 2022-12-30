using System;
using System.Collections.Generic;
using ReactiveUI.XamForms;
using Hands.ViewModels;

namespace Hands.Views
{
    public partial class StatsPage : ReactiveContentPage<StatsViewModel>
    {
        public StatsPage()
        {
            InitializeComponent();

            ViewModel = new StatsViewModel();
        }
    }
}
