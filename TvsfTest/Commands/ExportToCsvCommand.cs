using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TvsfTest.Commands
{
    public class ExportToCsvCommand : InjectableCommand<ExportToCsvCommand, IEnumerable<EmployeeViewModel>>
    {
        protected override bool CanExecuteInternal(IEnumerable<EmployeeViewModel> parameter)
        {
            return true;
        }

        protected override void ExecuteInternal(IEnumerable<EmployeeViewModel> parameter)
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "csv file|*.csv";
            dialog.DefaultExt = "csv";
            if (dialog.ShowDialog().Value)
            {
                Task.Run(() =>
                {
                    try
                    {
                        fastCSV.WriteFile(dialog.FileName,
                            new[] { "Id", "OrganizationId", "Surname", "Name", "MiddleName", "BirthDate", "PassportNumber", "Description" },
                            ',',
                            parameter.ToList(),
                            (o, c) =>
                            {
                                c.Add(o.Id);
                                c.Add(o.OrganizationId);
                                c.Add(o.Surname);
                                c.Add(o.Name);
                                c.Add(o.MiddleName);
                                c.Add(o.BirthDate);
                                c.Add(o.PassportNumber);
                                c.Add(o.Description);
                            });
                        Process.Start("explorer.exe", $"/select, \"{dialog.FileName}\"");
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                });
            }
        }
    }
}
