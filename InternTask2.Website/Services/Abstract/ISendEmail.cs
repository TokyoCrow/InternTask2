using System.Threading.Tasks;

namespace InternTask2.Website.Services.Abstract
{
    interface ISendEmail
    {
        Task<bool> Send(string mailText, string addressee, string subject);
    }
}