using BPHN.ModelLayer;

namespace BPHN.DataLayer.IRepositories
{
    public interface IItemRepository
    {
        Task<IEnumerable<Item>> GetItems(Guid accountId, string txtSearch, string status, string code, string unit, string quantity);
        Task<IEnumerable<Item>> GetAll(Guid accountId);
        Task<Item> GetById(Guid id);
        Task<bool> Insert(Item data);
        Task<bool> Update(Item data);
        Task UpdateQuantity(Guid accountId, IEnumerable<InvoiceItem> items);
    }
}
