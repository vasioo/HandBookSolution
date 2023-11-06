﻿"use strict"

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

document.getElementById("submitButton").disabled = true;

var currUserDiv = document.getElementById("current-user-username");
var currUsername = currUserDiv.getAttribute("data-username");

connection.on("ReceiveMessage", function (user, message) {

    var containterClass = "";
    var timePosition = "";
    var textAlign = "";
    var offset = "";
    var additionalRowSetting = "";
    var contcolor = "";
    if (user == currUsername) {
        containterClass = "message-sender";
        timePosition = "time-right text-light";
        textAlign = "text-right text-white mb-1 mt-1";
        contcolor = "bg-primary";
        offset = "col-md-7 row";
        additionalRowSetting = "d-flex justify-content-end";
    }
    else {
        containterClass = "message-receiver";
        timePosition = "time-left";
        textAlign = "text-left mb-1 mt-1";
        offset = "col-md-7 row";
    }
    var li =
        '<div class="row '+additionalRowSetting+'">' +
        '   <div class="' + offset + ' ' + additionalRowSetting + '" >' +
        '       <div class="col-auto">'+
        '           <div class="message-container ' + containterClass + ' ' + contcolor + '">' +
        '               <p class="sender ' + textAlign + '" hidden>' + user + '</p>' +
        '               <p class=" '+ textAlign + '">' + message + '</p>' +
        '           </div>' +
        '       </div >' +
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
    var targetUser = document.getElementById("targetusername").value;
    var message = document.getElementById("messageText").value;
    document.getElementById("messageText").value = "";
    connection.invoke("SendMessage", user, message,targetUser).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

$(document).ready(function () {
    $('html, body').animate({ scrollTop: $('#content').height() }, 0);
});