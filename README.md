# ShopXS - E-Commerce Web Application

ShopXS is a modern e-commerce platform built using ASP.NET Core MVC with SQLite database. It is inspired by major platforms such as Amazon, Alibaba, and Daraz.

## Features

### Core Features
- ✅ User Authentication & Authorization (ASP.NET Identity)
- ✅ Product Catalog with Categories
- ✅ Search & Filters
- ✅ Shopping Cart
- ✅ Order Management
- ✅ Payment Integration (Mock Gateway)
- ✅ Seller Marketplace
- ✅ Reviews & Ratings
- ✅ Admin Dashboard with Analytics

### User Types
1. *Customers* - Browse products, add to cart, place orders, track shipments
2. *Sellers* - Add products, manage inventory, view orders
3. *Administrators* - Manage users, approve products, handle orders, view analytics

## Technology Stack

- *Framework*: ASP.NET Core MVC (.NET 10)
- *Database*: SQLite (easily switchable to MySQL/SQL Server)
- *ORM*: Entity Framework Core
- *Authentication*: ASP.NET Identity
- *Frontend*: Razor Views, Bootstrap 5, Bootstrap Icons
- *Architecture*: Layered (Presentation, Business Logic, Data Access)

## Project Structure


ShopXS/
├── Areas/
│   ├── Admin/          # Admin dashboard and management
│   └── Seller/         # Seller dashboard and products
├── Controllers/        # Main application controllers
├── Data/              # Database context
├── Models/
│   ├── Entities/      # Domain models
│   └── ViewModels/    # View-specific models
├── Services/          # Business logic layer
├── Views/             # Razor views
└── wwwroot/           # Static files


## Getting Started

### Prerequisites
- .NET 10 SDK
- Visual Studio 2022+ or VS Code

### Installation

1. Clone the repository
bash
git clone <repository-url>
cd ShopXS


2. Restore packages
bash
dotnet restore


3. Run the application
bash
dotnet run


4. Open your browser and navigate to http://localhost:5000

### Default Admin Account
- *Email*: admin@shopxs.com
- *Password*: Admin@123

## Database

The application uses SQLite by default. The database file (shopxs.db) is created automatically on first run.

### Seed Data
- 5 default categories (Electronics, Clothing, Home & Garden, Sports, Books)
- Admin user account
- Roles: Admin, Seller, Customer

## Architecture

### Layered Architecture
1. *Presentation Layer*: Razor Views, Controllers, ViewModels
2. *Business Logic Layer*: Services with interfaces
3. *Data Access Layer*: Entity Framework Core with Repository pattern

### Key Entities
- User (ApplicationUser)
- Product
- Category
- Cart / CartItem
- Order / OrderItem
- Payment
- Review
- Shipment
- SupportTicket
- Wishlist

## Security Features
- Password hashing (ASP.NET Identity)
- Role-based access control
- CSRF protection
- Input validation
- Activity logging

## Screenshots

### Home Page
Modern hero section with featured products and categories

### Product Catalog
Filterable product grid with search and sorting

### Shopping Cart
Full cart management with order summary

### Admin Dashboard
Analytics and management interface

### Seller Dashboard
Product management and order tracking

## Future Enhancements
- AI Chatbot Support
- AR/VR Product Preview
- Advanced Analytics Dashboard
- Real Payment Gateway Integration
- Email Notifications
- Inventory Management

## License
This project is for educational purposes.
