//using CmsDataAccess.DbModels;
//using ServicesLibrary.BlazorAppointments;
//using Syncfusion.Blazor;
//using Syncfusion.Blazor.Data;

//namespace CmsWeb.SyncFusionLib
//{
//    public class CenterAppointmentDataAdapter:DataAdaptor
//    {
//        private readonly AppointmentDataService appointmentDataService;

//        public CenterAppointmentDataAdapter(AppointmentDataService appointmentDataService_)
//        {
//            appointmentDataService = appointmentDataService_;
//        }

//        public override async Task<object> ReadAsync(DataManagerRequest dataManagerRequest,string key=null)
//        {
//            IDictionary<string, object> Parms = dataManagerRequest.Params;
//            DateTime startDate = DateTime.Parse((string)Parms["StartDate"]);
//            DateTime endDate = DateTime.Parse((string)Parms["EndDate"]);
//            List<Appointment>? appointments = await appointmentDataService.Get(startDate, endDate);
//            return dataManagerRequest.RequiresCounts?new DataResult()
//            {
//                Result= appointments,
//                Count= appointments.Count,
//            } : appointments;

//        }

//        public override async Task<object> InsertAsync(DataManager dataManager,object data,string key)
//        {
//            await appointmentDataService.Insert(data as Appointment);
//            return data;
//        }

//        public override async Task<object> UpdateAsync(DataManager dataManager, object data,string keyField, string key)
//        {
//            await appointmentDataService.Update(data as Appointment);
//            return data;
//        }

//        public override async Task<object> RemoveAsync(DataManager dataManager, object data, string keyField, string key)
//        {
//            await appointmentDataService.Delete((Guid) data);
//            return data;
//        }

//        public override async Task<object> BatchUpdateAsync(DataManager dataManager, object changedRecords
//            , object addedRecords
//            , object deletedRecords
//            , string keyField, string key, int? dropIndex)
//        {
//            object records = deletedRecords;
//            List<Appointment>? deletedData = deletedRecords as List<Appointment>;
//            if (deletedData !=null)
//            {
//                foreach (var item in deletedData)
//                {
//                    await appointmentDataService.Delete(item.Id);
//                }
//            }

//            List<Appointment>? addedData = addedRecords as List<Appointment>;
//            if (addedData != null)
//            {
//                foreach (var item in addedData)
//                {
//                    await appointmentDataService.Insert(item as Appointment);
//                    records = addedRecords;
//                }
//            }

//            List<Appointment>? changedData = changedRecords as List<Appointment>;
//            if (changedData != null)
//            {
//                foreach (var item in changedData)
//                {
//                    await appointmentDataService.Update(item as Appointment);
//                    records = changedRecords;
//                }
//            }

//            return records;

//        }
//    }
//}
