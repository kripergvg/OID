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
            string queryIdentifier = Guid.NewGuid().ToString();
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
            string queryIdentifier = Guid.NewGuid().ToString();
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
    }
}
