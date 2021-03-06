﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Ixq.Core.Dto;
using Ixq.Core.Entity;
using Ixq.Core.Repository;
using Ixq.Extensions;
using Ixq.UI;
using Ixq.UI.ComponentModel.DataAnnotations;
using Ixq.UI.Controls;
using Newtonsoft.Json;

namespace Ixq.Web.Mvc
{
    /// <summary>
    ///     实体控制器。
    /// </summary>
    /// <typeparam name="TEntity">实体。</typeparam>
    /// <typeparam name="TDto">数据传输对象。</typeparam>
    /// <typeparam name="TKey">实体主键类型。</typeparam>
    public abstract class EntityController<TEntity, TDto, TKey> : BaseController, IEntityControllerDescriptor
        where TEntity : class, IEntity<TKey>, new()
        where TDto : class, IDto<TEntity, TKey>, new()
    {
        /// <summary>
        ///     实体仓储。
        /// </summary>
        public readonly IRepositoryBase<TEntity, TKey> Repository;

        private IEntityMetadataProvider _entityMetadataProvider;

        /// <summary>
        ///     初始化一个<see cref="EntityController{TEntity, TDto, TKey}" />对象。
        /// </summary>
        /// <param name="repository">实体仓储。</param>
        protected EntityController(IRepositoryBase<TEntity, TKey> repository)
        {
            PageSizeList = new[] {15, 30, 60, 120};
            PageConfig = typeof(TDto).GetAttribute<PageAttribute>() ??
                         new PageAttribute();
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            EntityMetadata = EntityMetadataProvider.GetEntityMetadata(typeof(TDto));
        }

        /// <summary>
        ///     获取或设置实体服务。
        /// </summary>
        public IEntityService<TEntity, TDto, TKey> EntityService { get; set; }

        /// <summary>
        ///     获取或设置实体元数据提供者。
        /// </summary>
        public IEntityMetadataProvider EntityMetadataProvider
        {
            get => _entityMetadataProvider ?? (_entityMetadataProvider = CreateEntityMetadataProvider());
            set => _entityMetadataProvider = value;
        }

        /// <summary>
        ///     获取或设置页面大小集合。
        /// </summary>
        public int[] PageSizeList { get; set; }

        /// <summary>
        ///     获取或设置页面配置信息。
        /// </summary>
        public IPageConfig PageConfig { get; set; }

        /// <summary>
        ///     获取或设置实体元数据。
        /// </summary>
        public IEntityMetadata EntityMetadata { get; set; }

        /// <summary>
        ///     Index 操作。
        /// </summary>
        /// <param name="orderField">排序字段。</param>
        /// <param name="orderDirection">排序方向。</param>
        /// <param name="pageSize">页面大小。</param>
        /// <param name="pageCurrent">当前页。</param>
        /// <returns></returns>
        public virtual ActionResult Index(string orderField, string orderDirection,
            int pageSize = 30, int pageCurrent = 1)
        {
            if (pageCurrent < 1)
                pageCurrent = 1;
            if (pageSize < 1)
                pageSize = PageSizeList[0];

            orderField = string.IsNullOrWhiteSpace(orderField) ? PageConfig.DefaultSortname ?? "Id" : orderField;
            orderDirection = string.IsNullOrWhiteSpace(orderDirection)
                ? PageConfig.IsDescending
                    ? "desc"
                    : "asc"
                : orderDirection;

            var pagination = new Pagination
            {
                PageSize = pageSize,
                PageCurrent = pageCurrent,
                PageSizeList = PageSizeList,
                DefualtPageSize = PageSizeList[0],
                OrderField = orderField,
                OrderDirection = orderDirection
            };

            var pageViewModel = EntityService.CreateIndexModel(pagination);
            return View(pageViewModel);
        }

        /// <summary>
        ///     List 操作。
        /// </summary>
        /// <param name="orderField">排序字段。</param>
        /// <param name="orderDirection">排序方向。</param>
        /// <param name="pageSize">页面大小。</param>
        /// <param name="pageCurrent">当前页。</param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<ActionResult> List(string orderField, string orderDirection,
            int pageSize = 30, int pageCurrent = 1)
        {
            if (pageCurrent < 1)
                pageCurrent = 1;
            if (pageSize < 1)
                pageSize = PageSizeList[0];

            orderField = string.IsNullOrWhiteSpace(orderField) ? PageConfig.DefaultSortname ?? "Id" : orderField;
            orderDirection = string.IsNullOrWhiteSpace(orderDirection)
                ? PageConfig.IsDescending
                    ? "desc"
                    : "asc"
                : orderDirection;

            var pageListViewModel = await EntityService.CreateListModelAsync(pageSize, pageCurrent, orderField,
                orderDirection);

            return Json(pageListViewModel, new JsonSerializerSettings {DateFormatString = "yyyy-MM-dd HH:mm:ss"});
        }

        /// <summary>
        ///     Detail 操作。
        /// </summary>
        /// <param name="id">实体主键。</param>
        /// <returns></returns>
        public virtual async Task<ActionResult> Detail(string id)
        {
            var detailModel = await EntityService.CreateDetailModelAsync(id);
            return View(detailModel);
        }

