using System;
using System.Collections.Generic;
using ReactiveUI.XamForms;
using Xamarin.Forms;
using Hands.ViewModels;
using Hands.Models;

namespace Hands.Views
{
    public partial class EntryDetailPage : ReactiveContentPage<EntryDetailViewModel>, ICancelableModalPage
    {
        public EntryDetailPage(TransactionWithAccountWithCategory transaction = null)
        {
            InitializeComponent();

            ViewModel = new EntryDetailViewModel(transaction);

            BindingContext = ViewModel;

            this.SetupNumericKeyboard();
        }

        public void OnCancel() => Navigation.PopModalAsync();

        #region Custom Numeric Keyboard
        private void SetupNumericKeyboard()
        {
            Grid container = new Grid
            {
                RowDefinitions = {
                    new RowDefinition{ Height = GridLength.Auto },
                    new RowDefinition{ Height = GridLength.Auto },
                    new RowDefinition{ Height = GridLength.Auto },
                    new RowDefinition{ Height = GridLength.Auto }, },
                ColumnDefinitions = {
                    new ColumnDefinition{ Width = GridLength.Star },
                    new ColumnDefinition{ Width = GridLength.Star },
                    new ColumnDefinition{ Width = GridLength.Star }, },
            };

            string[,] shape = new string[4, 3]
            {
                { "7",   "8", "9"},
                { "4",   "5", "6"},
                { "1",   "2", "3"},
                { "000", "0", " "},
            };

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (String.IsNullOrWhiteSpace(shape[i, j])) continue;
                    container.Children.Add(this.createKeyboardButton(shape[i, j]), j, i);
                }
            }

            // Add backspace icon separately
            container.Children.Add(this.createKeyboardButton("", true), 2, 3);

            // Mount keyboard to screen
            this.NumericKeyboard.Children.Add(container);
        }

        private Button createKeyboardButton(string text, bool isTextIcon = false)
        {
            var button = new Button
            {
                Text = text,
                FontSize = 32,
                HeightRequest = 80,
                TextColor = Color.DimGray,
                BackgroundColor = Color.White,
                FontFamily = isTextIcon ? "FAProRegular" : default,
            };
            button.Clicked += OnNumericKeyboardButtonClicked;
            return button;
        }

        public void OnNumericKeyboardButtonClicked(object sender, EventArgs e)
        {
            var text = (sender as Button).Text;
            int num;
            if (!int.TryParse(text, out num)) num = -1;
            if (num == 0 && text == "000") num = 1000;
            this.ViewModel.OnNumericKeyboardButtonClicked(num);
        }
        #endregion
    }
}
