using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using OID.DataProvider.Interfaces;
using OID.DataProvider.Models;
using OID.DataProvider.Models.User;
using OID.DataProvider.Models.User.In;
using OID.SoapDataProvider.Providers.Infrastructure;

namespace OID.SoapDataProvider.Providers
{
    public class UserProvider : IUserProvider
    {
        private readonly IUserSessionQueryExecutorDecorator _sessionQueryExecutor;
        private readonly IAppQueryExecutorDecorator _appQueryExecutor;

        public UserProvider(IUserSessionQueryExecutorDecorator sessionQueryExecutor, IAppQueryExecutorDecorator appQueryExecutor)
        {
            _sessionQueryExecutor = sessionQueryExecutor;
            _appQueryExecutor = appQueryExecutor;
        }

        public async Task<DataProviderModel<CreatedUserModel>> CreateUser(string email, string userName, string passwordHash)
        {
            var queryIdetifier = Guid.NewGuid().ToString();
            var query = new Query(queryIdetifier, "CreateUser");
            query.Parameters.Add(new QueryParameter("in", "Email", email));
            query.Parameters.Add(new QueryParameter("in", "UserName", userName));
            query.Parameters.Add(new QueryParameter("in", "PasswordHash", passwordHash));
            query.Parameters.Add(new QueryParameter("in", "SocAccount_Name", "Email"));
            query.Parameters.Add(new QueryParameter("in", "isNeedToAuth", "Y"));

            var result = await _appQueryExecutor.Execute(new List<Query>
            {
                query
            }).ConfigureAwait(false);

            var model = new DataProviderModel<CreatedUserModel>(result.ResultMessage, null);

            foreach (var q1 in result.Queries)
            {
                if (q1.Name == "CreateUser")
                {
                    if (q1.Parameters.Exists(x => x.Code == "UserSession_Id"))
                    {
                        model.Model = new CreatedUserModel(q1.Parameters.Find(x => x.Code == "UserSession_Id").Value);
                    }

                }
            }

            return model;
        }

        public async Task<DataProviderVoidModel> UpdateUser(UserProfileModel oldProfile, UserProfileModel newProfile)
        {
            var queryIdentifier = Guid.NewGuid().ToString();
            var query = new Query(queryIdentifier, "UpdateUser");

            if ((newProfile.LastName ?? "") != (oldProfile.LastName ?? ""))
            {
                query.Parameters.Add(new QueryParameter("in", "LastName", newProfile.LastName ?? "", SqlDbType.NVarChar));
            }
            if ((newProfile.FirstName ?? "") != (oldProfile.FirstName ?? ""))
            {
                query.Parameters.Add(new QueryParameter("in", "FirstName", newProfile.FirstName ?? "", SqlDbType.NVarChar));
            }
            if ((newProfile.SecondName ?? "") != (oldProfile.SecondName ?? ""))
            {
                query.Parameters.Add(new QueryParameter("in", "SecondName", newProfile.SecondName ?? "", SqlDbType.NVarChar));
            }
            if (oldProfile.BirthDate != newProfile.BirthDate)
            {
                query.Parameters.Add(new QueryParameter("in", "BirthDate", newProfile.BirthDate?.ToString() ?? String.Empty, SqlDbType.DateTime));
            }
            if ((newProfile.UserName ?? "") != (oldProfile.UserName ?? ""))
            {
                query.Parameters.Add(new QueryParameter("in", "UserName", newProfile.UserName, SqlDbType.NVarChar));
            }

            var result = await _sessionQueryExecutor.Execute(new List<Query>
            {
                query
            }).ConfigureAwait(false);

            return new DataProviderVoidModel(result.ResultMessage);
        }

        public async Task<DataProviderVoidModel> ChangePassword(string oldPasswordHash, string newPasswordHash)
        {
            var queryIdentifier = Guid.NewGuid().ToString();
            var query = new Query(queryIdentifier, "UpdateUser_ChangePassword");

            query.Parameters.Add(new QueryParameter("in", "OldPasswordHash", oldPasswordHash, SqlDbType.NVarChar));
            query.Parameters.Add(new QueryParameter("in", "NewPasswordHash", newPasswordHash, SqlDbType.NVarChar));

            var result = await _sessionQueryExecutor.Execute(new List<Query>
            {
                query
            });

            return new DataProviderVoidModel(result.ResultMessage);
        }

