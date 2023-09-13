"use strict";

const username = "@User.Identity.Name";
document.addEventListener("DOMContentLoaded", function () {

    // Retrieve the user's name from the hidden input field
    const userInput = document.getElementById("userInput");
    const username = userInput ? userInput.value : "";


    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub")
        .build();
    console.log(username);

    // Disable the send button until the connection is established.
    document.getElementById("sendButton").disabled = true;

    connection.on("ReceiveMessage", function (username, message) {
        var li = document.createElement("li");
        document.getElementById("messagesList").appendChild(li);
        li.textContent = `${username}: ${message}`;
        messageInput.value = "";
    });

    connection.start()
        .then(() => {
            console.log("SignalR connection started.");
            // Enable the send button after the connection is established.
            document.getElementById("sendButton").disabled = false;
        })
        .catch((err) => {
            console.error("SignalR connection error: " + err);
        });

    document.getElementById("sendButton").addEventListener("click", function (event) {
        var user = document.getElementById("userInput").value;
        var message = document.getElementById("messageInput").value;
        connection.invoke("SendMessage", username, message).catch(function (err) {
            return console.error(err.toString());
        });
        event.preventDefault();
    });
});