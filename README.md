# OpcTagAccessProvider
The .Net interface and implementation of mechanism for access to OPC parameters on OPC server. <br />
<br />
If you need to use an opc-server (RSLinx, Matricon and etc.) the library helps you to do it.<br />
First, you need to create and connect to opc-server. <br />
Second, you can use OpcValueImpl to read and write values from and into the server.<br />
<br />
var server = new OPCServer();<br />
server.Connect("OPC.SimaticNET", "localhost");<br />
<br />
var curvalue = new OpcValueImpl(server);<br />
curvalue.Name = "UseYourTagName";<br />
curvalue.IsListenValueChanging = true;<br />
curvalue.SubscribeToValueChange(new Listener());<br />
curvalue.Activate();<br />
<br />
curvalue.WriteValue(123);<br />
