using System;
using System.Data.Linq;
using System.Threading.Tasks;
using TvsfTest.Models;

namespace TvsfTest
{
    public class EmployeeViewModel : BindableBase
    {
        private EmployeeModel _model;
        private DataContext _context;

        public int Id => _model.Id;
        public int OrganizationId => _model.OrganizationId;
        public string Surname
        {
            get => _model.Surname;
            set => SetProperty(ref _model.Surname, value);
        }
        public string Name
        {
            get => _model.Name;
            set => SetProperty(ref _model.Name, value);
        }
        public string MiddleName
        {
            get => _model.MiddleName;
            set => SetProperty(ref _model.MiddleName, value);
        }
        public DateTime BirthDate
        {
            get => _model.BirthDate;
            set => SetProperty(ref _model.BirthDate, value);
        }
        public long PassportNumber
        {
            get => _model.PassportNumber;
            set => SetProperty(ref _model.PassportNumber, value);
        }
        public string Description
        {
            get => _model.Description;
            set => SetProperty(ref _model.Description, value);
        }

        public EmployeeViewModel(EmployeeModel model, DataContext context)
        {
            _model = model;
            _context = context;
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            Task.Run(_context.SubmitChanges);
        }
    }
}