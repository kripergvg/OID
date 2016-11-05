using System.Collections.Generic;
using System.Threading.Tasks;
using OID.DataProvider.Models;
using OID.DataProvider.Models.User;
using OID.DataProvider.Models.User.In;

namespace OID.DataProvider.Interfaces
{
    public interface IUserProvider
    {
        Task<DataProviderModel<CreatedUserModel>> CreateUser(string email, string userName, string passwordHash);

        Task<DataProviderVoidModel> UpdateUser(UserProfileModel oldProfile, UserProfileModel newProfile);

        Task<DataProviderVoidModel> ChangePassword(string oldPasswordHash, string newPasswordHash);

        Task<DataProviderVoidModel> UpsertUserContacts(UserContactsModel oldContacts, UserContactsModel newContacts);

        Task<DataProviderVoidModel> UpsertAccounts(IList<Account> accounts);

        Task<DataProviderModel<UserModel>> GetUser();

        Task<DataProviderModel<UserPhonesModel>> GetUserPhones();
    }
}