"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
//document.getElementById("sendButton").disabled = true;

//connection.on("AddNewGroup", function (user, message,classCss) {
//    var li = document.createElement("li");
//    document.getElementById("messagesList").appendChild(li);

//    li.classList.add(classCss);


//    li.textContent = `New Client: ${message}  `;
//    li.setAttribute("id", message);

//    var anchor = document.createElement("a");

//    // Set href attribute for the anchor element
//    anchor.setAttribute("href", "SingleChatPage/" + message);
//    anchor.setAttribute("target", "_blank");

//    var icon = document.createElement("i");
//    icon.classList.add("fa", "fa-external-link"); // Replace "fa-icon-class" with the actual Font Awesome class for the desired icon

//    // Append the icon to the anchor element
//    anchor.appendChild(icon);


//    // Append the anchor element to the li element
//    li.appendChild(anchor);



//});




connection.on("AddNewGroup1", function (group) {
    sync_handler();
});


connection.start().then(function () {
    //document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    var clientId = document.getElementById("clientIdInput").value;

    //connection.invoke("SendMessage", user, message).catch(function (err) {
    //    return console.error(err.toString());
    //});


    connection.invoke("MessageFromClient", connection.connectionId, clientId, user, message).catch(function (err) {
        return console.error(err.toString());
    });

    event.preventDefault();
});