        /// <summary>
        ///     Edit 操作。
        /// </summary>
        /// <param name="id">实体主键。</param>
        /// <returns></returns>
        public virtual async Task<ActionResult> Edit(string id)
        {
            var editModel = await EntityService.CreateEditModelAsync(id);
            return View(editModel);
        }

        /// <summary>
        ///     Edit 操作。
        /// </summary>
        /// <param name="model">数据传输对象模型。</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Edit([ModelBinder(typeof(EntityModelBinder))] TDto model)
        {
            var viewModel = await EntityService.CreateEditModelAsync(model);
            if (ModelState.IsValid)
            {
                await EntityService.UpdateEntity(model.MapTo());
                return PartialView("_Form", viewModel);
            }
            Response.StatusCode = 500;
            Response.TrySkipIisCustomErrors = true;
            return PartialView("_Form", viewModel);
        }

        /// <summary>
        ///     Delete 操作。
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Delete(IEnumerable<TKey> range)
        {
            var suc = await EntityService.RemoveRange(range);
            return Json(new RequestResult {Success = suc, ErrorCode = 0, ErrorMessage = ""});
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public virtual ActionResult Selector()
        {
            return View();
        }

        /// <summary>
        ///     对调用构造函数时可能不可用的数据进行初始化。
        /// </summary>
        /// <param name="requestContext">HTTP 上下文和路由数据。</param>
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            CreateEntityService(requestContext);
        }

        /// <summary>
        ///     创建实体元数据提供者，默认提供<see cref="Ixq.Web.Mvc.EntityMetadataProvider" />。可在派生类中重写。
        /// </summary>
        /// <returns></returns>
        protected virtual IEntityMetadataProvider CreateEntityMetadataProvider()
        {
            return DependencyResolver.Current.GetService<IEntityMetadataProvider>() ??
                   new EntityMetadataProvider();
        }

        /// <summary>
        ///     创建实体服务，默认提供 <see cref="EntityService{TEntity, TDto, TKey}"/>。
        /// </summary>
        /// <param name="requestContext"></param>
        protected virtual void CreateEntityService(RequestContext requestContext)
        {
            EntityService = new EntityService<TEntity, TDto, TKey>(Repository, requestContext, this);
        }

        #region ModelErrorResult

        /// <summary>
        ///     创建一个 <see cref="ModelErrorResult" /> 对象，将模型错误信息序列化成Json对象。
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        protected virtual ModelErrorResult ModelError(ModelStateDictionary modelState)
        {
            return new ModelErrorResult(modelState);
        }

        /// <summary>
        ///     创建一个 <see cref="ModelErrorResult" /> 对象，将模型错误信息序列化成Json对象。
        /// </summary>
        /// <returns></returns>
        protected virtual ModelErrorResult ModelError()
        {
            return ModelError(ModelState);
        }

        #endregion

        #region JsonResult

        /// <summary>
        ///     创建一个<see cref="JsonReader" />对象，将指定的对象序列化为JavaScript Object Notation（JSON）。
        /// </summary>
        /// <param name="data">要序列化的对象。</param>
        /// <param name="serializerSettings">设置。</param>
        /// <returns></returns>
        protected virtual JsonResult Json(object data, JsonSerializerSettings serializerSettings)
        {
            return Json(data, null, null, JsonRequestBehavior.DenyGet, serializerSettings);
        }

        /// <summary>
        ///     创建一个<see cref="JsonReader" />对象，将指定的对象序列化为JavaScript Object Notation（JSON）。
        /// </summary>
        /// <param name="data">要序列化的对象。</param>
        /// <param name="contentType">内容类型（MIME类型）。</param>
        /// <param name="contentEncoding">内容编码。</param>
        /// <returns></returns>
        protected override System.Web.Mvc.JsonResult Json(object data, string contentType, Encoding contentEncoding)
        {
            return Json(data, contentType, contentEncoding, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        ///     创建一个<see cref="JsonReader" />对象，将指定的对象序列化为JavaScript Object Notation（JSON）。
        /// </summary>
        /// <param name="data">要序列化的对象。</param>
        /// <param name="contentType">内容类型（MIME类型）。</param>
        /// <param name="contentEncoding">内容编码。</param>
        /// <param name="behavior">JSON请求行为。</param>
        /// <returns></returns>
        protected override System.Web.Mvc.JsonResult Json(object data, string contentType, Encoding contentEncoding,
            JsonRequestBehavior behavior)
        {
            return Json(data, contentType, contentEncoding, JsonRequestBehavior.DenyGet, null);
        }

        /// <summary>
        ///     创建一个<see cref="JsonReader" />对象，将指定的对象序列化为JavaScript Object Notation（JSON）。
        /// </summary>
        /// <param name="data">要序列化的对象。</param>
        /// <param name="contentType">内容类型（MIME类型）。</param>
        /// <param name="contentEncoding">内容编码。</param>
        /// <param name="behavior">JSON请求行为。</param>
        /// <param name="serializerSettings">设置。</param>
        /// <returns></returns>
        protected virtual JsonResult Json(object data, string contentType, Encoding contentEncoding,
            JsonRequestBehavior behavior,
            JsonSerializerSettings serializerSettings)
        {
            return new JsonResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                SerializerSettings = serializerSettings
            };
        }

        #endregion
    }
}