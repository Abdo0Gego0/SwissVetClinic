/****************************************************************/
/********************* Doctor Calendar *****************************/
/****************************************************************/


$(document).ready(function () {
    get_status_of_clinics();
});



var connectionNewApplication = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
connectionNewApplication.on("RefreshClinicStatus_", function (group) {


    get_status_of_clinics();
    
});


connectionNewApplication.start().then(function () {
    //document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

function get_status_of_clinics() {

    $.ajax({
        url: '/Auth/WebNotification/GetStatusOfClinic', 
        method: 'GET',  
        success: function (data) {

            for (let i = 0; i < data.length; i++) {
                if (data[i].Status == 1) {

                    $("#" + data[i].Id).removeClass("classFree");
                    $("#" + data[i].Id).addClass("classBusy");
                    $("#" + data[i].Id +" clinicSpan").show();
                }
                else {
                    $("#" + data[i].Id).removeClass("classBusy");
                    $("#" + data[i].Id).addClass("classFree");
                    $("#" + data[i].Id + " clinicSpan").hide();
                }

            }
            try {
                clinicFilter();
            }
            catch (err) {

            }

        },
        error: function () {
            // Handle error if needed
        }
    });


}
function clinicFilter() {
    var selectedClinic = $("#clinicFilter").val();
    var schedulerDataSource = $("#scheduler").data("kendoScheduler").dataSource;

    if (selectedClinic == "") {
        var scheduler = $("#scheduler").data("kendoScheduler");
        scheduler.options.group.resources = ['BaseClinicId'];
        scheduler.view(scheduler.view().name);
    }
    else {
        var scheduler = $("#scheduler").data("kendoScheduler");
        scheduler.options.group.resources = [];
        scheduler.view(scheduler.view().name);
    }

    schedulerDataSource.transport.options.read.data = { ClinicId: selectedClinic };
    schedulerDataSource.read();
}


/****************************************************************/
/********************* End Doctor Calendar *************************/
/****************************************************************/

