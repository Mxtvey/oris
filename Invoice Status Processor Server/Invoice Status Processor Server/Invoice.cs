namespace Invoice_Status_Processor_Server;

public class Invoice
{

    internal int id{get;set;}
    internal string bankname { get; set; }
    public decimal amount { get; set; }
    public string status { get; set; }
    public DateTime updateat { get; set; }
}