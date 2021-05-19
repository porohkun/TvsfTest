using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

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

        public MainWindowViewModel()
        {
            RefreshOrganizationsCommand = new DelegateCommand(RefreshOrganizations);
        }

        private void RefreshOrganizations()
        {
            Services.DatabaseService.GetOrganizations().ContinueWith(task =>
            {
                try
                {
                    if (task.IsCompleted)
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            Organizations.Clear();
                            foreach (var org in task.Result.Item2)
                                Organizations.Add(new OrganizationViewModel(org, task.Result.Item1));
                        });
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
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
    }
}
