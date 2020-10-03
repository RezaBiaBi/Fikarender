namespace Fikarender.Models
{
    public enum ShortLinkType
    {
        Blog,
        Service,
        Faq
    }
    
    public enum TagType
    {
        Blog,
        Faq,
        /*Service*/
    }

    public enum OrderStatus
    {
        Initial,
        PaymentDone,
        Proccessing,
        Sent,
        Completed
    }
}
