using System.Threading.Tasks;

namespace InternTask2.Core.Services.Abstract
{
    public interface ISendEmail
    {
        Task<bool> Send(string mailText, string addressee, string subject);
    }
}