public class Batch
{
    private int _id;
    private BeerType _type;
    private int _amount;

    public int Id
    {
        get { return _id; }
        set { _id = value; }
    }

    public BeerType Type
    {
        get { return _type; }
        set { _type = value; }
    }

    public int Amount
    {
        get { return _amount; }
        set { _amount = value; }
    }

    public Batch(int id, BeerType type, int amount)
    {
        _id = id;
        _type = type;
        _amount = amount;
    }
}