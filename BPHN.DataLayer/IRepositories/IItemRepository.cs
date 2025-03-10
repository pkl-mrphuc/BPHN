using BPHN.ModelLayer;

namespace BPHN.DataLayer.IRepositories
{
    public interface IItemRepository
    {
        Task<IEnumerable<Item>> GetAll(Guid accountId);
        Task<Item> GetById(Guid id);
        Task<bool> Insert(Item data);
        Task<bool> Update(Item data);
    }
}
