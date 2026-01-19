using Microsoft.AspNetCore.Identity;
using ShopSana.Models.Entities;

namespace ShopSana.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Seed Roles
            await SeedRoles(roleManager);

            // Seed Admin User
            await SeedAdminUser(userManager);

            // Seed Categories
            await SeedCategories(context);

            // Seed Products
            await SeedProducts(context);
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Customer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task SeedAdminUser(UserManager<ApplicationUser> userManager)
        {
            var adminEmail = "admin@shopsana.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }

        private static async Task SeedCategories(ApplicationDbContext context)
        {
            if (context.Categories.Any())
                return;

            var categories = new List<Category>
            {
                new Category
                {
                    Name = "Electronics",
                    Description = "Latest gadgets and electronic devices including smartphones, laptops, tablets, and accessories.",
                    ImageUrl = "https://images.unsplash.com/photo-1498049794561-7780e7231661?w=400",
                    IsActive = true
                },
                new Category
                {
                    Name = "Clothing",
                    Description = "Trendy fashion apparel for men, women, and kids. From casual wear to formal attire.",
                    ImageUrl = "https://images.unsplash.com/photo-1445205170230-053b83016050?w=400",
                    IsActive = true
                },
                new Category
                {
                    Name = "Home & Garden",
                    Description = "Everything you need for your home and garden. Furniture, decor, and gardening supplies.",
                    ImageUrl = "https://images.unsplash.com/photo-1484154218962-a197022b5858?w=400",
                    IsActive = true
                },
                new Category
                {
                    Name = "Sports & Outdoors",
                    Description = "Sports equipment, outdoor gear, and fitness accessories for an active lifestyle.",
                    ImageUrl = "https://images.unsplash.com/photo-1461896836934- voices-media?w=400",
                    IsActive = true
                },
                new Category
                {
                    Name = "Books & Stationery",
                    Description = "Books across all genres, office supplies, and stationery items.",
                    ImageUrl = "https://images.unsplash.com/photo-1524995997946-a1c2e315a42f?w=400",
                    IsActive = true
                }
            };

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
        }

        private static async Task SeedProducts(ApplicationDbContext context)
        {
            if (context.Products.Any())
                return;

            var electronics = context.Categories.FirstOrDefault(c => c.Name == "Electronics");
            var clothing = context.Categories.FirstOrDefault(c => c.Name == "Clothing");
            var homeGarden = context.Categories.FirstOrDefault(c => c.Name == "Home & Garden");
            var sports = context.Categories.FirstOrDefault(c => c.Name == "Sports & Outdoors");
            var books = context.Categories.FirstOrDefault(c => c.Name == "Books & Stationery");

            var products = new List<Product>
            {
                // Electronics
                new Product
                {
                    Name = "Wireless Bluetooth Headphones",
                    Description = "Premium noise-cancelling wireless headphones with 30-hour battery life. Features include Active Noise Cancellation, Bluetooth 5.0, and comfortable over-ear design.",
                    Price = 199.99m,
                    DiscountPrice = 149.99m,
                    ImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=400",
                    StockQuantity = 50,
                    SKU = "ELEC-001",
                    Brand = "AudioMax",
                    IsFeatured = true,
                    CategoryId = electronics!.Id
                },
                new Product
                {
                    Name = "Smart Watch Pro",
                    Description = "Advanced smartwatch with health monitoring, GPS tracking, and water resistance. Compatible with iOS and Android devices.",
                    Price = 349.99m,
                    DiscountPrice = 299.99m,
                    ImageUrl = "https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=400",
                    StockQuantity = 30,
                    SKU = "ELEC-002",
                    Brand = "TechWear",
                    IsFeatured = true,
                    CategoryId = electronics.Id
                },
                new Product
                {
                    Name = "4K Ultra HD Webcam",
                    Description = "Professional-grade webcam with 4K resolution, auto-focus, and built-in ring light. Perfect for streaming and video calls.",
                    Price = 129.99m,
                    ImageUrl = "https://images.unsplash.com/photo-1587826080692-f439cd0b70da?w=400",
                    StockQuantity = 75,
                    SKU = "ELEC-003",
                    Brand = "StreamPro",
                    IsFeatured = false,
                    CategoryId = electronics.Id
                },

                // Clothing
                new Product
                {
                    Name = "Classic Cotton T-Shirt",
                    Description = "Premium quality 100% cotton t-shirt. Soft, comfortable, and perfect for everyday wear. Available in multiple colors.",
                    Price = 29.99m,
                    DiscountPrice = 24.99m,
                    ImageUrl = "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=400",
                    StockQuantity = 200,
                    SKU = "CLOTH-001",
                    Brand = "ComfortWear",
                    IsFeatured = true,
                    CategoryId = clothing!.Id
                },
                new Product
                {
                    Name = "Slim Fit Denim Jeans",
                    Description = "Modern slim fit jeans made with premium stretch denim. Comfortable all-day wear with classic styling.",
                    Price = 79.99m,
                    DiscountPrice = 59.99m,
                    ImageUrl = "https://images.unsplash.com/photo-1542272604-787c3835535d?w=400",
                    StockQuantity = 100,
                    SKU = "CLOTH-002",
                    Brand = "DenimCo",
                    IsFeatured = true,
                    CategoryId = clothing.Id
                },
                new Product
                {
                    Name = "Winter Jacket",
                    Description = "Warm and stylish winter jacket with water-resistant outer shell and soft fleece lining. Multiple pockets for convenience.",
                    Price = 149.99m,
                    ImageUrl = "https://images.unsplash.com/photo-1591047139829-d91aecb6caea?w=400",
                    StockQuantity = 45,
                    SKU = "CLOTH-003",
                    Brand = "OutdoorStyle",
                    IsFeatured = false,
                    CategoryId = clothing.Id
                },

                // Home & Garden
                new Product
                {
                    Name = "Modern Floor Lamp",
                    Description = "Contemporary LED floor lamp with adjustable brightness and color temperature. Sleek design complements any room.",
                    Price = 89.99m,
                    DiscountPrice = 69.99m,
                    ImageUrl = "https://images.unsplash.com/photo-1507473885765-e6ed057f782c?w=400",
                    StockQuantity = 40,
                    SKU = "HOME-001",
                    Brand = "LightWorks",
                    IsFeatured = true,
                    CategoryId = homeGarden!.Id
                },
                new Product
                {
                    Name = "Indoor Plant Set (3 Pack)",
                    Description = "Beautiful set of three low-maintenance indoor plants in decorative ceramic pots. Perfect for home or office.",
                    Price = 49.99m,
                    ImageUrl = "https://images.unsplash.com/photo-1459411552884-841db9b3cc2a?w=400",
                    StockQuantity = 25,
                    SKU = "HOME-002",
                    Brand = "GreenLife",
                    IsFeatured = false,
                    CategoryId = homeGarden.Id
                },

                // Sports & Outdoors
                new Product
                {
                    Name = "Professional Yoga Mat",
                    Description = "Extra thick eco-friendly yoga mat with non-slip surface. Perfect for yoga, pilates, and floor exercises.",
                    Price = 39.99m,
                    DiscountPrice = 34.99m,
                    ImageUrl = "https://images.unsplash.com/photo-1544367567-0f2fcb009e0b?w=400",
                    StockQuantity = 80,
                    SKU = "SPORT-001",
                    Brand = "FitZone",
                    IsFeatured = true,
                    CategoryId = sports!.Id
                },
                new Product
                {
                    Name = "Running Shoes - Men",
                    Description = "Lightweight breathable running shoes with cushioned sole for maximum comfort. Ideal for road running and gym workouts.",
                    Price = 119.99m,
                    DiscountPrice = 99.99m,
                    ImageUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=400",
                    StockQuantity = 60,
                    SKU = "SPORT-002",
                    Brand = "SpeedRunner",
                    IsFeatured = true,
                    CategoryId = sports.Id
                },

                // Books & Stationery
                new Product
                {
                    Name = "Premium Notebook Set",
                    Description = "Set of 3 hardcover notebooks with 200 pages each. Features lay-flat binding and premium paper.",
                    Price = 24.99m,
                    DiscountPrice = 19.99m,
                    ImageUrl = "https://images.unsplash.com/photo-1531346878377-a5be20888e57?w=400",
                    StockQuantity = 150,
                    SKU = "BOOK-001",
                    Brand = "WriteWell",
                    IsFeatured = false,
                    CategoryId = books!.Id
                },
                new Product
                {
                    Name = "Art Supply Kit",
                    Description = "Complete art supply kit including colored pencils, markers, watercolors, and sketchbook. Perfect for beginners and artists.",
                    Price = 59.99m,
                    ImageUrl = "https://images.unsplash.com/photo-1513364776144-60967b0f800f?w=400",
                    StockQuantity = 35,
                    SKU = "BOOK-002",
                    Brand = "ArtMaster",
                    IsFeatured = false,
                    CategoryId = books.Id
                }
            };

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }
    }
}
