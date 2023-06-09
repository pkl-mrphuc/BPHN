using System.Net.Mail;

namespace BPHN.BusinessLayer.IServices
{
    public interface IMailBuilder
    {
        string BuildSubject(object? data);
        Task<string> BuildBody(object? data);
        List<Attachment> BuildAttachments(object? data);
    }
}
