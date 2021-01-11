namespace InternTask2.Core.Services.Abstract
{
    public interface ISendEmail
    {
        bool Send(string mailText, string addressee, string subject);
    }
}