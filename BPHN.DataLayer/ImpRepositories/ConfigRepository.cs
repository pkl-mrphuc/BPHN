﻿using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Google.Protobuf.WellKnownTypes;
using Org.BouncyCastle.Utilities.Collections;
using System.Collections;

namespace BPHN.DataLayer.ImpRepositories
{
    public class ConfigRepository : BaseRepository, IConfigRepository
    {
        public ConfigRepository(IOptions<AppSettings> appSettings) : base(appSettings)
        {

        }

        public List<Config> GetConfigs(Guid accountId, string key = null)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("@accountId", accountId);
                string query = $"select c.* from configs c where c.AccountId = @accountId";
                if (!string.IsNullOrEmpty(key))
                {
                    dic.Add("@key", key);
                    query = $"select c.* from configs c where c.AccountId = @accountId and c.Key in (@key)";
                }
                var configs = connection.Query<Config>(query, dic).ToList();
                return configs;
            }
        }

        public bool Save(List<Config> configs)
        {
            using (var connection = ConnectDB(GetConnectionString())) 
            {
                connection.Open();
                using (var tranction = connection.BeginTransaction())
                {
                    foreach (var config in configs)
                    {
                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        dic.Add("@id", config.Id);
                        dic.Add("@accountId", config.AccountId);
                        dic.Add("@key", config.Key);
                        dic.Add("@value", config.Value);
                        dic.Add("@createdDate", config.CreatedDate);
                        dic.Add("@createdBy", config.CreatedBy);
                        dic.Add("@modifiedDate", config.ModifiedDate);
                        dic.Add("@modifiedBy", config.ModifiedBy);
                        string query = @"set @idConfig = (select c.id from configs c where c.AccountId = @accountId and c.`Key` = @key);
                                        set @idConfig = if(@idConfig is null, @id, @idConfig);
                                        insert into configs(Id, AccountId, `Key`, Value, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy)
                                            value(@idConfig, @accountId, @key, @value, @createdDate, @createdBy, @modifiedDate, @modifiedBy)
                                        on duplicate key update Value = @value, ModifiedDate = @modifiedDate, ModifiedBy = @modifiedBy";
                        int affect = connection.Execute(query, dic, tranction, commandType: System.Data.CommandType.Text);
                        if(affect == 0)
                        {
                            tranction.Rollback();
                            return false;
                        }
                    }
                    tranction.Commit();
                }
            }
            return true;
        }
    }
}
