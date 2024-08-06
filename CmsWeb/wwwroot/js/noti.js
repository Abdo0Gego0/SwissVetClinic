"use strict";

var connection11 = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
connection11.on("RefreshEmployeeNotiList_", function (group) {
    read_new_noti();
});


connection11.start().then(function () {
    //document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

//ENotificationController
//GetNyUnreadNoti

//return Json(new { Count = nots.Count, Msgs = nots });

var baseUrl = document.getElementById('appConfig').getAttribute('data-base-url');
var baseArea = document.getElementById('areaConfig').getAttribute('data-base-area');

function read_new_noti() {




    $.ajax({
        url: baseUrl,
        //'@Url.Action("GetNyUnreadNoti","ENotificationController",new {area="Designer"})', // Replace with your actual controller and action
        type: 'get', // Or 'GET' depending on your server-side implementation

        contentType: false,
        processData: false,

        success: function (response) {
            //alert(response.Count);
            if (response.Count>0) {
                $("#notiIcon").attr("src", "/Auth/images/redBell.svg");
                var notification = $("#notification").data("kendoNotification");

                notification.show({
                    title: "New Notification",
                    message: "You have new unread notiication!"
                }, "info");

                $("#notificationList").empty();
                $.each(response.Msgs, function (index, notification) {
                    $("#notificationList").append("<li>" + notification + "</li>");
                });
            }
            else {
                $("#notiIcon").attr("src", "/Auth/images/bell.svg");
                $("#notificationList").empty();
            }

            //debugger;
        },
        error: function (error) {
            debugger;
        }
    });
}

read_new_noti();


function show_noti_list() {


    window.location = "/" + baseArea +"/ENotification/Index/";

    return;


    $("#notificationList").toggle();

    if ($("#notificationList").is(":visible")) {
        var notificationContainer = $(".notification-container");
        var notificationList = $("#notificationList");

        // Position the flyout div below the bell icon
        notificationList.css({
            top: '75%',
            left: '-555%'
        });
    }
}
