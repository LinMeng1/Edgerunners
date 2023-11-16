﻿using Microsoft.AspNetCore.Mvc;
using Moon.Core.Models;
using Moon.Core.Models.Edgerunners;
using Moon.Core.Standard;

namespace Moon.Controllers.Application.NightCity.Modules
{
    [Route("api/application/night-city/modules/product/[action]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> log;
        public ProductController(ILogger<ProductController> _log)
        {
            log = _log;
        }

        #region 获取产品列表
        [HttpGet]
        public ControllersResult GetProductList()
        {
            ControllersResult result = new();
            try
            {
                List<string> products = Database.Edgerunners.Queryable<Product>().OrderBy(it => it.InternalName).Select(it => it.InternalName).ToList();
                result.Content = products;
                result.Result = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = $"Exception : {e.Message}";
                log.LogError(result.ErrorMessage);
            }
            return result;
        }
        #endregion

        #region 获取产品流程
        [HttpGet]
        public ControllersResult GetProductProcessList()
        {
            ControllersResult result = new();
            try
            {
                List<string> productProcesses = Database.Edgerunners.Queryable<ProductProcess>().OrderBy(it => it.Name).Select(it => it.Name).ToList();
                result.Content = productProcesses;
                result.Result = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = $"Exception : {e.Message}";
                log.LogError(result.ErrorMessage);
            }
            return result;
        }
        #endregion
    }
}
