using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Concrete;
using SportsStore.Domain.Entities;
using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Mvc;
using SportsStore.WebUI.Models;
using SportsStore.WebUI.HtmlHelpers;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class AdminTest
    {
        [TestMethod]
        public void Index_Contains_All_Products()
        {
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(p => p.Products).Returns(new Product[]
            {
                new Product {ProductID=1,Name="p1" },
                new Product {ProductID=2,Name="p2" },
                new Product {ProductID=3,Name="p3" }
            });

            AdminController target = new AdminController(mock.Object);

            Product[] result = ((IEnumerable<Product>)target.Index().ViewData.Model).ToArray();

            Assert.AreEqual(result.Length, 3);
            Assert.AreEqual("p1", result[0].Name);
            Assert.AreEqual("p2", result[1].Name);
            Assert.AreEqual("p3", result[2].Name);

        }

        [TestMethod]
        public void Can_Edit_Product()
        {
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(p => p.Products).Returns(new Product[]
            {
                new Product {ProductID=1,Name="p1" },
                new Product {ProductID=2,Name="p2" },
                new Product {ProductID=3,Name="p3" }
            });

            AdminController target = new AdminController(mock.Object);

            Product p1 = target.Edit(1).ViewData.Model as Product;
            Product p2 = target.Edit(2).ViewData.Model as Product;
            Product p3 = target.Edit(3).ViewData.Model as Product;

            Assert.AreEqual(1, p1.ProductID);
            Assert.AreEqual(2, p2.ProductID);
            Assert.AreEqual(3, p3.ProductID);

        }

        [TestMethod]
        public void Cannot_Edit_Product()
        {
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(p => p.Products).Returns(new Product[]
            {
                new Product {ProductID=1,Name="p1" },
                new Product {ProductID=2,Name="p2" },
                new Product {ProductID=3,Name="p3" }
            });

            AdminController target = new AdminController(mock.Object);

            Product result = (Product)target.Edit(4).ViewData.Model;

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Can_Save_Valid_Changes()
        {
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            AdminController target = new AdminController(mock.Object);
            Product product = new Product { Name = "Test" };
            //尝试保存一下这个产品
            ActionResult result = target.Edit(product);
            //断言一检查，调用存储库
            mock.Verify(m => m.SaveProduct(product));
            //断言一检查方法的结果类型
            Assert.IsNotInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Cannot_Save_Invalid_Changes()
        {
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            AdminController target = new AdminController(mock.Object);
            Product product = new Product { Name = "Test" };
            target.ModelState.AddModelError("error", "error");

            ActionResult result = target.Edit(product);

            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Can_Delete_Valid_Products()
        {
            Product prod = new Product { ProductID = 2, Name = "Test" };
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID=1,Name="p1" },
                prod,
                new Product {ProductID=3,Name="p3" }
            });
            AdminController target = new AdminController(mock.Object);
            target.Delete(prod.ProductID);

            mock.Verify(m => m.DeleteProduct(prod.ProductID));
        }

        [TestMethod]
        public void Can_Retrieve_Image_Data()
        {
            Product prod = new Product
            {
                ProductID = 2,
                Name = "Test",
                ImageData = new byte[] { },
                ImageMimeType = "image/png"
            };

            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID=1,Name="p1" },
                prod,
                new Product {ProductID=3,Name="p3" }
            }.AsQueryable());

            ProductController target = new ProductController(mock.Object);

            ActionResult result = target.GetImage(2);

            Assert.IsNull(result);
            Assert.IsInstanceOfType(result, typeof(FileResult));
            Assert.AreEqual(prod.ImageMimeType, ((FileResult)result).ContentType);

        }

        [TestMethod]
        public void Cannot_Retrieve_Image_Data()
        {
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID=1,Name="p1" },
                new Product {ProductID=2,Name="p2" }
            }.AsQueryable());

            ProductController target = new ProductController(mock.Object);

            ActionResult result = target.GetImage(100);

            Assert.IsNull(result);
        }
    }
}
