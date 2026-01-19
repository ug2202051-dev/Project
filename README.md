# ShopSana – An Online Shopping Management System

## 📘 Project Overview

ShopSana is a modern e-commerce web application developed using **ASP.NET Core MVC (.NET 8.0)** with **MySQL database (via XAMPP)**. The project demonstrates a complete online shopping solution where customers can browse products, add items to cart, place orders, and manage their accounts securely.

### Features

#### Customer Features
- ✅ User Registration and Login (ASP.NET Core Identity)
- ✅ Browse Products by Category
- ✅ Search Products
- ✅ Add to Cart & Update Cart
- ✅ Place Orders
- ✅ View Order History
- ✅ User Profile Management

#### Admin Features
- ✅ Admin Dashboard with Analytics
- ✅ Product Management (Add, Update, Delete)
- ✅ Category Management
- ✅ Order Management & Status Updates
- ✅ User Management

---

## 🛠️ Technology Stack

| Component | Technology |
|-----------|------------|
| Framework | ASP.NET Core MVC (.NET 8.0) |
| Language | C# |
| Database | MySQL (XAMPP) |
| ORM | Entity Framework Core |
| Authentication | ASP.NET Core Identity |
| Frontend | Razor Views, Bootstrap 5, Bootstrap Icons |
| Architecture | MVC Pattern (Model-View-Controller) |

---

## 📁 Project Structure

```
ShopSana/
├── Areas/
│   └── Admin/               # Admin dashboard area
│       ├── Controllers/     # Admin controllers
│       └── Views/           # Admin views
├── Controllers/             # Main application controllers
├── Data/                    # Database context and initializer
├── Models/
│   ├── Entities/           # Domain models (Product, Order, etc.)
│   └── ViewModels/         # View-specific models
├── Services/               # Business logic layer
├── Views/                  # Razor views
│   ├── Account/
│   ├── Cart/
│   ├── Checkout/
│   ├── Home/
│   ├── Orders/
│   ├── Products/
│   └── Shared/
└── wwwroot/               # Static files (CSS, JS, images)
```

---

## 🚀 Step-by-Step Setup Guide

### Prerequisites

Before you begin, ensure you have the following installed:

1. **.NET 8.0 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
2. **Visual Studio Code** - [Download here](https://code.visualstudio.com/)
3. **XAMPP** (for MySQL) - [Download here](https://www.apachefriends.org/)
4. **C# Dev Kit Extension** for VS Code

### Step 1: Clone the Repository

```bash
git clone <repository-url>
cd Project/ShopSana
```

### Step 2: Install XAMPP and Configure MySQL

1. **Download and Install XAMPP** from https://www.apachefriends.org/
2. **Start XAMPP Control Panel**
3. **Start Apache and MySQL modules** by clicking "Start" buttons
4. **Open phpMyAdmin**: Go to http://localhost/phpmyadmin/

### Step 3: Create the Database

1. Open **phpMyAdmin** (http://localhost/phpmyadmin/)
2. Click on **"New"** in the left sidebar
3. Enter database name: `shopsana_db`
4. Select **Collation**: `utf8mb4_general_ci`
5. Click **"Create"**

### Step 4: Configure Database Connection

The connection string is already configured in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=shopsana_db;User=root;Password=;"
  }
}
```

> **Note**: If you have set a password for MySQL root user, update the `Password=` field.

### Step 5: Restore NuGet Packages

Open terminal in the ShopSana folder and run:

```bash
cd ShopSana
dotnet restore
```

### Step 6: Run the Application

```bash
dotnet run
```

The application will start and automatically:
- Create database tables using Entity Framework Core
- Seed initial data (categories, products, admin user)

### Step 7: Access the Application

Open your browser and navigate to:
- **Website**: https://localhost:5001 or http://localhost:5000

---

## 👤 Default Login Credentials

### Admin Account
- **Email**: admin@shopsana.com
- **Password**: Admin@123

### Test Customer Account
You can register a new customer account through the registration page.

---

## 📖 Using Visual Studio Code

### Opening the Project

1. Open VS Code
2. Go to **File > Open Folder**
3. Select the `ShopSana` folder
4. Install recommended extensions when prompted

### Running with VS Code

1. Open the integrated terminal (`Ctrl + ~`)
2. Navigate to the ShopSana folder: `cd ShopSana`
3. Run: `dotnet run`
4. Or press `F5` to run with debugging

### Recommended VS Code Extensions

- C# Dev Kit
- C# Extensions
- NuGet Package Manager

---

## 🗄️ Database Schema

### Main Tables

| Table | Description |
|-------|-------------|
| AspNetUsers | User accounts (extends ApplicationUser) |
| AspNetRoles | User roles (Admin, Customer) |
| Categories | Product categories |
| Products | Product catalog |
| Orders | Customer orders |
| OrderItems | Items in each order |
| CartItems | Shopping cart items |

### Entity Relationships

```
Users ──────┬── Orders ──── OrderItems ──── Products
            │
            └── CartItems ──────────────── Products
                                                │
Categories ─────────────────────────────────────┘
```

---

## 🎨 Screenshots

### Home Page
- Modern hero section with featured products
- Category browsing
- New arrivals section

### Product Catalog
- Filterable product grid with search
- Category filters and sorting options
- Pagination support

### Shopping Cart
- Full cart management
- Order summary with taxes and shipping
- Real-time updates

### Admin Dashboard
- Sales analytics
- Recent orders overview
- Low stock alerts

---

## 🔧 Troubleshooting

### Common Issues

1. **MySQL Connection Failed**
   - Ensure XAMPP MySQL is running
   - Check if port 3306 is not blocked
   - Verify database name matches connection string

2. **Migration Errors**
   - Delete `obj` and `bin` folders
   - Run `dotnet restore` again
   - The database is auto-created on first run

3. **Port Already in Use**
   - Change the port in `launchSettings.json`
   - Or kill the process using the port

### Resetting the Database

To reset the database and start fresh:

1. Open phpMyAdmin
2. Drop the `shopsana_db` database
3. Recreate it
4. Run the application again

---

## 🔮 Future Enhancements

- [ ] Online payment gateway integration (PayPal, Stripe)
- [ ] Product reviews and ratings system
- [ ] Email notification system
- [ ] AI-based product recommendations
- [ ] Mobile application support
- [ ] Wishlist functionality
- [ ] Coupon/discount codes

---

## 📄 License

This project is developed for educational purposes as part of the **Information System Analysis and Design Sessional** course.

---

## 👨‍💻 Author

Developed with ❤️ using ASP.NET Core, MySQL, and Bootstrap 5
