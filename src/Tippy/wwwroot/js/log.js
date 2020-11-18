"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/loghub").build();

connection.on("ReceiveLog", function (log) {
    var logEncoded = log.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    document.getElementById("logList").value += logEncoded;
});

connection.start().then(function () {
}).catch(function (err) {
    return console.error(err.toString());
});