        public async Task<DataProviderVoidModel> UpdatetUserAddress(UpdateUserAddressModel user)
        {
            var queries = new List<Query>();

            if ((user.DeleveryLocationType == DeleveryLocationType.City && user.CityCode != null)
                || (user.DeleveryLocationType == DeleveryLocationType.Location && user.LocalityCode != null)
                || !String.IsNullOrWhiteSpace(user.Address))
            {
                var queryIdentifier = Guid.NewGuid().ToString();
                var q1 = new Query(queryIdentifier, "UpdateUser");

                q1.Parameters.Add(!string.IsNullOrEmpty(user.Address)
                    ? new QueryParameter("in", "Address", user.Address, SqlDbType.NVarChar)
                    : new QueryParameter("in", "Address", "", SqlDbType.NVarChar));

                if (user.DeleveryLocationType == DeleveryLocationType.City && user.CityCode != null)
                {
                    q1.Parameters.Add(new QueryParameter("in", "CityCode", user.CityCode, SqlDbType.NVarChar));
                }

                if (user.DeleveryLocationType == DeleveryLocationType.Location && user.LocalityCode != null)
                {
                    q1.Parameters.Add(new QueryParameter("in", "CityCode", user.LocalityCode, SqlDbType.NVarChar));
                }

                queries.Add(q1);
            }

            var result = await _sessionQueryExecutor.Execute(queries).ConfigureAwait(false);

            return new DataProviderVoidModel(result.ResultMessage);
        }

        public async Task<DataProviderVoidModel> DeleteUserPhone(PhoneType phoneType, UserPhone phone)
        {
            var guid = Guid.NewGuid().ToString();

            var queries = new List<Query>();
            var q2 = new Query(guid, "DeleteUserPhone");
            q2.Parameters.Add(new QueryParameter("in", "PhoneType_Name", phoneType.GetName(), SqlDbType.NVarChar));
            q2.Parameters.Add(new QueryParameter("in", "PhoneNumber", GetPhoneOnlyDigits(phone.Number), SqlDbType.Int));
            queries.Add(q2);

            var result = await _sessionQueryExecutor.Execute(queries).ConfigureAwait(false);
            return new DataProviderVoidModel(result.ResultMessage);
        }

        public async Task<DataProviderVoidModel> UpsertUserPhone(PhoneType phoneType, UserPhone phone)
        {
            var guid = Guid.NewGuid().ToString();
            var q1 = new Query(guid, "UpsertUserPhone");
            q1.Parameters.Add(new QueryParameter("in", "PhoneType_Name", phoneType.GetName(), SqlDbType.NVarChar));
            q1.Parameters.Add(new QueryParameter("in", "PhoneNumber", GetPhoneOnlyDigits(phone.Number), SqlDbType.Int));
            q1.Parameters.Add(new QueryParameter("in", "Comment", phone.Comment, SqlDbType.NVarChar));

            var queries = new List<Query> {q1};

            var result = await _sessionQueryExecutor.Execute(queries).ConfigureAwait(false);
            return new DataProviderVoidModel(result.ResultMessage);
        }

        public async Task<DataProviderVoidModel> UpsertAccounts(IList<Account> accounts)
        {
            var listQuery = new List<Query>();

            foreach (var acc in accounts)
            {
                if (acc.AccountId == null || acc.Deleted)
                {
                    listQuery.Add(AccountToQuery(acc));
                }
            }

            var result = await _sessionQueryExecutor.Execute(listQuery).ConfigureAwait(false);

            return new DataProviderVoidModel(result.ResultMessage);
        }

        public async Task<DataProviderModel<DataProvider.Models.User.UserModel>> GetUser()
        {
            List<Query> listQuery = new List<Query>();
            string q_guid = Guid.NewGuid().ToString();
            Query query = new Query(q_guid, "GetUser");
            listQuery.Add(query);

            var result = await _sessionQueryExecutor.Execute(listQuery).ConfigureAwait(false);

            var userRows = result.Queries.FirstOrDefault(q => q.Name == "GetUser")?.RetTable?.Rows;
            if (userRows != null && userRows.Count > 0)
            {
                var userRow = userRows[0];
                DeleveryLocationType? deleveryType = null;
                if (userRow["DeliveryLocationType"] != null)
                {
                    switch (userRow["DeliveryLocationType"].ToString())
                    {
                        case "Location":
                            deleveryType = DeleveryLocationType.Location;
                            break;
                        case "City":
                            deleveryType = DeleveryLocationType.City;
                            break;
                    }
                }

                DateTime? birthDate = null;
                if (!String.IsNullOrEmpty(userRow["BirthDate"]?.ToString()))
                {
                    birthDate = Convert.ToDateTime(userRow["BirthDate"].ToString());
                }

                var user = new DataProvider.Models.User.UserModel(
                    userRow["User_Id"].ToString(),
                    userRow["Email"].ToString(),
                    userRow["UserName"].ToString(),
                    userRow["LastName"].ToString(),
                    userRow["FirstName"].ToString(),
                    userRow["SecondName"].ToString(),
                    birthDate,
                    userRow["CityCode"].ToString(),
                    userRow["LocalityCode"].ToString(),
                    userRow["RegionCode"].ToString(),
                    deleveryType,
                    userRow["Address"].ToString(),
                    Convert.ToDateTime(userRow["CreateDate"].ToString()),
                    userRow["Blocked"].ToString() != "N");

                return new DataProviderModel<DataProvider.Models.User.UserModel>(result.ResultMessage, user);
            }

            return new DataProviderModel<DataProvider.Models.User.UserModel>(result.ResultMessage);
        }

