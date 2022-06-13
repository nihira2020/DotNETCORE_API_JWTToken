using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using CustomerAPI.Entities;
using Microsoft.AspNetCore.Hosting;

namespace CustomerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly Learn_DBContext context;
        private IWebHostEnvironment hostingEnv;
        public ProductController(Learn_DBContext learn_DB, IWebHostEnvironment environment)
        {
            context = learn_DB;
            hostingEnv = environment;
        }
        // GET: api/<CustomerController>
        [HttpGet]
        public IEnumerable<TblProduct> Get()
        {
            return context.TblProduct.ToList();
        }

        [HttpGet("{id}")]
        public TblProduct Get(int id)
        {
            return context.TblProduct.FirstOrDefault(o => o.Code == id);
        }

        [HttpGet("GetProductwithimage")]
        public IEnumerable<Product> GetProductwithimage()
        {
            List<Product> _list = new List<Product>();
            var _product = context.TblProduct.ToList();
            if (_product != null && _product.Count > 0)
            {
                _product.ForEach(item =>
                {
                    _list.Add(new Product()
                    {
                        Code = item.Code,
                        Name = item.Name,
                        Amount = item.Amount,
                        ProductImage = GetImagebycode(item.Code)
                    });

                });

            }
            return _list;
        }

        [HttpGet("GetProductwithimagebycode/{id}")]
        public IEnumerable<Product> GetProductwithimagebycode(int id)
        {
            List<Product> _list = new List<Product>();
            var _product = context.TblProduct.FirstOrDefault(item => item.Code == id);
            if (_product != null)
            {
                string apppath = "https://localhost:44308";
                string imagepath = "/Uploads/Product/" + id;
                string[] filePaths = Directory.GetFiles(hostingEnv.ContentRootPath + "/wwwroot"+imagepath);

                for (int i = 0; i < filePaths.Length; i++)
                {
                    string filename = Path.GetFileName(filePaths[i]);
                    string productimage = apppath + imagepath + "/" + filename;
                    _list.Add(new Product()
                    {
                        Code = _product.Code,
                        Name = _product.Name,
                        Amount = _product.Amount,
                        ProductImage = productimage
                    });
                }

            }
            return _list;
        }

       
        [HttpPost]
        public APIResponse Post([FromBody] TblProduct value)
        {
            string result = string.Empty;
            try
            {
                var _emp = context.TblProduct.FirstOrDefault(o => o.Code == value.Code);
                if (_emp != null)
                {
                    _emp.Code = value.Code;
                    _emp.Amount = value.Amount;
                    _emp.Name = value.Name;
                    context.SaveChanges();
                    result = "pass";
                }
                else
                {
                    TblProduct TblProduct = new TblProduct()
                    {
                        Name = value.Name,
                        Code = value.Code,
                        Amount = value.Amount
                    };
                    context.TblProduct.Add(TblProduct);
                    context.SaveChanges();
                    result = "pass";
                }

            }
            catch (Exception ex)
            {
                result = string.Empty;
            }
            return new APIResponse { keycode = string.Empty, result = result };
        }

        [HttpDelete("{id}")]
        public APIResponse Delete(int id)
        {
            string result = string.Empty;
            var _emp = context.TblProduct.FirstOrDefault(o => o.Code == id);
            if (_emp != null)
            {
                context.TblProduct.Remove(_emp);
                context.SaveChanges();
                result = "pass";
            }
            return new APIResponse { keycode = string.Empty, result = result };
        }

        [HttpPost("UploadImage")]
        public async Task<ActionResult> UploadImage()
        {
            bool Result = false;
            var Files = Request.Form.Files;
            foreach (IFormFile source in Files)
            {
                string FileName = source.FileName;
                string imagepath = GetActualpath(FileName);
                try
                {
                    if (!System.IO.Directory.Exists(imagepath))
                        System.IO.Directory.CreateDirectory(imagepath);

                    string Filepath = imagepath + "\\1.png";

                    if (System.IO.File.Exists(Filepath))
                        System.IO.File.Delete(Filepath);

                    using (FileStream stream = System.IO.File.Create(Filepath))
                    {
                        await source.CopyToAsync(stream);
                        Result = true;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return Ok(Result);

        }
        [HttpGet("RemoveImage/{Code}")]
        public APIResponse RemoveImage(string Code)
        {
            string Result = string.Empty;
            string FileName = Code;
            string imagepath = GetActualpath(FileName);
            try
            {
                
                string Filepath = imagepath + "\\1.png";

                if (System.IO.File.Exists(Filepath))
                    System.IO.File.Delete(Filepath);

                Result = "pass";
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return new APIResponse { keycode = Code, result = Result };

        }

        [NonAction]
        public string GetActualpath(string FileName)
        {
            return hostingEnv.WebRootPath+"\\Uploads\\Product\\" + FileName;
        }

        [NonAction]
        private string GetImagebycode(int Code)
        {
            string hosturl = "https://localhost:44308";
            string mainimagepath = GetActualpath(Code.ToString());
            string Filepath = mainimagepath + "\\1.png";

            if (System.IO.File.Exists(Filepath))
                return hosturl + "/Uploads/Product/" + Code + "/1.png";
            else
                return hosturl + "/Uploads/Common/noimage.png";
        }
       
       
    }
}
