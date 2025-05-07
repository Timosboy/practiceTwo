using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatientManager.Models;

namespace PatientManager.Managers
{
    public interface IPatientManager
    {
        IEnumerable<Patient> GetAll();
        Patient GetByCi(string ci);
        void Create(Patient patient);
        bool Update(string ci, string name, string lastName);
        bool Delete(string ci);
    }
}
