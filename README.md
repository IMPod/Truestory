# Truestory.WebAPI

This is a simple RESTful Web API built with ASP.NET Core 8 that integrates with the mock API at [https://restful-api.dev](https://restful-api.dev). It extends the mock API functionality by adding filtering, pagination, validation, error handling, and clean architecture using MediatR.

## ✅ Features

- 🔎 **GET** products with filtering by name and pagination
- ➕ **POST** new product with validation
- ❌ **DELETE** a product by ID
- ⚙️ Clean architecture with CQRS via MediatR
- 🧪 Validation and structured error handling
- 🧾 OpenAPI/Swagger UI for testing

---

## 🧱 Tech Stack

- .NET 8
- ASP.NET Core Web API
- MediatR (for CQRS)
- Swagger (Swashbuckle)
- System.Net.Http for integration

---

## 🚀 How to Run the Project

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

### Steps

1. **Build and run:**

```bash
dotnet build
dotnet run
```

2. **Open Swagger UI:**

Visit: [https://localhost:5001/swagger](https://localhost:5001/swagger)

> You may also try `http://localhost:5000/swagger` depending on your launch settings.

---

## 📦 API Endpoints

### GET `/api/products`

Returns a list of products with optional filtering and paging.

#### Query Parameters:

| Parameter  | Type   | Description                        |
|------------|--------|------------------------------------|
| `name`     | string | (optional) filter by name          |
| `page`     | int    | (optional) default = 1             |
| `pageSize` | int    | (optional) default = 10            |

### POST `/api/products`

Creates a new product.

#### Request Body Example:

```json
{
  "name": "iPhone 15",
  "data": {
    "color": "Black",
    "storage": "128GB"
  }
}
```

- Returns: created product object

### DELETE `/api/products/{id}`

Deletes a product by its ID.

- Returns: `204 No Content` on success

---

## ✅ Validation Rules

- `name` must not be empty.
- `data` must be present and valid JSON.

---

## ❗ Error Handling

- Returns `400` for invalid input
- Returns `404` if product not found
- Returns `500` if external API fails

---

## 🧑‍💻 Author

This project was created as part of a technical assignment for **Truestory**.