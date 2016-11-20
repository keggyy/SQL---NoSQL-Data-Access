﻿using SQL.NoSQL.BLL.Common.DTO;
using SQL.NoSQL.BLL.NoSQL.DAL.Entity;
using SQL.NoSQL.Library.Interfaces;
using SQL.NoSQL.Library.NoSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQL.NoSQL.BLL.NoSQL.Repository
{
    public class NoSQLLogRepository : IRepository<LogDto>
    {
        public override void Delete(LogDto dto)
        {
            using (UnitOfMongo op = new UnitOfMongo())
            {
                NoSQLLogEntity entity = op.Query<NoSQLLogEntity>().Where(x => x.Id.Equals(dto.Id)).FirstOrDefault();
                if (entity != null)
                    op.Delete(entity);
            }
        }

        public override List<LogDto> GetAll()
        {
            using (UnitOfMongo op = new UnitOfMongo())
            {
                List<NoSQLLogEntity> entity = op.Query<NoSQLLogEntity>().ToList();
                return ConvertEntityListToDtoList(entity);
            }
        }

        public override LogDto GetById(Guid Id)
        {
            using (UnitOfMongo op = new UnitOfMongo())
            {
                NoSQLLogEntity entity = op.Query<NoSQLLogEntity>().Where(x => x.Id.Equals(Id)).FirstOrDefault();
                return ConvertEntityToDto(entity);
            }
        }

        public override void Save(LogDto dto)
        {
            using (UnitOfMongo op = new UnitOfMongo())
            {
                NoSQLLogEntity entity = op.Query<NoSQLLogEntity>().Where(x => x.Id.Equals(dto.Id)).FirstOrDefault();
                if (entity == null)
                    entity = new NoSQLLogEntity();
                entity.AppId = dto.App.Id;
                entity.AppName = dto.App.Name;
                entity.Level = dto.Level;
                entity.LogDate = dto.LogDate;
                entity.Message = dto.Message;
                op.SaveOrUpdate(entity);

            }
        }

        public List<LogDto> Search(Guid? SelectedApp, string TextToSearch)
        {
            using (UnitOfMongo op = new UnitOfMongo())
            {
                op.BeginTransaction();
                IQueryable<NoSQLLogEntity> query = op.Query<NoSQLLogEntity>();
                if (!string.IsNullOrEmpty(TextToSearch))
                    query = query.Where(x => x.Message.Contains(TextToSearch));
                if (SelectedApp != null)
                    query = query.Where(x => x.AppId.Equals(SelectedApp));
                List<NoSQLLogEntity> entity = query.ToList();
                return ConvertEntityListToDtoList(entity);
            }
        }

        #region Entity to Dto
        internal static LogDto ConvertEntityToDto(NoSQLLogEntity entity)
        {
            LogDto result = new LogDto();
            if (entity != null)
            {
                result.Id = entity.Id;
                result.App = new AppDto { Id = entity.AppId, Name = entity.AppName };
                result.Level = entity.Level;
                result.LogDate = entity.LogDate;
                result.Message = entity.Message;
            }
            return result;
        }

        internal static List<LogDto> ConvertEntityListToDtoList(List<NoSQLLogEntity> entity)
        {
            List<LogDto> result = new List<LogDto>();
            if (entity != null && entity.Count > 0)
            {
                foreach (NoSQLLogEntity en in entity)
                {
                    result.Add(ConvertEntityToDto(en));
                }
            }
            return result;
        }
        #endregion
    }
}
