using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using Microsoft.Extensions.DependencyInjection;

namespace BPHN.BusinessLayer.ImpServices
{
    public class GlobalVariableService : IGlobalVariableService
    {
        private readonly IAccountRepository _accountRepository;
        private Dictionary<Guid, Account> _dictionaryAccountSystem;
        public GlobalVariableService(IServiceProvider provider)
        {
            using var scope = provider.CreateScope();
            _accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
            _dictionaryAccountSystem = new Dictionary<Guid, Account>();
            Start();
        }

        public Dictionary<Guid, Account> AccountSystem => _dictionaryAccountSystem;

        public void Start()
        {
            var lstAccount = _accountRepository.GetAll().Result;
            for (int i = 0; i < lstAccount.Count; i++)
            {
                var item = lstAccount[i];
                _dictionaryAccountSystem.Add(item.Id, item);
            }
        }

        public void Reset()
        {
            _dictionaryAccountSystem.Clear();
            Start();
        }
    }
}
