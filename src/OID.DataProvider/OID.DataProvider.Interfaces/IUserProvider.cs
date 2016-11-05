using System.Collections.Generic;
using System.Threading.Tasks;
using OID.DataProvider.Models;
using OID.DataProvider.Models.User;
using OID.DataProvider.Models.User.In;
using UserModel = OID.DataProvider.Models.UserModel;

namespace OID.DataProvider.Interfaces
{
    public interface IUserProvider
    {
        Task<DataProviderModel<CreatedUserModel>> CreateUser(string email, string userName, string passwordHash);

        Task<DataSessionProviderVoidModel> UpdateUser(UserModel userModel, UserProfileModel oldProfile, UserProfileModel newProfile);

        Task<DataSessionProviderVoidModel> ChangePassword(UserModel userModel, string oldPasswordHash, string newPasswordHash);

        Task<DataSessionProviderVoidModel> UpsertUserContacts(UserModel userModel, UserContactsModel oldContacts, UserContactsModel newContacts);

        Task<DataSessionProviderVoidModel> UpsertAccounts(UserModel userModel, IList<Account> accounts);

        Task<DataSessionProviderModel<DataProvider.Models.User.UserModel>> GetUser(UserModel userModel);

        Task<DataSessionProviderModel<UserPhonesModel>> GetUserPhones(UserModel userModel);
    }
}