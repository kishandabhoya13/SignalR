﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
//const connection = new signalR.HubConnectionBuilder()
//    .withUrl('https://localhost:7281/notificationHub', {
//        withCredentials: true
//    })
//    .build();

//connection.start()
//    .then(() => console.log('Connected to SignalR'))
//    .catch(err => console.error(err.toString()));

connection.on("Notifications", function (message) {
    console.log("toastyer");
    toastr.info(message);
});