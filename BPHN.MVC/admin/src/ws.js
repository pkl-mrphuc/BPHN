const signalR = require("@microsoft/signalr");
var connection = new signalR.HubConnectionBuilder().withUrl(process.env.VUE_APP_WS, {
    skipNegotiation: true,
    transport: signalR.HttpTransportType.WebSockets
}).build();

export const startWs = (() => {
    connection.start().then(function () {
        console.log("connected");
    }).catch(function (err) {
        console.error(err.toString());
    });

    connection.on("SERVER_ConfirmOtherClientLogin", function() {
        console.log("open confirm dialog");
    });
});

export const isConnected = (() => {
    return connection.state === signalR.HubConnectionState.Connected;
})

export default connection;