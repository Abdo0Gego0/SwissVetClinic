using CmsDataAccess;
using CmsDataAccess.DbModels;

namespace ServicesLibrary.Services.AppointmentSRVC
{
    public abstract class BaseService
    {
        public virtual ApplicationDbContext GetContext()
        {
            return new ApplicationDbContext();
        }
    }
}
