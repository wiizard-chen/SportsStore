﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportsStore.Domain.Entities;

namespace SportsStore.Domain.Abstract
{
   public interface IProductsRepository
    {
        IEnumerable <Product> Products { get; }

        void SaveProduct(Product product);
    }
}
