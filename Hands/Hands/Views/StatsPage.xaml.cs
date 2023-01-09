using System;
using System.Collections.Generic;
using ReactiveUI.XamForms;
using Hands.ViewModels;
using Syncfusion.SfChart.XForms;

namespace Hands.Views
{
    public partial class StatsPage : ReactiveContentPage<StatsViewModel>
    {
        public StatsPage()
        {
            InitializeComponent();

            ViewModel = new StatsViewModel();

            BindingContext = ViewModel;
        }
    }
}
