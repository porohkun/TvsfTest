using CsvHelper;
using Microsoft.Win32;
using System;
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

        private void RefreshOrganizations()
        {
            Busy = true;
            DatabaseService.GetOrganizations().ContinueWith(task =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        Organizations.Clear();
                        foreach (var org in task.Result.Item2)
                            Organizations.Add(new OrganizationViewModel(org, task.Result.Item1));
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

        private void RefreshEmployees(OrganizationViewModel organization)
        {
            Busy = true;
            if (organization == null)
            {
                App.Current.Dispatcher.Invoke(Employees.Clear);
                Busy = false;
            }
            else
                organization.GetEmployees().ContinueWith(task =>
                {
                    if (task.Status == TaskStatus.RanToCompletion)
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            Employees.Clear();
                            foreach (var empl in task.Result.Item2)
                                Employees.Add(new EmployeeViewModel(empl, task.Result.Item1));
                            Busy = false;
                        });
                    else
                        Busy = false;
                });
        }

        private void ImportFromCsv()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "csv file|*.csv";
            dialog.DefaultExt = "csv";
            if (dialog.ShowDialog().Value)
            {
                using (var reader = new StreamReader(dialog.FileName))
                using (var csv = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture) { MissingFieldFound = null }))
                {
                    csv.Context.RegisterClassMap<EmployeeMap>();

                    DatabaseService.BulkCopy(csv.GetRecords<EmployeeModel>(), SelectedOrganization).ContinueWith(task =>
                    {
                        RefreshEmployees(SelectedOrganization);
                    });
                }
            }
        }

        private void ExportToCsv()
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "csv file|*.csv";
            dialog.DefaultExt = "csv";
            if (dialog.ShowDialog().Value)
            {
                Task.Run(() =>
                {
                    using (var writer = new StreamWriter(dialog.FileName))
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        csv.WriteRecords(Employees);
                    }
                    Process.Start("explorer.exe", $"/select, \"{dialog.FileName}\"");
                });
            }
        }
    }
}
