using Models.Entity;

namespace RabbitListener.Services.Interface
{
    public interface ILogService
    {
        Task InsertLog(LogModel log);
    }
}
