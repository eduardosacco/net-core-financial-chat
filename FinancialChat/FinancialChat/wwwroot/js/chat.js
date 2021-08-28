"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.

    var date = new Date();
    var timestamp = date.toLocaleString('es-AR');
    var li = document.createElement("li");
    var userName = document.createElement("b");
    userName.textContent = `${user}: `;
    document.getElementById("messagesList").appendChild(li);
    li.appendChild(document.createTextNode(`[${timestamp}] - `));
    li.appendChild(userName);
    li.appendChild(document.createTextNode(`${message}`));
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});