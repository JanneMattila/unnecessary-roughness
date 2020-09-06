function appendEvents(xml) {
    console.log("Network-appendEvents");
    DotNet.invokeMethod("UR.Client", "AppendEvent", xml);
}

let connection = new signalR.HubConnectionBuilder()
    .withUrl("GameHub")
    .build();
connection.on("AppendEvent", appendEvents);
connection.start();
