const signalR = require("@microsoft/signalr");
var connection = new signalR.HubConnectionBuilder().withUrl(process.env.VUE_APP_WS, {
    skipNegotiation: true,
    transport: signalR.HttpTransportType.WebSockets
}).build();

export const isConnected = (() => {
    return connection.state === signalR.HubConnectionState.Connected;
})

export default connection;