/****************************************************************/
/********************* Admin NavBar *****************************/
/****************************************************************/

get_number_of_new_application();

var connectionNewApplication = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
connectionNewApplication.on("RefreshNewApplicationCount_", function (group) {
    get_number_of_new_application();
});


connectionNewApplication.start().then(function () {
    //document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

function get_number_of_new_application() {

    $.ajax({
        url: '/Auth/WebNotification/GetNewRequestCount',  // Replace with your controller and action
        method: 'GET',  // or 'POST' depending on your server-side logic
        success: function (data) {

            if (data == "0") {
                $("#newRequstsSpan").hide();

            }
            else {

                $("#newRequstsSpan").show();
                $('#newRequstsSpan').text(data);
            }


        },
        error: function () {
            // Handle error if needed
        }
    });


}



/****************************************************************/
/********************* End Admin NavBar *************************/
/****************************************************************/




/****************************************************************/
/********************* Center Admin NavBar *****************************/
/****************************************************************/

get_number_of_new_orders();

get_number_of_new_bills();

connectionNewApplication.on("RefreshNewOrderCount_", function (group) {
    get_number_of_new_orders();
});

connectionNewApplication.on("RefreshBillStatus_", function (group) {
    get_number_of_new_bills();
});



function get_number_of_new_orders() {

    $.ajax({
        url: '/Auth/WebNotification/GetNewOrdersCount',  // Replace with your controller and action
        method: 'GET',  // or 'POST' depending on your server-side logic
        success: function (data) {

            if (data == "0") {
                $("#newOrdersSpan").hide();

            }
            else {

                $("#newOrdersSpan").show();
                $('#newOrdersSpan').text(data);
            }


        },
        error: function () {
            // Handle error if needed
        }
    });


}


function get_number_of_new_bills() {

    $.ajax({
        url: '/Auth/WebNotification/GetStatusOfBills',  // Replace with your controller and action
        method: 'GET',  // or 'POST' depending on your server-side logic
        success: function (data) {

            if (data == "0") {
                $("#newBillsSpan").hide();


            }
            else {

                $("#newBillsSpan").show();
                $('#newBillsSpan').text(data);

                $("#newBillsSpan1").show();
                $('#newBillsSpan1').text(data);

            }


        },
        error: function () {
            // Handle error if needed
        }
    });


}

$(document).ready(function () {

    get_number_of_new_bills();
});



/****************************************************************/
/********************* End Center Admin NavBar *************************/
/****************************************************************/


