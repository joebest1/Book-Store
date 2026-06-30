
# 📚 Book Heaven (Book-Store)

ASP.NET Core MVC bookstore application with a full Web API backend, Identity-based authentication, role-based authorization (Admin / User), shopping cart, book search & sorting, Cloudflare Turnstile bot protection, and email-based password reset.



---

## 🧱 Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core MVC (.NET 8) |
| ORM | Entity Framework Core 8 (Code-First + Migrations) |
| Database | SQL Server |
| Auth | ASP.NET Core Identity (Cookie-based) + Session |
| Bot protection | Cloudflare Turnstile |
| Email | SMTP (Gmail) for password reset links |
| Logging | Serilog |
| API Docs | Swashbuckle (Swagger / OpenAPI) |
| Frontend | Razor Views (.cshtml) + Bootstrap/CSS + JS |

---

## 🏗️ Architecture

The project follows a **hybrid MVC + internal Web API** pattern: the MVC `Controllers` (Books, Account, Cart, Home) render the UI, but instead of talking to `AppDbContext` directly for books/cart/auth, they call **internal API controllers** (`ApiBookController`, `ApiCartController`, `UserApiController`) over HTTP using a shared `HttpClient`. This separates presentation from data access and makes the same API reusable by other clients (mobile, SPA, etc.).

```
Browser
   │
   ▼
MVC Controllers (Books / Account / Cart / Home)
   │  (HttpClient calls, cookie-based session preserved)
   ▼
Internal Web API (ApiBookController / ApiCartController / UserApiController / ApiFilterController)
   │
   ▼
AppDbContext (EF Core) ──► SQL Server
```

### Project structure

```
BookStore/
├── ActionFilter/            # Custom MVC action filters
├── Controllers/
│   ├── AcountController.cs  # MVC: Login, Register, Logout, Forgot/Reset Password
│   ├── BooksController.cs   # MVC: Book listing (search/sort), Create (Admin only)
│   ├── CartController.cs    # MVC: Cart page, Add/Delete (proxies to API)
│   ├── CartCountViewComponent.cs
│   ├── HomeController.cs
│   ├── Cart/ApiCartController.cs     # API: add / mycart / delete/{bookId}
│   ├── User/UserApiController.cs     # API: register / login / logout / forgot-password / reset-password
│   ├── User/EmailMsg.cs              # EmailSettings + IEmailService (SMTP)
│   └── book/
│       ├── ApiBookController.cs      # API: POST/GET/DELETE Books (Admin-protected writes)
│       └── ApiFilterController.cs    # API: search & sort books
├── Data/
│   ├── Db.cs                # AppDbContext (IdentityDbContext)
│   ├── Books.cs               # Books entity
│   ├── Cart.cs                # Cart entity
│   └── ViewModels/
├── Models/                    # ViewModels (Login, Register, Reset/Forgot Password, MyBookModel, ...)
├── Services/
│   ├── AuthService.cs         # Wraps calls to UserApiController
│   ├── BookService.cs         # Wraps calls to ApiBookController + client-side search/sort
│   └── CartService.cs         # Wraps calls to ApiCartController
├── Views/                     # Razor views: Account, Books, Cart, Home, Shared
├── Migrations/                # EF Core migrations
├── Seeds/SeedAdmin.cs
├── Program.cs                  # App startup, DI, middleware pipeline
├── appsettings.json
└── BookStore.csproj
```

---

## ✨ Features

