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
    public class Display
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
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;

            //assert
            Product[] proarray = result.Products.ToArray();
            Assert.IsTrue(proarray.Length == 1);
            Assert.AreEqual(proarray[0].Name, "p4");
        }

        [TestMethod]
        public void Can_Generate_Page_Links()
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

            //action
            MvcHtmlString result = myhelper.PageLinks(pagingInfo, pageUrlDelegate);

            //assert
            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1</a>" + @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>"
+ @"<a class=""btn btn-default"" href=""Page3"">3</a>", result.ToString());

        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(p => p.Products).Returns(new Product[]
            {
                new Product {ProductID=1,Name="p1" },
                new Product {ProductID=2,Name="p2" },
                new Product {ProductID=3,Name="p3" },
                new Product {ProductID=4,Name="p4" },
                new Product {ProductID=5,Name="p5" }
            });

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;

            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);

        }

        [TestMethod]
        public void Can_Filter_Products()
        {
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(p => p.Products).Returns(new Product[]
            {
                new Product {ProductID=1,Name="p1",Category="Cat1" },
                new Product {ProductID=2,Name="p2",Category="Cat2" },
                new Product {ProductID=3,Name="p3",Category="Cat1"},
                new Product {ProductID=4,Name="p4",Category="Cat2" },
                new Product {ProductID=5,Name="p5" ,Category="Cat3"}
            });

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 4;

            Product[] result = ((ProductsListViewModel)controller.List("Cat2", 1).Model).Products.ToArray();

            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Name == "p2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "p4" && result[1].Category == "Cat2");

        }
    }
}
