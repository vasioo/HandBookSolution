"use strict"

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

document.getElementById("submitButton").disabled = true;

var currUserDiv = document.getElementById("current-user-username");
var currUsername = currUserDiv.getAttribute("data-username");

connection.on("ReceiveMessage", function (user, message) {

    var containterClass = "";
    var timePosition = "";
    var textAlign = "";
    var offset = "";
    if (user == currUsername) {
        containterClass = "container darker bg-primary";
        timePosition = "time-right text-light";
        textAlign = "text-right text-white";
        offset = "col-md-6 offset-md-6 row";
    }
    else {
        containterClass = "container bg-light";
        timePosition = "time-left";
        textAlign = "text-left";
        offset = "";
    }
    var li =
        '<div class="row pt-1">' +
        '   <div class="' + offset + '" >' +
        '       <div class="' + containterClass + '">' +
        '           <p class="sender ' + textAlign + '">' + user + '</p>' +
        '           <p class=" '+ textAlign + '">' + message + '</p>' +
        '       </div>' +
        '   </div >' +
        '</div > ';
     document.getElementById("chat").innerHTML += li;
});


connection.start().then(function () {
    document.getElementById("submitButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});


document.getElementById("submitButton").addEventListener("click", function (event) {
    var user = document.getElementById("username").value;
    var message = document.getElementById("messageText").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});