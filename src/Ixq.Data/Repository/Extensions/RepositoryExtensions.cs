﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Ixq.Core.Dto;
using Ixq.Core.Entity;
using Ixq.Core.Repository;
using Ixq.Core.Mapper;

namespace Ixq.Data.Repository.Extensions
{
    /// <summary>
    ///     仓储扩展。
    /// </summary>
    public static class RepositoryExtensions
    {

        /// <summary>
        ///     获取上下文。
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="repository"></param>
        /// <returns></returns>
        public static DbContext GetDbContext<TEntity, TKey>(this IRepositoryBase<TEntity, TKey> repository)
            where TEntity : class, IEntity<TKey>, new()
        {
            if (repository.UnitOfWork == null)
                throw new ArgumentNullException(nameof(repository.UnitOfWork), "未初始化工作单元");
            return (DbContext) repository.UnitOfWork;
        }

        /// <summary>
        ///     获取上下文。
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TDbContext"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="repository"></param>
        /// <returns></returns>
        public static TDbContext GetDbContext<TEntity, TDbContext, TKey>(this IRepositoryBase<TEntity, TKey> repository)
            where TEntity : class, IEntity<TKey>, new()
            where TDbContext : DbContext
        {
            if (repository.UnitOfWork == null)
                throw new ArgumentNullException(nameof(repository.UnitOfWork), "未初始化工作单元");
            return (TDbContext) repository.UnitOfWork;
        }


        /// <summary>
        /// 将指定的 <see cref="IQueryable{TEntity}"/> 转为 <see cref="IDto"/>
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static TDto[] ToDtoArray<TDto, TEntity, TKey>(this IQueryable<TEntity> queryable)
            where TEntity : class, IEntity<TKey>, new()
            where TDto : class, IDto<TEntity, TKey>, new()
        {
            var entityColl = queryable.ToList();
            return entityColl.Select(item => item.MapToDto<TDto, TKey>()).ToArray();
        }

        /// <summary>
        /// 将指定的 <see cref="IQueryable{TEntity}"/> 转为 <see cref="IDto"/>
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static Task<TDto[]> ToDtoArrayAsync<TDto, TEntity, TKey>(this IQueryable<TEntity> queryable)
            where TEntity : class, IEntity<TKey>, new()
            where TDto : class, IDto<TEntity, TKey>, new()
        {
            return Task.FromResult(ToDtoArray<TDto, TEntity, TKey>(queryable));
        }

        /// <summary>
        /// 将 <see cref="IQueryable{TEntity}"/> 转为 <see cref="List{TDto}"/>
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static List<TDto> ToDtoList<TDto, TEntity, TKey>(this IQueryable<TEntity> queryable)
            where TEntity : class, IEntity<TKey>, new()
            where TDto : class, IDto<TEntity, TKey>, new()
        {
            var entityColl = queryable.ToList();
            return entityColl.Select(item => item.MapToDto<TDto, TKey>()).ToList();
        }

        /// <summary>
        /// 将 <see cref="IQueryable{TEntity}"/> 转为 <see cref="List{TDto}"/>
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static Task<List<TDto>> ToDtoListAsync<TDto, TEntity, TKey>(this IQueryable<TEntity> queryable)
            where TEntity : class, IEntity<TKey>, new()
            where TDto : class, IDto<TEntity, TKey>, new()
        {
            return Task.FromResult(ToDtoList<TDto, TEntity, TKey>(queryable));
        }

        public static object ParseEntityKey<TKey>(string value)
        {
            var type = typeof(TKey);

            dynamic resultValue = value;
            if (type == typeof(Guid))
            {
                resultValue = Guid.Parse(value);
            }
            else if (type == typeof(int))
            {
                resultValue = Convert.ToInt32(value);
            }
            else if (type == typeof(short))
            {
                resultValue = Convert.ToInt16(value);
            }
            else if (type == typeof(long))
            {
                resultValue = Convert.ToInt64(value);
            }

            return resultValue;
        }
    }
}