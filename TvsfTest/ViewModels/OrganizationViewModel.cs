using System.Collections.Generic;
using System.Data.Linq;
using System.Threading.Tasks;
using TvsfTest.Models;
using TvsfTest.Services;

namespace TvsfTest
{
    public class OrganizationViewModel : BindableBase
    {
        private OrganizationModel _model;
        private DataContext _context;

        public int Id => _model.Id;
        public string Name
        {
            get => _model.Name;
            set => SetProperty(ref _model.Name, value);
        }
        public long INN
        {
            get => _model.INN;
            set => SetProperty(ref _model.INN, value);
        }
        public string LegalAddress
        {
            get => _model.LegalAddress;
            set => SetProperty(ref _model.LegalAddress, value);
        }
        public string PhysicalAddress
        {
            get => _model.PhysicalAddress;
            set => SetProperty(ref _model.PhysicalAddress, value);
        }
        public string Description
        {
            get => _model.Description;
            set => SetProperty(ref _model.Description, value);
        }

        public OrganizationViewModel(OrganizationModel model, DataContext context)
        {
            _model = model;
            _context = context;
        }

        public async Task<(DataContext, IEnumerable<EmployeeModel>)> GetEmployees()
        {
            return await DatabaseService.GetEmployees(_model);
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            Task.Run(_context.SubmitChanges);
        }
    }
}