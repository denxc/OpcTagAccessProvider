# OpcTagAccessProvider
The .Net interface and implementation of mechanism for access to OPC parameters on OPC server.

If you need to use an opc-server (RSLinx, Matricon and etc.) the library helps you to do it.
First, you need to create and connect to opc-server. 
Second, you can use OpcValueImpl to read and write values from and into the server.

var server = new OPCServer();
server.Connect("OPC.SimaticNET", "localhost");

var curvalue = new OpcValueImpl(server);
curvalue.Name = "UseYourTagName";
curvalue.IsListenValueChanging = true;
curvalue.SubscribeToValueChange(new Listener());
curvalue.Activate();

curvalue.WriteValue(123);