- **Authentication & Authorization**: Register/Login/Logout via ASP.NET Core Identity, with `Admin` and `User` roles seeded automatically on startup (`admin@example.com` / `Admin@123`, `user@example.com` / `User@123` — demo accounts created in `Program.cs`).
- **Bot protection**: Registration form is protected with **Cloudflare Turnstile**; the server verifies the token server-side before creating the account.
- **Password recovery**: Forgot/Reset password flow that emails a reset link via SMTP (Gmail).
- **Book catalog**: Browse, search (by title/author), and sort (by price/date, asc/desc).
- **Admin-only book creation**: Only users with the `Admin` role can add new books, including image upload (saved to `wwwroot/images/books`).
- **Shopping cart**: Per-user cart (tied to the logged-in `UserId`), add/remove items, automatic quantity merge if the same book is added twice, live cart item count shown in the navbar via a `ViewComponent`.
- **Swagger UI**: Auto-generated API documentation available at `/swagger`.
- **Centralized error handling**: Global exception handler middleware returns a clean JSON 500 response and logs the error via Serilog.
- **Session-based UI state**: `UserRole` and `UserEmail` are stored in session to drive navbar rendering (guest vs. authenticated, Admin-only links).

---

## 🔌 API Reference

All internal APIs require the user to be authenticated (cookie-based), except where noted.

### Auth — `UserApiController` (`/api/UserApi`)

| Method | Route | Description |
|---|---|---|
| POST | `/api/UserApi/register` | Create a new user (FirstName, LastName, Email, Password, optional Role) |
| POST | `/api/UserApi/login` | Sign in, returns the user's role |
| POST | `/api/UserApi/logout` | Sign out the current user |
| POST | `/api/UserApi/forgot-password` | Sends a password-reset email |
| POST | `/api/UserApi/reset-password` | Resets the password using the emailed token |

### Books — `ApiBookController` (`/Books`)

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/Books` (`ApiBook/Books`) | Public | List all books |
| POST | `/Books` (`ApiBook/Books`) | **Admin only** | Add a new book (multipart form, supports image upload) |
| DELETE | `/Books/{id}` | — | Delete a book by id |

### Search & Sort — `ApiFilterController` (`/api/Books`)

| Method | Route | Description |
|---|---|---|
| GET | `/api/Books/search?author=&title=&category=` | Filter books by author/title/category (at least one required) |
| GET | `/api/Books/sort?sortBy=date\|price&order=asc\|desc` | Sort the full book list |

### Cart — `ApiCartController` (`/api/Cart`)

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/api/Cart/add` | Required | Add a book to the current user's cart (merges quantity if already present) |
| GET | `/api/Cart/mycart` | Required | Get the current user's cart items + total price |
| DELETE | `/api/Cart/delete/{bookId}` | Required | Remove a book from the cart |

---

## ⚙️ Setup & Running Locally

### Prerequisites
- .NET 8 SDK
- SQL Server (local or Express)

### Steps

1. **Clone the repo**
   ```bash
   git clone https://github.com/joebest1/Book-Store.git
   cd Book-Store
   ```

2. **Configure `appsettings.json`** (use `appsettings.Development.json` locally — do **not** commit real secrets):
   - `ConnectionStrings:DefaultConnection` → your SQL Server connection string
   - `EmailSettings` → your SMTP credentials (for password reset emails)
   - `Captcha:SecretKey` → your Cloudflare Turnstile secret key

3. **Apply EF Core migrations**
   ```bash
   dotnet ef database update
   ```

4. **Run the app**
   ```bash
   dotnet run
   ```

5. **Browse**
   - App: `https://localhost:7265/`
   - Swagger: `https://localhost:7265/swagger`

   On first run, the app auto-seeds two demo accounts:
   - Admin: `admin@example.com` / `Admin@123`
   - User: `user@example.com` / `User@123`

---

## ⚠️ Security note

`appsettings.json` in this repo currently contains real-looking SMTP credentials and a Captcha secret key committed directly in source control. **These should be rotated immediately and moved out of source control** — e.g. into `appsettings.Development.json` (gitignored), environment variables, or a secrets manager (User Secrets / Azure Key Vault). Committing live credentials publicly is a serious security risk even for a learning project.

---

## 🗺️ Roadmap / Possible Improvements

- Move secrets to `dotnet user-secrets` or environment variables
- Add server-side validation messages translated to Arabic for end users
- Add unit/integration tests for `Services` and API controllers
- Add a deployed demo link
- Add order/checkout flow on top of the existing cart
