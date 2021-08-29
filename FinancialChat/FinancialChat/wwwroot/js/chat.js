"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.

    // Messages already arrive in order by time
    var date = new Date();
    var timestamp = date.toLocaleString('es-AR');

    // remove messages if over 50 elements
    var messageList = document.getElementById("messagesList");
    if (messageList.childElementCount > 50) {
        messageList.lastElementChild.remove();
    }

    var li = document.createElement("li");
    var userName = document.createElement("b");
    userName.textContent = `${user}: `;
    li.appendChild(document.createTextNode(`[${timestamp}] - `));
    li.appendChild(userName);
    li.appendChild(document.createTextNode(`${message}`));

    // Prepend so newer messages appear first
    messageList.prepend(li);

});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});