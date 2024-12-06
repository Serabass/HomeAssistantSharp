using HomeAssistantSharp.Client.Websocket;

var client = new HomeAssistantWebsocketClient("ws://hass.local/api/websocket", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiI1Y2Y4NTFiYzhiNTc0M2JiODA2M2U5ZGE5NzgzNTE2OCIsImlhdCI6MTcxOTI5MzgzOSwiZXhwIjoyMDM0NjUzODM5fQ.AyVoJzwscxvJ403zacMOX2Vc78W_CgiusUdCZ0amPcc");

await client.ConnectAsync();

client.Ready += (sender, args) =>
{
  Console.WriteLine("Ready");
};
