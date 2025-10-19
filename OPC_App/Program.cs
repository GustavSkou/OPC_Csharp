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

            Thread.Sleep(500);
            int batchId = 327;
            BeerType beerType = BeerType.Pilsner;
            int amount = 50;
            prg.ResetMachine();
            Thread.Sleep(500);
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
        var humidity = _accessPoint.ReadNode(NodeIds.StatusHumidity);

        var batchId = _accessPoint.ReadNode(NodeIds.StatusBatchId);

        var machspeed = _accessPoint.ReadNode(NodeIds.StatusMachSpeed);

        var toProducedAmount = _accessPoint.ReadNode(NodeIds.StatusCurAmount);
        var producedAmount = _accessPoint.ReadNode(NodeIds.AdminProcessedCount);
        //var producedAmount = _accessPoint.ReadNode(NodeIds.StatusCurAmount);

        //Print values to console window
        //Console.WriteLine($"Temp: {temperature,-5} Movement: {movement,-5} humidity: {humidity,-5} CtrlCmd: {ctrlcmd,-5} batch id: {batchId,-5}");
        Console.WriteLine($"current amount: {producedAmount,-5} goal amount: {toProducedAmount,-5} speed: {machspeed,-5} batch id: {batchId,-5}");
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

        // when brew
        OpcWriteNode[] commands = {
            new OpcWriteNode(NodeIds.CmdId, (float) batch.Id),
            new OpcWriteNode(NodeIds.CmdType, (float) batch.Type),
            new OpcWriteNode(NodeIds.CmdAmount, (float) batch.Amount),
            new OpcWriteNode(NodeIds.MachSpeed, (float) 222),
            new OpcWriteNode(NodeIds.CntrlCmd, 2),
            new OpcWriteNode(NodeIds.CmdChangeRequest, true )
        };
        if (_accessPoint == null) throw new Exception();

        _accessPoint.WriteNodes(commands);
    }

    public void ResetMachine()
    {
        OpcWriteNode[] commands = {
            new OpcWriteNode(NodeIds.CntrlCmd, 1),
            new OpcWriteNode(NodeIds.CmdChangeRequest, true)
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