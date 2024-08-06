"use strict";



var ableTosend = 0;

var chatContainerIsOpened = 0;

setMyValue();

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessageFromStaff", function ( message,src) {
    if (src == 2) {
        var liParent = document.createElement("div");



        document.getElementById("messagesList").appendChild(liParent);

        var div1 = document.createElement("div");
        var div2 = document.createElement("div");

        liParent.appendChild(div1);

        var img1 = document.createElement("img");
        img1.src = "/public/images/robotIcon.svg";

        div1.appendChild(img1);


        liParent.appendChild(div2);



        liParent.classList.add("robotContainer");

        liParent = div2;
        liParent.classList.add("divDest");


        var li = document.createElement("div");

        liParent.appendChild(li);
        li.textContent = `${message}`;
        li.classList.add("classDestionation");


    }
    else {
        var liParent = document.createElement("div");

        document.getElementById("messagesList").appendChild(liParent);

        liParent.classList.add("divSource");


        var li = document.createElement("div");

        liParent.appendChild(li);
        li.textContent = `${message}`;
        li.classList.add("classSource");

    }

    if (chatContainerIsOpened == 0) {
        document.querySelector('.newMsg').style.display = 'block';

    }
    else {
        document.querySelector('.newMsg').style.display = 'none';

    }


});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {

    if (ableTosend == 0) {
        return;
    }

    //var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    var clientId = document.getElementById("clientIdInput").value;
    document.getElementById("messageInput").value = "";

    //connection.invoke("SendMessage", user, message).catch(function (err) {
    //    return console.error(err.toString());
    //});


    connection.invoke("MessageFromClient", connection.connectionId, clientId, message).catch(function (err) {
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

    if (ableTosend==0) {
        return;
    }

    //var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    var clientId = document.getElementById("clientIdInput").value;
    document.getElementById("messageInput").value = "";

    //connection.invoke("SendMessage", user, message).catch(function (err) {
    //    return console.error(err.toString());
    //});


    connection.invoke("MessageFromClient", connection.connectionId, clientId, message).catch(function (err) {
        return console.error(err.toString());
    });

    event.preventDefault();
}

function chooseService(id) {

    if (ableTosend==1) {
        return;
    }

    ableTosend = 1;

    //var user = document.getElementById("userInput").value;
    var message = document.getElementById(id).textContent;
    var clientId = document.getElementById("clientIdInput").value;
    document.getElementById("messageInput").value = "";

    //connection.invoke("SendMessage", user, message).catch(function (err) {
    //    return console.error(err.toString());
    //});


    connection.invoke("MessageFromClient", connection.connectionId, clientId, message).catch(function (err) {
        return console.error(err.toString());
    });

    event.preventDefault();
}



function closeChatDiv() {
    // Implement close chat logic here
    // For example, hide the chat container or perform other actions
    document.querySelector('.chat-container').style.display = 'block';
    document.querySelector('.chatBotDiv').style.display = 'none';
    chatContainerIsOpened = 1;
    document.querySelector('.newMsg').style.display = 'none';

}


function closeChat() {
    // Implement close chat logic here
    // For example, hide the chat container or perform other actions
    document.querySelector('.chat-container').style.display = 'none';
    document.querySelector('.chatBotDiv').style.display = 'block';
    chatContainerIsOpened = 0;


}