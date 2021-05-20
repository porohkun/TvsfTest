using CsvHelper;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using TvsfTest.Models;
using TvsfTest.Services;

namespace TvsfTest
{
    public class MainWindowViewModelDummy : MainWindowViewModel
    {
        public MainWindowViewModelDummy()
        {
            Organizations.Add(new OrganizationViewModel(new Models.OrganizationModel() { Id = 1, INN = 5553535, Name = "ООО Пальто", LegalAddress = "Не дом и не улица", PhysicalAddress = "Деревня дедушки", Description = "Не звонить" }, null));
            Organizations.Add(new OrganizationViewModel(new Models.OrganizationModel() { Id = 2, INN = 5553535, Name = "ООО Пальто", LegalAddress = "Не дом и не улица", PhysicalAddress = "Деревня дедушки", Description = "Не звонить" }, null));
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
            DatabaseService.GetOrganizations().ContinueWith(task =>
            {
                if (task.IsCompleted)
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        Organizations.Clear();
                        foreach (var org in task.Result.Item2)
                            Organizations.Add(new OrganizationViewModel(org, task.Result.Item1));
                    });
            });
        }

        private void RefreshEmployees(OrganizationViewModel organization)
        {
            if (organization == null)
                App.Current.Dispatcher.Invoke(Employees.Clear);
            else
                organization.GetEmployees().ContinueWith(task =>
                {
                    if (task.IsCompleted)
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            Employees.Clear();
                            foreach (var empl in task.Result.Item2)
                                Employees.Add(new EmployeeViewModel(empl, task.Result.Item1));
                        });
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
                using (var csv = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    MissingFieldFound = null
                }))
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
