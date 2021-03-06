﻿using RBSectorUWPBusinessLogic.MainServiceClient;
using RBSectorUWPBusinessLogic.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace RBSectorUWPBusinessLogic.Service
{
    public class ProductService
    {
        private Presenter _presenter;
        public ProductViewModel product { get; private set; }
        public static ImageViewModel SelectedImageForProduct { get; set; }
        private ImageService im_srv;
        public ProductService()
        {
            _presenter = Presenter.Instance();
            product = new ProductViewModel();
            im_srv = new ImageService();
        }
        public bool CreateProduct(string name, string price)
        {
            if (!_presenter.CheckNameUnique<ProductViewModel>(name)) return false;
            if (product == null) product = new ProductViewModel();
            product.PR_RECID = GenerateNextProductID;
            product.PR_Name = name;
            decimal product_price;
            if (Decimal.TryParse(price, out product_price))
            {
                product.Price = product_price;
            }
            else
                return false;
            product.TabParent = _presenter.GetSelectedTab();
            product.CategoryParent = _presenter.GetSelectedCategory();
            if (SelectedImageForProduct != null)
            {
                product.Image = SelectedImageForProduct.bitmapImage;
                product.IM_Name = ImageType.IM_Product.ToString() + "_" + product.PR_Name;
                product.IM_Byte = SelectedImageForProduct.BytesImage;
                product.IM_RECID = GenerateNextImageID;
                product.IM_Type = ImageType.IM_Product.ToString();
                product.ByteString = product.IM_Byte.Count() > 0 ? ImageService.ByteToStringForDB(product.IM_Byte) : string.Empty;
            }
            product.Status = STATUS.Created.ToString();
            product.CategoryParent.Products.Add(product);
            SetProductsSingleToBindingModel(product);
            return true;
        }
        public void DeleteProduct(int id)
        {
            if (FindProduct(id))
            {
                if (!STATUS.Created.Equals(product.Status))
                    _presenter.DELETED_ITEM = DELETED_PART.PRODUCT_DELETED + ":" + id;
                _presenter.Products.Remove(product);
            }
        }
        public bool FindProduct(int id)
        {
            product = _presenter.Products.Where(x => x.PR_RECID == id).FirstOrDefault();
            if (product != null) return true;
            return false;
        }
        public ProductViewModel GetProduct(int Recid)
        {
            ProductViewModel product = _presenter.Products.Where(x => x.PR_RECID == Recid).FirstOrDefault();
            return product;
        }
        public bool Update(ProductViewModel editProduct)
        {
            try
            {
                var item = _presenter.Products.FirstOrDefault(x => x.PR_RECID == editProduct.PR_RECID);
                if (item != null)
                {
                    if (SelectedImageForProduct != null)
                    {
                        editProduct.IM_RECID = editProduct.IM_RECID == 0 ? GenerateNextImageID : editProduct.IM_RECID;
                        editProduct.Image = SelectedImageForProduct.bitmapImage;
                        editProduct.IM_Name = ImageType.IM_Product.ToString() + "_" + editProduct.PR_Name;
                        editProduct.IM_Byte = SelectedImageForProduct.BytesImage;
                        editProduct.ByteString = editProduct.IM_Byte.Count() > 0 ? ImageService.ByteToStringForDB(editProduct.IM_Byte) : string.Empty;
                        editProduct.IM_Type = ImageType.IM_Product.ToString();
                    }
                    editProduct.Status = STATUS.Edited.ToString();
                    item = editProduct;
                }
                product = editProduct;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return false;
            }
            return true;
        }
        public void SetProduct(ProductViewModel item)
        {
            this.product = item;
        }
        public int GenerateNextProductID
        {
            get
            {
                var list = GetAllProducts();
                if (list.Count > 0)
                {
                    return list.Select(x => x.PR_RECID).Max() + 1;
                }
                return 1;
            }
        }

        public int GenerateNextImageID
        {
            get
            {
                var list = GetAllProducts();
                if (list.Count > 0)
                    return (list.Select(x => x.IM_RECID).Max() + 1);
                return 1;
            }
        }
        public void SetProductsToBindingModel(ObservableCollection<ProductViewModel> products)
        {
            if (_presenter.Products.Count > 0)
                _presenter.Products.Clear();
            foreach (var item in products)
            {
                if (_presenter.Products.Where(x => x.PR_RECID == item.PR_RECID).FirstOrDefault() == null&&
                    _presenter.Products.Where(x => x.PR_Name == item.PR_Name).FirstOrDefault() == null)
                    _presenter.Products.Add(item);
            }
        }
        public void SetProductsSingleToBindingModel(ProductViewModel product)
        {
            if (_presenter.Products.Where(x => x.PR_RECID == product.PR_RECID).FirstOrDefault() == null)
                _presenter.Products.Add(product);
        }
        public ObservableCollection<ProductViewModel> GetAllProducts()
        {
            ObservableCollection<ProductViewModel> products = new ObservableCollection<ProductViewModel>();
            foreach (var tab in _presenter.Tabs)
                foreach (var cat in tab.Categories)
                {
                    _presenter.AddRange(products, cat.Products);
                }
            return products;
        }
        public ProductViewModel Copy(ProductViewModel product)
        {
            ProductViewModel newProduct = new ProductViewModel();
            newProduct.PR_Name = product.PR_Name;
            newProduct.PR_RECID = product.PR_RECID;
            newProduct.Products = product.Products;
            newProduct.Price = product.Price;
            newProduct.Image = product.Image;
            newProduct.IM_Byte = product.IM_Byte;
            newProduct.IM_Name = product.IM_Name;
            newProduct.IM_RECID = product.IM_RECID;
            newProduct.IM_Type = product.IM_Type;
            return newProduct;
        }
    }
}
