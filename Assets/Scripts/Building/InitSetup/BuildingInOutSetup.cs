public class BuildingInOutSetup
{
    public BuildingInOutSetup(
        ItemType @in = ItemType.NONE, int inAmount = 1, int inMaxBuffer = 5, 
        ItemType @out = ItemType.NONE, int outAmount = 1, int outMaxBuffer = 5, 
        ItemType in2 = ItemType.NONE, int inAmount2 = 1, int inMaxBuffer2 = 5, 
        ItemType out2 = ItemType.NONE, int outAmount2 = 1, int outMaxBuffer2 = 5,
        ItemType in3 = ItemType.NONE, int inAmount3 = 1, int inMaxBuffer3 = 5
    )
    {
        In = @in;            
        InAmount = inAmount;
        InMaxBuffer = inMaxBuffer;

        Out = @out;
        OutAmount = outAmount;
        OutMaxBuffer = outMaxBuffer;

        In2 = in2;
        InAmount2 = inAmount2;
        InMaxBuffer2 = inMaxBuffer2;

        Out2 = out2;
        OutAmount2 = outAmount2;
        OutMaxBuffer2 = outMaxBuffer2;

        In3 = in3;
        InAmount3 = inAmount3;
        InMaxBuffer3 = inMaxBuffer3;
    }

    public ItemType In { get; set; }       
    public int InAmount { get; set; }
    public int InMaxBuffer { get; set; }

    public ItemType Out { get; set; }
    public int OutAmount { get; set; }
    public int OutMaxBuffer { get; set; }

    public ItemType In2 { get; set; }
    public int InAmount2 { get; set; }
    public int InMaxBuffer2 { get; set; }

    public ItemType Out2 { get; set; }
    public int OutAmount2 { get; set; }
    public int OutMaxBuffer2 { get; set; }

    public ItemType In3 { get; set; }
    public int InAmount3 { get; set; }
    public int InMaxBuffer3 { get; set; }
}
