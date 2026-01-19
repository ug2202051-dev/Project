using Microsoft.EntityFrameworkCore;
using ShopSana.Data;
using ShopSana.Models.Entities;
using ShopSana.Models.ViewModels;

namespace ShopSana.Services
{
    public interface IProductService
    {
        Task<ProductListViewModel> GetProductsAsync(int? categoryId, string? searchTerm, string? sortBy, int page, int pageSize);
        Task<Product?> GetProductByIdAsync(int id);
        Task<IEnumerable<Product>> GetFeaturedProductsAsync(int count = 8);
        Task<IEnumerable<Product>> GetNewArrivalsAsync(int count = 8);
        Task<IEnumerable<Product>> GetRelatedProductsAsync(int productId, int count = 4);
        Task<Product> CreateProductAsync(ProductFormViewModel model);
        Task<Product> UpdateProductAsync(ProductFormViewModel model);
        Task<bool> DeleteProductAsync(int id);
        Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 10);
    }

    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProductListViewModel> GetProductsAsync(int? categoryId, string? searchTerm, string? sortBy, int page, int pageSize)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive);

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(term) ||
                                        (p.Description != null && p.Description.ToLower().Contains(term)) ||
                                        (p.Brand != null && p.Brand.ToLower().Contains(term)));
            }

            query = sortBy switch
            {
                "price_asc" => query.OrderBy(p => p.DiscountPrice ?? p.Price),
                "price_desc" => query.OrderByDescending(p => p.DiscountPrice ?? p.Price),
                "name_asc" => query.OrderBy(p => p.Name),
                "name_desc" => query.OrderByDescending(p => p.Name),
                "newest" => query.OrderByDescending(p => p.CreatedAt),
                _ => query.OrderByDescending(p => p.IsFeatured).ThenByDescending(p => p.CreatedAt)
            };

            var totalProducts = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();

            return new ProductListViewModel
            {
                Products = products,
                Categories = categories,
                SelectedCategoryId = categoryId,
                SearchTerm = searchTerm,
                SortBy = sortBy,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalProducts = totalProducts
            };
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
        }

        public async Task<IEnumerable<Product>> GetFeaturedProductsAsync(int count = 8)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive && p.IsFeatured)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetNewArrivalsAsync(int count = 8)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetRelatedProductsAsync(int productId, int count = 4)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return new List<Product>();

            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive && p.Id != productId && p.CategoryId == product.CategoryId)
                .OrderByDescending(p => p.IsFeatured)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Product> CreateProductAsync(ProductFormViewModel model)
        {
            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                DiscountPrice = model.DiscountPrice,
                ImageUrl = model.ImageUrl,
                StockQuantity = model.StockQuantity,
                SKU = model.SKU,
                Brand = model.Brand,
                IsActive = model.IsActive,
                IsFeatured = model.IsFeatured,
                CategoryId = model.CategoryId
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateProductAsync(ProductFormViewModel model)
        {
            var product = await _context.Products.FindAsync(model.Id);
            if (product == null)
                throw new ArgumentException("Product not found");

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.DiscountPrice = model.DiscountPrice;
            product.ImageUrl = model.ImageUrl;
            product.StockQuantity = model.StockQuantity;
            product.SKU = model.SKU;
            product.Brand = model.Brand;
            product.IsActive = model.IsActive;
            product.IsFeatured = model.IsFeatured;
            product.CategoryId = model.CategoryId;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 10)
        {
            return await _context.Products
                .Where(p => p.IsActive && p.StockQuantity <= threshold)
                .OrderBy(p => p.StockQuantity)
                .ToListAsync();
        }
    }
}
