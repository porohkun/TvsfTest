using CsvHelper;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TvsfTest.Models;
using TvsfTest.Services;

namespace TvsfTest
{
    public class MainWindowViewModelDummy : MainWindowViewModel
    {
        public MainWindowViewModelDummy()
        {
            Organizations.Add(new OrganizationViewModel(new OrganizationModel() { Id = 1, INN = 5553535, Name = "ООО Пальто", LegalAddress = "Не дом и не улица", PhysicalAddress = "Деревня дедушки", Description = "Не звонить" }, null));
            Organizations.Add(new OrganizationViewModel(new OrganizationModel() { Id = 2, INN = 5553535, Name = "ООО Пальто", LegalAddress = "Не дом и не улица", PhysicalAddress = "Деревня дедушки", Description = "Не звонить" }, null));
            Employees.Add(new EmployeeViewModel(new EmployeeModel() { Id = 1, OrganizationId = 1, Surname = "Боширов", Name = "Руслан", MiddleName = "Тимурович", BirthDate = DateTime.Now, PassportNumber = 1122333444, Description = "Любит шпили" }, null));
            Employees.Add(new EmployeeViewModel(new EmployeeModel() { Id = 12, OrganizationId = 1, Surname = "Петров", Name = "Александр", MiddleName = "Евгеньевич", BirthDate = DateTime.Now, PassportNumber = 1122444333, Description = "Тоже любит" }, null));
        }
    }

    public class MainWindowViewModel : BindableBase
    {
        public ObservableCollection<OrganizationViewModel> Organizations { get; } = new ObservableCollection<OrganizationViewModel>();
        public ObservableCollection<EmployeeViewModel> Employees { get; } = new ObservableCollection<EmployeeViewModel>();

        private OrganizationViewModel _selectedOrganization;
        public OrganizationViewModel SelectedOrganization
        {
            get => _selectedOrganization;
            set { if (SetProperty(ref _selectedOrganization, value)) RefreshEmployees(value); }
        }

        private bool _busy;
        public bool Busy
        {
            get => _busy;
            set => SetProperty(ref _busy, value);
        }

        public ICommand RefreshOrganizationsCommand { get; }
        public ICommand ImportFromCsvCommand { get; }
        public ICommand ExportToCsvCommand { get; }

        public MainWindowViewModel()
        {
            RefreshOrganizationsCommand = new DelegateCommand(RefreshOrganizations);
            ImportFromCsvCommand = new DelegateCommand(ImportFromCsv);
            ExportToCsvCommand = new DelegateCommand(ExportToCsv);
        }

        private void WrapAsyncAction<T>(Task<T> getTask, Action<T> resultAction)
        {
            Busy = true;
            getTask.ContinueWith(task =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        resultAction?.Invoke(task.Result);
                        Busy = false;
                    });
                else
                {
                    Busy = false;
                    if (task.Exception != null)
                        MessageBox.Show(task.Exception.InnerException.Message);
                }
            });
        }

        private void RefreshOrganizations()
        {
            WrapAsyncAction(DatabaseService.GetOrganizations(),
                result =>
                {
                    Organizations.Clear();
                    foreach (var org in result.Item2)
                        Organizations.Add(new OrganizationViewModel(org, result.Item1));
                });
        }

        private void RefreshEmployees(OrganizationViewModel organization)
        {
            if (organization == null)
                App.Current.Dispatcher.Invoke(Employees.Clear);
            else
                WrapAsyncAction(organization.GetEmployees(),
                    result =>
                    {
                        Employees.Clear();
                        foreach (var empl in result.Item2)
                            Employees.Add(new EmployeeViewModel(empl, result.Item1));
                    });
        }

        private void ImportFromCsv()
        {
            var dialog = new OpenFileDialog()
            {
                Filter = "csv file|*.csv",
                DefaultExt = "csv"
            };
            if (dialog.ShowDialog().Value)
            {
                async Task<object> ImportAndMerge(string filename, OrganizationViewModel organization)
                {
                    using (var reader = new StreamReader(filename))
                    using (var csv = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture) { MissingFieldFound = null }))
                    {
                        csv.Context.RegisterClassMap<EmployeeMap>();

                        await DatabaseService.MergeEmployeesIntoOrganization(csv.GetRecords<EmployeeModel>(), organization);
                    }
                    return null;
                }

                WrapAsyncAction(ImportAndMerge(dialog.FileName, SelectedOrganization), result => RefreshEmployees(SelectedOrganization));
            }
        }

        private void ExportToCsv()
        {
            var dialog = new SaveFileDialog()
            {
                Filter = "csv file|*.csv",
                DefaultExt = "csv"
            };
            if (dialog.ShowDialog().Value)
            {
                async Task<object> Export(string filename, IEnumerable<EmployeeViewModel> employees)
                {
                    using (var writer = new StreamWriter(filename))
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        csv.WriteRecords(employees);
                    }
                    Process.Start("explorer.exe", $"/select, \"{filename}\"");
                    return null;
                }

                WrapAsyncAction(Export(dialog.FileName, Employees), result => RefreshEmployees(SelectedOrganization));
            }
        }
    }
}
