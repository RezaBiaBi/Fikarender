namespace Fikarender.Models
{
    public enum ShortLinkType
    {
        Blog,
        Service,
        WorkSample
    }
    
    public enum TagType
    {
        Blog,
        Faq,
        /*Service*/
    }

    public enum WorkSampleType
    {
        Video,
        Picture,
        File
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
