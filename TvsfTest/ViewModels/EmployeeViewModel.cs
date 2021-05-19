using System;
using TvsfTest.Models;

namespace TvsfTest
{
    public class EmployeeViewModel : BindableBase
    {
        private EmployeeModel _model;

        public int Id
        {
            get => _model.Id;
            set => SetProperty(ref _model.Id, value);
        }
        public int OrganizationId
        {
            get => _model.OrganizationId;
            set => SetProperty(ref _model.OrganizationId, value);
        }
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

        public EmployeeViewModel(EmployeeModel model)
        {
            _model = model;
        }
    }
}