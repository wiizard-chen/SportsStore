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

namespace SportsStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Paginate()
        {
            //arrange
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(p => p.Products).Returns(new Product[]
            {
                new Product {ProductID=1,Name="p1" },
                new Product {ProductID=2,Name="p2" },
                new Product {ProductID=3,Name="p3" },
                new Product {ProductID=4,Name="p4" }
            });
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //action
            IEnumerable<Product> result = (IEnumerable<Product>)controller.List(2).Model;

            //assert
            Product[] proarray = result.ToArray();
            Assert.IsTrue(proarray.Length == 1);
            Assert.AreEqual(proarray[0].Name, "p4");
        }

        [TestMethod]
        public void  Can_Generate_Page_Links()
        {
            //arrange
            HtmlHelper myhelper = null;
            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };
            Func<int, string> pageUrlDelegate = i => "Page" + i;



        }
    }
}
