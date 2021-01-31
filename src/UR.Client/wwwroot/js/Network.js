function appendEvents(json) {
    console.log("Network-appendEvents");
    DotNet.invokeMethod("UR.Client", "AppendEvent", json);
}

let connection = new signalR.HubConnectionBuilder()
    .withUrl("GameHub")
    .build();
connection.on("AppendEvent", appendEvents);
connection.start();
