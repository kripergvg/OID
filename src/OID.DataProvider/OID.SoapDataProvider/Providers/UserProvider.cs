using System;
using System.Collections.Generic;
using System.Data;
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
        private readonly IUserSessionQueryExecutor _sessionQueryExecutor;
        private readonly IAppQueryExecutor _appQueryExecutor;

        public UserProvider(IUserSessionQueryExecutor sessionQueryExecutor, IAppQueryExecutor appQueryExecutor)
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

        public async Task<DataProviderModel<SessionModel>> UpdateUser(UserModel userModel, UserProfileModel oldProfile, UserProfileModel newProfile)
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
            }, userModel).ConfigureAwait(false);

            if (result.ResultMessage.Code == 0)
            {
                return new DataProviderModel<SessionModel>(result.ResultMessage, new SessionModel(result.SessionId));
            }
            else
            {
                return new DataProviderModel<SessionModel>(result.ResultMessage, null);
            }
        }

        public async Task<DataProviderModel<SessionModel>> ChangePassword(UserModel userModel, string oldPasswordHash, string newPasswordHash)
        {
            var queryIdentifier = Guid.NewGuid().ToString();
            var query = new Query(queryIdentifier, "UpdateUser_ChangePassword");

            query.Parameters.Add(new QueryParameter("in", "OldPasswordHash", oldPasswordHash, SqlDbType.NVarChar));
            query.Parameters.Add(new QueryParameter("in", "NewPasswordHash", newPasswordHash, SqlDbType.NVarChar));

            var result = await _sessionQueryExecutor.Execute(new List<Query>
            {
                query
            }, userModel);

            if (result.ResultMessage.Code == 0)
            {
                return new DataProviderModel<SessionModel>(result.ResultMessage, new SessionModel(result.SessionId));
            }
            else
            {
                return new DataProviderModel<SessionModel>(result.ResultMessage, null);
            }
        }

        public async Task<DataProviderModel<SessionModel>> UpsertUserContacts(UserModel userModel, UserContactsModel oldContacts, UserContactsModel newContacts)
        {
            var queries = new List<Query>();

            GetPhoneSaveQuery(newContacts.PhoneMobile, oldContacts.PhoneMobile, "Mobile", queries);
            GetPhoneSaveQuery(newContacts.PhoneWork, oldContacts.PhoneWork, "Work", queries);
            GetPhoneSaveQuery(newContacts.PhoneHome, oldContacts.PhoneHome, "Home", queries);
            GetPhoneSaveQuery(newContacts.PhoneAdditional, oldContacts.PhoneAdditional, "Additional", queries);

            if ((newContacts.DeliveryLocationType == "City" && newContacts.CityCode != null)
                || (newContacts.DeliveryLocationType == "Location" && newContacts.LocalityCode != null)
                || !String.IsNullOrWhiteSpace(newContacts.Address))
            {
                var queryIdentifier = Guid.NewGuid().ToString();
                var q1 = new Query(queryIdentifier, "UpdateUser");

                q1.Parameters.Add(!string.IsNullOrEmpty(newContacts.Address)
                    ? new QueryParameter("in", "Address", newContacts.Address, SqlDbType.NVarChar)
                    : new QueryParameter("in", "Address", "", SqlDbType.NVarChar));

                if (newContacts.DeliveryLocationType == "City" && newContacts.CityCode != null)
                {
                    q1.Parameters.Add(new QueryParameter("in", "CityCode", newContacts.CityCode, SqlDbType.NVarChar));
                }

                if (newContacts.DeliveryLocationType == "Location" && newContacts.LocalityCode != null)
                {
                    q1.Parameters.Add(new QueryParameter("in", "CityCode", newContacts.LocationCode, SqlDbType.NVarChar));
                }

                queries.Add(q1);
            }

            var result = await _sessionQueryExecutor.Execute(queries, userModel).ConfigureAwait(false);

            return new DataProviderModel<SessionModel>(result.ResultMessage, new SessionModel(result.SessionId));
        }

        public async Task<DataProviderModel<SessionModel>> UpsertAccounts(UserModel userModel, IList<Account> accounts)
        {
            var listQuery = new List<Query>();

            foreach (var acc in accounts)
            {
                if (acc.AccountId == null || acc.Deleted)
                {
                    listQuery.Add(AccountToQuery(acc));
                }
            }

            var result = await _sessionQueryExecutor.Execute(listQuery, userModel).ConfigureAwait(false);

            return new DataProviderModel<SessionModel>(result.ResultMessage, new SessionModel(result.SessionId));
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

        private List<Query> GetPhoneSaveQuery(UserContactsModel.UserPhone phone, UserContactsModel.UserPhone old_phone, string PhoneType, List<Query> listQuery)
        {
            var guid = Guid.NewGuid().ToString();

            if (phone != null)
            {
                if (old_phone != null && String.IsNullOrWhiteSpace(phone.Number))
                {
                    //Delete phone
                    var q2 = new Query(guid, "DeleteUserPhone");
                    q2.Parameters.Add(new QueryParameter("in", "PhoneType_Name", PhoneType, SqlDbType.NVarChar));
                    q2.Parameters.Add(new QueryParameter("in", "PhoneNumber", GetPhoneOnlyDigits(old_phone.Number), SqlDbType.Int));

                    listQuery.Add(q2);
                }
                else if (!String.IsNullOrWhiteSpace(phone.Number))
                {
                    var q1 = new Query(guid, "UpsertUserPhone");
                    q1.Parameters.Add(new QueryParameter("in", "PhoneType_Name", PhoneType, SqlDbType.NVarChar));
                    q1.Parameters.Add(new QueryParameter("in", "PhoneNumber", GetPhoneOnlyDigits(phone.Number), SqlDbType.Int));
                    q1.Parameters.Add(new QueryParameter("in", "Comment", phone.Comment, SqlDbType.NVarChar));

                    listQuery.Add(q1);

                }
            }

            return listQuery;
        }

        private static string GetPhoneOnlyDigits(string s)
        {
            return s.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Replace("+", "");
        }
    }
}
