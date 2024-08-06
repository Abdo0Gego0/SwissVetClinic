function sync_handler() {
    this.read();

}

function requestEnd(e) {
    if (e.type == "create") {
        if (!e.response.Data)
            // alert(JSON.stringify(e.response));
            return;
    }
    if (e.type == "update") {
        if (!e.response.Data)
            // alert(JSON.stringify(e.response));
            return;
    }
}



function requestStart(e) {
    if (e.type == "create") {
    }
}

function onEdit(e) {

}

function returnFalse() {
    return false;
}

function onDataBound(e) {
    var grid = this;
    grid.table.find("tr").each(function () {
        var dataItem = grid.dataItem(this);

        $(this).find('script').each(function () {
            eval($(this).html());
        });

        kendo.bind($(this), dataItem);
    });



}


/*************** SubscriptionPlan ***************************/

function goEditSubscriptionPlan(e) {
    dataItemPay = this.dataItem($(e.currentTarget).closest("tr"));
    transferId = dataItemPay.Id;
    var url = "/Admin/SubscriptionPlan/EditPlan/" + transferId;
    var a = document.createElement('a');
    a.href = url;
    a.click();
}
/*************** End Of SubscriptionPlan *******************/






/*************** SubscriptionApplication ***************************/

function goEditSubscriptionApplication(e) {
    dataItemPay = this.dataItem($(e.currentTarget).closest("tr"));
    transferId = dataItemPay.Id;
    var url = "/Admin/NewSubscription/Edit/" + transferId;
    var a = document.createElement('a');
    a.href = url;
    a.click();
}

/*************** End Of SubscriptionApplication *******************/


/*************** Clinic ***************************/

function goEditClinic(e) {
    dataItemPay = this.dataItem($(e.currentTarget).closest("tr"));
    transferId = dataItemPay.Id;
    var url = "/CenterAdmin/Clinic/Edit_Clinic/" + transferId;
    var a = document.createElement('a');
    a.href = url;
    a.click();
}

function goEditClinicBySysAdmin(e) {
    dataItemPay = this.dataItem($(e.currentTarget).closest("tr"));
    transferId = dataItemPay.Id;
    var url = "/Admin/Clinic/Edit_Clinic/" + transferId;
    var a = document.createElement('a');
    a.href = url;
    a.click();
}



/*************** End Of Clinic *******************/


/*************** Doctor ***************************/

function goEditDoctor(e) {
    dataItemPay = this.dataItem($(e.currentTarget).closest("tr"));
    transferId = dataItemPay.Id;
    var url = "/CenterAdmin/Doctor/Edit_Doctor/" + transferId;
    var a = document.createElement('a');
    a.href = url;
    a.click();
}


function goEditDoctorBySysAdmin(e) {
    dataItemPay = this.dataItem($(e.currentTarget).closest("tr"));
    transferId = dataItemPay.Id;
    var url = "/Admin/Doctor/Edit_Doctor/" + transferId;
    var a = document.createElement('a');
    a.href = url;
    a.click();
}


/*************** End Of Doctor *******************/



/*************** Employee ***************************/

function goEditEmployee(e) {
    dataItemPay = this.dataItem($(e.currentTarget).closest("tr"));
    transferId = dataItemPay.Id;
    var url = "/CenterAdmin/Employee/Edit_Employee/" + transferId;
    var a = document.createElement('a');
    a.href = url;
    a.click();
}
function goEditEmployeeBySysAdmin(e) {
    dataItemPay = this.dataItem($(e.currentTarget).closest("tr"));
    transferId = dataItemPay.Id;
    var url = "/Admin/Employee/Edit_Employee/" + transferId;
    var a = document.createElement('a');
    a.href = url;
    a.click();
}

/*************** End Of Employee *******************/


/*************** Certificate ***************************/



/*************** End Of Certificate *******************/



/*************** Medical Center ***************************/
function goEditCenter(e) {
    dataItemPay = this.dataItem($(e.currentTarget).closest("tr"));
    transferId = dataItemPay.Id;
    var url = "/Admin/AllCenter/EditCenter/" + transferId;
    var a = document.createElement('a');
    a.href = url;
    a.click();
}

/*************** End Of Medical Center *******************/


/*************** Medical Center Admin ***************************/
function goEditCenterAdmin(e) {
    dataItemPay = this.dataItem($(e.currentTarget).closest("tr"));
    transferId = dataItemPay.Id;
    var url = "/Admin/AllCenter/EditCenterAdmin/" + transferId;
    var a = document.createElement('a');
    a.href = url;
    a.click();
}

/*************** End Of Medical Center Admin *******************/


/*************** Therapy Plan ***************************/
function goEditPlan(e) {
    dataItemPay = this.dataItem($(e.currentTarget).closest("tr"));
    transferId = dataItemPay.Id;
    var url = "/CenterAdmin/TherapyPlan/Edit_Plan/" + transferId;
    var a = document.createElement('a');
    a.href = url;
    a.click();
}

/*************** End Of Therapy Plan *******************/

/*************** Patient ***************************/
function goEditPatient(e) {
    dataItemPay = this.dataItem($(e.currentTarget).closest("tr"));
    transferId = dataItemPay.Id;
    var url = "/CenterAdmin/Patient/Edit_Patient/" + transferId;
    var a = document.createElement('a');
    a.href = url;
    a.click();
}


function goEditPatientUsingDoctor(e) {
    dataItemPay = this.dataItem($(e.currentTarget).closest("tr"));
    transferId = dataItemPay.Id;
    var url = "/Doctor/Patient/Edit_Patient/" + transferId;
    var a = document.createElement('a');
    a.href = url;
    a.click();
}





function goEditPatientUsingEmployee(e) {
    dataItemPay = this.dataItem($(e.currentTarget).closest("tr"));
    transferId = dataItemPay.Id;
    var url = "/CenterEmployeeReception/Patient/Edit_Patient/" + transferId;
    var a = document.createElement('a');
    a.href = url;
    a.click();
}




function goEditPatientBySysAdmin(e) {
    dataItemPay = this.dataItem($(e.currentTarget).closest("tr"));
    transferId = dataItemPay.Id;
    var url = "/Admin/Patient/Edit_Patient/" + transferId;
    var a = document.createElement('a');
    a.href = url;
    a.click();
}


/*************** End Of Patient *******************/






function openPreviousVisitAdmin(e) {
    dataItemPay = this.dataItem($(e.currentTarget).closest("tr"));
    transferId = dataItemPay.Id;
    var url = "/CenterAdmin/Appointment/Open_Visit/" + transferId;
    var a = document.createElement('a');
    a.href = url;
    a.target = '_blank';
    a.click();
}


function openPreviousVisitDoctor(e) {
    dataItemPay = this.dataItem($(e.currentTarget).closest("tr"));
    transferId = dataItemPay.Id;
    var url = "/Doctor/Appointment/Open_Visit/" + transferId;
    var a = document.createElement('a');
    a.href = url;
    a.target = '_blank';
    a.click();
}

function openPreviousVisitEmployee(e) {
    dataItemPay = this.dataItem($(e.currentTarget).closest("tr"));
    transferId = dataItemPay.Id;
    var url = "/CenterEmployeeReception/Appointment/Open_Visit/" + transferId;
    var a = document.createElement('a');
    a.href = url;
    a.target = '_blank';
    a.click();
}



