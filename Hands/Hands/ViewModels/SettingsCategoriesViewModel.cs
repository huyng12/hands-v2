using Splat;
using ReactiveUI;
using DynamicData;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Hands.Services;
using Hands.Models;
using System.Threading.Tasks;
using DynamicData.Binding;

namespace Hands.ViewModels
{
    public class SettingsCategoriesViewModel : ReactiveObject, IDisposable
    {
        private readonly ISettingsService service;

        public SettingsCategoriesViewModel()
        {
            service = Locator.Current.GetService<ISettingsService>();

            SelectedCategoryType = "Expense";

            var typeFilter = this
                .WhenAnyValue(vm => vm.SelectedCategoryType)
                .Select(CreateFilterByType);

            disposable = service.ConnectCategoriesSetting()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Filter(typeFilter)
                .Sort(SortExpressionComparer<TCategory>.Ascending(v => v.Name))
                .DistinctUntilChanged()
                .Bind(out items)
                .Subscribe();

            AddCommand = ReactiveCommand.CreateFromTask(ExecuteAddCommand);
            EditCommand = ReactiveCommand.CreateFromTask<TCategory>(ExecuteEditCommand);
            DeleteCommand = ReactiveCommand.CreateFromTask<TCategory>(ExecuteRemoveCommand);
        }

        private readonly ReadOnlyObservableCollection<TCategory> items;
        public ReadOnlyObservableCollection<TCategory> Items => items;

        private string selectedCategoryType;
        public string SelectedCategoryType
        {
            get => selectedCategoryType;
            set => this.RaiseAndSetIfChanged(ref selectedCategoryType, value);
        }

        public ReactiveCommand<Unit, Unit> AddCommand { get; set; }

        public ReactiveCommand<TCategory, Unit> EditCommand { get; set; }

        public ReactiveCommand<TCategory, Unit> DeleteCommand { get; set; }

        private async Task ExecuteAddCommand()
        {
            var name = await App.Current.MainPage.DisplayPromptAsync(
                $"New Category in {SelectedCategoryType}",
                "Please input a name to create a new category",
                "Create", "Cancel", "Name");
            if (String.IsNullOrEmpty(name)) return;
            service.AddNewCategory(name, SelectedCategoryType);
        }

        private async Task ExecuteRemoveCommand(TCategory category)
        {
            bool answer = await App.Current.MainPage.DisplayAlert(
                $"Delete \"{category.Name}\"?",
                "Transactions under this category will remain.",
                "Delete", "Cancel");
            if (!answer) return;
            service.RemoveCategory(category);
        }

        private async Task ExecuteEditCommand(TCategory category)
        {
            var name = await App.Current.MainPage.DisplayPromptAsync(
                $"Editing \"{category.Name}\"",
                "Please input a new name for this category",
                "Update", "Cancel", "", -1, null, category.Name);
            if (String.IsNullOrEmpty(name)) return;
            service.UpdateCategory(new TCategory { Id = category.Id, Type = category.Type, Name = name });
        }

        private Func<TCategory, bool> CreateFilterByType(string type)
        {
            if (String.IsNullOrEmpty(type)) return category => true;
            return category => category.Type == type;
        }

        private readonly IDisposable disposable;
        public void Dispose() { disposable.Dispose(); }
    }
}
