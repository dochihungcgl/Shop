﻿using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Shop.Api.Infrastructure.Core;
using Shop.Api.Models;
using Shop.Model.Models;
using Shop.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Shop.Api.Controllers
{
    [RoutePrefix("api/productcategory")]
    public class ProductCategoryController : ApiControllerBase
    {
        IProductCategoryService _productCategoryService;
        public ProductCategoryController(IErrorService errorService, IProductCategoryService productCategoryService) : base(errorService)
        {
            this._productCategoryService = productCategoryService;
        }

        [Route("getallparents")]
        [HttpGet]
        public HttpResponseMessage GetAll(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                var listProductCategory = _productCategoryService.GetAll();

                var listProductCategoryViewModel = Mapper.Map<List<ProductCategoryViewModel>>(listProductCategory);

                HttpResponseMessage response = request.CreateResponse(HttpStatusCode.OK, listProductCategoryViewModel);
                return response;
            });
        }

        [Route("getall")]
        [HttpGet]
        public HttpResponseMessage GetAll(HttpRequestMessage request, string keyword, int page, int pageSize = 10)
        {
            HttpResponseMessage response = null;
            return CreateHttpResponse(request, () =>
            {
                int totalRow = 0;

                var listProductCategories = _productCategoryService.GetAll(keyword);

                totalRow = listProductCategories.Count();

                var query = listProductCategories.OrderByDescending(x => x.CreatedDate).Skip(page * pageSize).Take(pageSize);

                var listProductViewModel = Mapper.Map<List<ProductCategoryViewModel>>(query);

                var paginationSet = new PaginationSet<ProductCategoryViewModel>()
                {
                    Items = listProductViewModel,
                    Page = page,
                    TotalCount = totalRow,
                    TotalPages = (int)Math.Ceiling((decimal)totalRow / pageSize)
                };

                response = request.CreateResponse(HttpStatusCode.OK, paginationSet);
                return response;
            });
        }

        [Route("getbyid/{id:int}")]
        [HttpGet]
        public HttpResponseMessage GetById(HttpRequestMessage request, int id)
        {
            HttpResponseMessage response = null;

            return CreateHttpResponse(request, () =>
            {
                var productCategory = _productCategoryService.GetById(id);

                var productCategoryViewModel = Mapper.Map<ProductCategoryViewModel>(productCategory);

                response = request.CreateResponse(HttpStatusCode.OK, productCategoryViewModel);

                return response;
            });
        }

        [Route("create")]
        [HttpPost]
        public HttpResponseMessage Create(HttpRequestMessage request, ProductCategoryViewModel productCategoryViewModel)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
                    return response;
                }
                var newProductCategory = new ProductCategory();

                newProductCategory = Mapper.Map<ProductCategory>(productCategoryViewModel);

                newProductCategory.CreatedDate = DateTime.Now;
                newProductCategory.CreatedBy = "admin";

                _productCategoryService.Add(newProductCategory);
                _productCategoryService.SaveChanges();

                response = request.CreateResponse(HttpStatusCode.Created, productCategoryViewModel);
                return response;

            });
        }

        [Route("update")]
        [HttpPut]
        public HttpResponseMessage Update(HttpRequestMessage request, ProductCategoryViewModel productCategoryViewModel)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
                    return response;
                }

                var dbProductCategory = _productCategoryService.GetById(productCategoryViewModel.Id);


                dbProductCategory = Mapper.Map<ProductCategory>(productCategoryViewModel);
                dbProductCategory.UpdatedDate = DateTime.Now;
                dbProductCategory.UpdatedBy = "admin";

                _productCategoryService.Update(dbProductCategory);
                _productCategoryService.SaveChanges();

                productCategoryViewModel = Mapper.Map<ProductCategoryViewModel>(dbProductCategory);

                response = request.CreateResponse(HttpStatusCode.Created, productCategoryViewModel);

                return response;
            });
        }


        public HttpResponseMessage Delete(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var oldProductCategory = _productCategoryService.Delete(id);
                _productCategoryService.SaveChanges();

                var productCategoryViewModel = Mapper.Map<ProductCategoryViewModel>(oldProductCategory);

                response = request.CreateResponse(HttpStatusCode.OK, productCategoryViewModel);
                return response;
            });
        }
    }
}