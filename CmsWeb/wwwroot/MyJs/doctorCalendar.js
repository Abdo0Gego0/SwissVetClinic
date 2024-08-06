/****************************************************************/
/********************* Employee Calendar *****************************/
/****************************************************************/



$(document).ready(function () {
    get_clinic_appointment();
});



var connectionNewApplication = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
connectionNewApplication.on("RefreshClinicStatus_", function (group) {

    
    get_clinic_appointment();
});


connectionNewApplication.start().then(function () {
    //document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

function get_clinic_appointment() {

    $.ajax({
        url: '/Auth/WebNotification/GetClinicAppointment', 
        method: 'GET',  
        success: function (data) {

            if (data.Count == 1) {
                //alert(data.Appointment.Start)
                $("#NoVisits").hide();
                $("#AppDetails").show();
                $("#appDate").text( data.Appointment.Start);
                $("#appPat").text(data.Appointment.PatientName);
                $("#appId").val(data.Appointment.Id);
            }
            else {
                $("#NoVisits").show();
                $("#AppDetails").hide();
            }
        },
        error: function () {
            // Handle error if needed
        }
    });


}



/****************************************************************/
/********************* End Employee Calendar *************************/
/****************************************************************/

