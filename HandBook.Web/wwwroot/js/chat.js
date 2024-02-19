"use strict"

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

document.getElementById("submitButton").disabled = true;

var currUserDiv = document.getElementById("current-user-username");
var currUsername = currUserDiv.getAttribute("data-username");

connection.on("ReceiveMessage", async function (user, message, shouldRead) {

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
        '<div class="row ' + additionalRowSetting + '">' +
        '   <div class="' + offset + ' ' + additionalRowSetting + '" >' +
        '       <div class="col-auto">' +
        '           <div class="message-container ' + containterClass + ' ' + contcolor + '">' +
        '               <p class="sender ' + textAlign + '" hidden>' + user + '</p>' +
        '               <p class=" ' + textAlign + '">' + message + '</p>' +
        '           </div>' +
        '       </div >' +
        '   </div >' +
        '</div > ';
    document.getElementById("chat").innerHTML += li;
    var targetUser = document.getElementById("targetusername").value;
    if (shouldRead) {
        await connection.invoke("MarkAsRead", user, targetUser).catch(function (err) {
            return console.error(err.toString());
        });
    }
});


connection.start().then(function () {
    document.getElementById("submitButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});


document.getElementById("submitButton").addEventListener("click", function (event) {
    var user = document.getElementById("username").value;
    var targetUser = document.getElementById("targetusername").value;
    var message = document.getElementById("messageText");
    if (!message.value || message.value.trim() === '') {
        message.focus();
    }
    else {
        connection.invoke("SendMessage", user, message.value, targetUser).catch(function (err) {
            return console.error(err.toString());
        });
        event.preventDefault();
    }
    document.getElementById("messageText").value = "";
});

$(document).ready(function () {
    if ($("#confirm-status-of-overlay").length) {
        Swal.fire({
            title: 'Warning!',
            text: 'This user could be a spam account.',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Continue',
            cancelButtonText: 'Block'
        }).then((result) => {
            if (!result.isConfirmed) {
                Swal.fire({
                    title: 'Are you sure?',
                    text: 'Do you want to block this user?',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonColor: '#3085d6',
                    cancelButtonColor: '#d33',
                    confirmButtonText: 'Yes, block',
                    cancelButtonText: 'Cancel'
                }).then((blockResult) => {
                    if (blockResult.isConfirmed) {
                        $.ajax({
                            type: "POST",
                            url: "Messages/BanUser",
                            data: { userId: $('#target-user-username').data('username') },
                            success: function (response) {
                                Swal.fire(
                                    'Banned!',
                                    'The user was banned successfully.',
                                    'success'
                                );
                            },
                            error: function (xhr, status, error) {
                            }
                        });
                    }
                });
            }
        });
    }

    $('html, body').animate({ scrollTop: $('#content').height() }, 0);
});