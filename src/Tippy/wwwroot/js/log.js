"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/loghub").build();
var ansiUp = new AnsiUp;

connection.on("ReceiveLog", function (log) {
    var line = ansiUp.ansi_to_html(log);
    document.getElementById("log-box").innerHTML += "<p>" + line + "</p>";
});

connection.start().then(function () {
}).catch(function (err) {
    return console.error(err.toString());
});
