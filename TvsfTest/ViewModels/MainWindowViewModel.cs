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
            Organizations.Add(new OrganizationViewModel(new Models.OrganizationModel() { Id = 1, INN = 5553535, Name = "ООО Пальто", LegalAddress = "Не дом и не улица", PhysicalAddress = "Деревня дедушки", Description = "Не звонить" }));
            Organizations.Add(new OrganizationViewModel(new Models.OrganizationModel() { Id = 2, INN = 5553535, Name = "ООО Пальто", LegalAddress = "Не дом и не улица", PhysicalAddress = "Деревня дедушки", Description = "Не звонить" }));
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
                            foreach (var org in task.Result)
                                Organizations.Add(new OrganizationViewModel(org));
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
            organization.GetEmployees().ContinueWith(task =>
            {
                if (task.IsCompleted)
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        Employees.Clear();
                        foreach (var empl in task.Result)
                            Employees.Add(new EmployeeViewModel(empl));
                    });
            });
        }
    }
}
