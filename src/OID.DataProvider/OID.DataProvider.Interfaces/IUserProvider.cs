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

        Task<DataProviderVoidModel> UpdatetUserAddress(UpdateUserAddressModel user);

        Task<DataProviderVoidModel> UpsertAccounts(IList<Account> accounts);

        Task<DataProviderModel<UserModel>> GetUser();

        Task<DataProviderModel<UserPhonesModel>> GetUserPhones();

        Task<DataProviderVoidModel> DeleteUserPhone(PhoneType phoneType, UserPhone phone);

        Task<DataProviderVoidModel> UpsertUserPhone(PhoneType phoneType, UserPhone phone);

        Task<DataProviderModel<List<UserAccount>>> GetUserAccounts();

        Task<DataProviderModel<CreateUserAccountModel>> CreateUserAccount(int accountNumber, PaymentType paymentType);
    }
}