        public async Task<DataProviderModel<UserPhonesModel>> GetUserPhones()
        {
            List<Query> listQuery = new List<Query>();
            string q_guid = Guid.NewGuid().ToString();
            Query query = new Query(q_guid, "GetUserPhones");
            listQuery.Add(query);

            var result = await _sessionQueryExecutor.Execute(listQuery).ConfigureAwait(false);

            var userRows = result.Queries.FirstOrDefault(q => q.Name == "GetUserPhones")?.RetTable?.Rows;
            if (userRows != null && userRows.Count > 0)
            {
                UserPhone mobile = null, work = null, home = null, additional = null;
                foreach (DataRow userRow in userRows)
                {
                    if (userRow["PhoneType_Name"].ToString() == "Mobile")
                    {
                        mobile = new UserPhone(userRow["PhoneNumber"].ToString(), userRow["Comment"].ToString());
                    }
                    if (userRow["PhoneType_Name"].ToString() == "Work")
                    {
                        work = new UserPhone(userRow["PhoneNumber"].ToString(), userRow["Comment"].ToString());
                    }
                    if (userRow["PhoneType_Name"].ToString() == "Home")
                    {
                        home = new UserPhone(userRow["PhoneNumber"].ToString(), userRow["Comment"].ToString());
                    }
                    if (userRow["PhoneType_Name"].ToString() == "Additional")
                    {
                        additional = new UserPhone(userRow["PhoneNumber"].ToString(), userRow["Comment"].ToString());
                    }
                }

                var model = new UserPhonesModel(mobile, work, home, additional);

                return new DataProviderModel<UserPhonesModel>(result.ResultMessage, model);
            }

            return new DataProviderModel<UserPhonesModel>(result.ResultMessage);
        }

        public async Task<DataProviderModel<List<UserAccount>>> GetUserAccounts()
        {
            var queryIdentifier1 = Guid.NewGuid().ToString();
            var query1 = new Query(queryIdentifier1, "GetUserAccounts");
            var listQuery = new List<Query> { query1 };

            var result = await _sessionQueryExecutor.Execute(listQuery).ConfigureAwait(false);

            var getUserAccountsQuery = result.Queries.First(q => q.Name == "GetUserAccounts");

            var dataTable = getUserAccountsQuery.RetTable;

            var userAccounts = new List<UserAccount>();
            foreach (DataRow row in dataTable.Rows)
            {
                var paymentType = PaymentType.Yandex;
                switch (int.Parse(row["PaymentCptyService_Id"].ToString()))
                {
                    case 2:
                        paymentType = PaymentType.Yandex;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("PaymentCptyService_Id");
                }

                userAccounts.Add(new UserAccount(
                    row["UserAccount_Id"].GetInt(),
                    row["Account_Id"].GetInt(),
                    paymentType,
                    row["AccountNumber"].GetInt(),
                    row["CreateDate"].GetDateTime()));


            }

            return new DataProviderModel<List<UserAccount>>(result.ResultMessage, userAccounts);
        }

        public async Task<DataProviderModel<CreateUserAccountModel>> CreateUserAccount(int accountNumber, PaymentType paymentType)
        {
            var guid = Guid.NewGuid().ToString();
            var q1 = new Query(guid, "InsertUserAccount");
            q1.Parameters.Add(new QueryParameter("in", "CptyService_Id", ((int)paymentType).ToString(), SqlDbType.Int));
            q1.Parameters.Add(new QueryParameter("in", "AccountNumber", accountNumber.ToString(), SqlDbType.NVarChar));

            var queries = new List<Query> { q1 };

            var result = await _sessionQueryExecutor.Execute(queries).ConfigureAwait(false);

            var createUserAccountQuery = result.Queries.FirstOrDefault(q => q.Name == "InsertUserAccount");
            if (createUserAccountQuery != null)
            {
                
            }

            return new DataProviderModel<CreateUserAccountModel>(result.ResultMessage);
        }

        private static Query AccountToQuery(Account acc)
        {
            var queryIdentifier = Guid.NewGuid().ToString();
            Query query;

            if (acc.Deleted)
            {
                query = new Query(queryIdentifier, "DeleteUserAccount");
                query.Parameters.Add(new QueryParameter("in", "UserAccount_Id", acc.UserAccountId, SqlDbType.Int));
            }
            else
            {
                query = new Query(queryIdentifier, "InsertUserAccount");
                query.Parameters.Add(new QueryParameter("in", "CptyService_Id", acc.PaymentCptyServiceId, SqlDbType.Int));
                query.Parameters.Add(new QueryParameter("in", "AccountNumber", acc.AccountNumber, SqlDbType.NVarChar));
            }

            return query;
        }

        private static string GetPhoneOnlyDigits(string s)
        {
            if (String.IsNullOrEmpty(s))
            {
                return s;
            }
            else
            {
                return s.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Replace("+", "");
            }
        }
    }
}
