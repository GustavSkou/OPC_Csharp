using Opc.UaFx;
using Opc.UaFx.Client;

public class Program
{
    OpcClient? _accessPoint;
    //string _serverURL = "opc.tcp://192.168.0.122:4840";  //Physical PLC
    string _serverURL = "opc.tcp://127.0.0.1:4840";      //Simulated PLC

    //Main for testing
    static void Main(string[] args)
    {
        Program prg = new Program();

        using (prg._accessPoint = new OpcClient(prg._serverURL))
        {
            //Connect to server
            prg.ConnectToServer();

            int batchId = 29;
            BeerType beerType = BeerType.Stout;
            int amount = 50;

            prg.WriteBatchToServer(new Batch(batchId, beerType, amount));

            while (true)
            {
                prg.ReadFromServer();
                //prg.ReadTreagerWay();
                Thread.Sleep(500); //Hang out for half a second (testing)
            }
        }
    }

    public void ReadFromServer()
    {
        //Read example node values
        var temperature = _accessPoint.ReadNode(NodeIds.StatusTemp);
        var movement = _accessPoint.ReadNode(NodeIds.StatusMovement);

        var ctrlcmd = _accessPoint.ReadNode(NodeIds.CntrlCmd);
        var param2 = _accessPoint.ReadNode(NodeIds.Statuspara2);

        var value = _accessPoint.ReadNode(NodeIds.StatusBatchId);

        //Print values to console window
        //Console.WriteLine($"Temp: {temperature,-5} Movement: {movement,-5} param2: {param2,-5} CtrlCmd: {ctrlcmd,-5} Value: {value,-5}");
        Console.WriteLine($"Temp: {temperature,-5} Movement: {movement,-5} param2: {param2,-5} CtrlCmd: {ctrlcmd,-5} Value: {value,-5}");
    }

    public void ReadValuesFromServer()
    {
        OpcReadNode[] commands = new OpcReadNode[] {
            new OpcReadNode(NodeIds.CmdId),
            new OpcReadNode(NodeIds.CmdType),
            new OpcReadNode(NodeIds.CmdAmount)
        };

        IEnumerable<OpcValue> job = _accessPoint.ReadNodes(commands);
        foreach (OpcValue o in job)
        {
            Console.WriteLine((float)o.Value);
        }
    }

    public void WriteBatchToServer(Batch batch)
    {
        // Values are up casted to insure that the types algin with the expected data types
        OpcWriteNode[] commands = {
            new OpcWriteNode(NodeIds.CmdId, (float) batch.Id),
            new OpcWriteNode(NodeIds.CmdType, (float) batch.Type),
            new OpcWriteNode(NodeIds.CmdAmount, (float) batch.Amount),
            new OpcWriteNode(NodeIds.MachSpeed, (float) 250),
            new OpcWriteNode(NodeIds.CntrlCmd, 2),
            new OpcWriteNode(NodeIds.CmdChangeRequest, true )
        };
        _accessPoint.WriteNodes(commands);
    }

    public void ConnectToServer()
    {
        _accessPoint.Connect();
    }
    public void DisconnectFromServer()
    {
        _accessPoint.Disconnect();
        _accessPoint.Dispose(); //Clean up in case it wasn't automatically handled
    }
}