"use strict";

var timer; // Variable to hold the timer ID

function startTimer() {
    // Set a ten-minute timer
    timer = setTimeout(endConversatio, 10 * 60 * 1000); // 10 minutes in milliseconds
}

function resetTimer() {
    // Reset the timer
    clearTimeout(timer);
    startTimer();
}


var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessageFromClient", function (message, src) {




    if (src == 1) {
        resetTimer();

        var li = document.createElement("div");
        document.getElementById("messagesList").appendChild(li);

        li.classList.add("classDestionation");
        li.textContent = `${message}`;
        li.classList.add("classDestionation");

    }
    else {
        var liParent = document.createElement("div");

        document.getElementById("messagesList").appendChild(liParent);

        liParent.setAttribute("style", "display:flex;justify-content: end;");


        var li = document.createElement("div");

        liParent.appendChild(li);
        li.textContent = `${message}`;
        li.classList.add("classSource");

    }

});





connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    //var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    var clientId = document.getElementById("clientIdInput").value;

    document.getElementById("messageInput").value = "";

    //connection.invoke("SendMessage", user, message).catch(function (err) {
    //    return console.error(err.toString());
    //});


    connection.invoke("MessageFromStaff", connection.connectionId, clientId, message).catch(function (err) {
        return console.error(err.toString());
    });

    event.preventDefault();
});

document.getElementById("messageInput").addEventListener("keydown", function (event) {
    // Check if the key pressed is "Enter" (key code 13)
    if (event.key === "Enter") {
        // Your logic here
        sendMessage();
    }
});

function sendMessage() {
    //var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    var clientId = document.getElementById("clientIdInput").value;
    document.getElementById("messageInput").value = "";

    //connection.invoke("SendMessage", user, message).catch(function (err) {
    //    return console.error(err.toString());
    //});


    connection.invoke("MessageFromStaff", connection.connectionId, clientId, message).catch(function (err) {
        return console.error(err.toString());
    });

    event.preventDefault();
}

function sendEndMessage() {
    //var user = document.getElementById("userInput").value;
    var message = document.getElementById("endMSG").value;
    var clientId = document.getElementById("clientIdInput").value;
    document.getElementById("messageInput").value = "";

    //connection.invoke("SendMessage", user, message).catch(function (err) {
    //    return console.error(err.toString());
    //});


    connection.invoke("MessageFromStaff", connection.connectionId, clientId, message).catch(function (err) {
        return console.error(err.toString());
    });

    event.preventDefault();
}

