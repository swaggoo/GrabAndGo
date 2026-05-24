# 📘 Grab&Go (Backend)

> HoreCA Застосунок для пошуку та замовлення не реалізованих товарів (Food Rescue Platform)

---

## 👤 Автор

**ПІБ:** Нирка Назар Іванович
**Група:** ФЕІ-45  
**Керівник:** доцент Соколовський Богдан Степанович  
**Дата виконання:** 23.05.2026

---

## 📌 Загальна інформація

**Тип проєкту:** Розподілена мікросервісна система (Backend)  
**Мова програмування:** C# 12  
**Платформа:** .NET 8.0  
**Архітектура:** Clean Architecture + Vertical Slice Architecture + CQRS  
**Технологічний стек:**
- **API Gateway:** Ocelot
- **Брокер повідомлень:** RabbitMQ (MassTransit)
- **Бази даних:** PostgreSQL (Order, Identity), MongoDB (Catalog)
- **Транзакції:** патерн Saga (State Machine) + Transactional Outbox
- **Аутентифікація:** ASP.NET Core Identity + JWT (Access/Refresh tokens)
- **Спостережуваність (Observability):** OpenTelemetry + Prometheus + Grafana + Loki + Tempo

---

## 🧠 Опис функціоналу

- 🔐 **Identity Service** — Керування користувачами, реєстрація, авторизація та видача JWT токенів з підтримкою ролей (Customer/Business).
- 🛍️ **Catalog Service** — Управління каталогом товарів («Секретні коробки»), категоріями та профілями магазинів (MongoDB).
- 🛒 **Order Service** — Обробка життєвого циклу замовлення: створення, підтвердження та історія (PostgreSQL).
- 🗺️ **Геофільтрація** — Пошук товарів та магазинів у заданому радіусі за координатами GPS.
- 🔄 **Розподілені транзакції** — Автоматичне резервування товарів на складі через асинхронну чергу повідомлень (Saga).
- 📊 **Моніторинг** — Повний LGTM-стек для відстеження RPS, помилок, трейсингу запитів та логування в реальному часі.

---

## 🧱 Опис основних проектів

### Core Services (Мікросервіси)
| Проект | Призначення |
|------|-----------|
| `GrabAndGo.Identity.API` | Сервіс ідентифікації та профілів користувачів |
| `GrabAndGo.Catalog.API` | Сервіс управління асортиментом та магазинами |
| `GrabAndGo.Order.API` | Сервіс обробки замовлень та оплат |

### Infrastructure & Communication
| Проект / Файл | Призначення |
|------|-----------|
| `GrabAndGo.gateway` | Ocelot API Gateway — єдина точка входу |
| `GrabAndGo.BuildingBlocks` | Спільна бібліотека (Middleware, Events, Specifications) |
| `docker-compose.yml` | Конфігурація для розгортання всієї інфраструктури |

---

## ▶️ Як запустити проєкт "з нуля"

### 1. Встановлення інструментів

Переконайтесь, що у вас встановлено:
- **Docker Desktop**
- **Python 3.x** (для навантажувальних тестів)

### 2. Клонування репозиторію

```bash
git clone https://github.com/GrabAndGoUA/GrabAndGo-Backend.git
cd GrabAndGo
```

### 3. Запуск інфраструктури (Docker)

Запустіть всі сервіси, бази даних та систему моніторингу однією командою:

```bash
docker-compose up -d
```

### 4. Доступ до документації

Після запуску документація Swagger доступна за адресами:
- **Gateway (All):** `http://localhost:5010/docs/services`
- **Identity:** `http://localhost:5001`
- **Catalog:** `http://localhost:5002`
- **Order:** `http://localhost:5003`

---

## 🖱️ Моніторинг та аналітика

Для відстеження стану системи доступні наступні панелі:

- 📈 **Grafana:** `http://localhost:3000` (Логін: `admin`, Пароль: `admin`)
- 📉 **Prometheus:** `http://localhost:9090`
- 📨 **RabbitMQ UI:** `http://localhost:15672` (guest/guest)

---

## 📱 Структура рішення

```
GrabAndGo/
├── src/
│   ├── gateway/                  # Ocelot Gateway
│   ├── services/
│   │   ├── Identity/             # Auth & User Service
│   │   ├── Catalog/              # MongoDB Catalog Service
│   │   └── Order/                # Order Processing Service
│   └── BuildingBlocks/           # Shared Kernel
├── tests/
│   ├── GrabAndGo.Catalog.UnitTests/
│   ├── GrabAndGo.Order.UnitTests/
│   ├── GrabAndGo.Identity.UnitTests/
│   └── GrabAndGo.BuildingBlocks.UnitTests/
├── infra/
│   └── observability/            # Grafana/Loki/Prometheus configs
└── load-test.py                  # Locust load test script
```

---

## 🧪 Проблеми і рішення

| Проблема | Рішення |
|----------|---------|
| **Конфлікт при бронюванні** | Впроваджено **Optimistic Concurrency Control** (RowVersion) у PostgreSQL |
| **Втрата повідомлень** | Використано патерн **Transactional Outbox** для гарантованої доставки в RabbitMQ |
| **Складні запити** | Реалізовано патерн **Specification** для гнучкої фільтрації та пошуку |
| **Latency у мікросервісах** | Налаштовано **Distributed Tracing** (Tempo) для пошуку вузьких місць |

---

## 📋 Тестування

### 1. Модульне тестування (xUnit + Moq)
Запуск всіх тестів (понад 50 тестів):
```bash
dotnet test
```

### 2. Навантажувальне тестування (Locust)
Запуск симуляції пікового навантаження (Happy Hour):
```bash
pip install locust
locust -f load-test.py --host http://localhost:5010
```

---

## 🧾 Використані джерела / література

### Основні ресурси
- [Microsoft Learn: .NET Microservices Architecture](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/)
- [MassTransit: Distributed App Framework](https://masstransit.io/)
- [Ocelot Documentation](https://ocelot.readthedocs.io/)
- [OpenTelemetry Documentation](https://opentelemetry.io/)

### Наукові джерела
- Makov, T., et al. (2020). Social and environmental analysis of food waste abatement. *Nature Communications*.
- de Almeida Oroski, F. (2023). Understanding food waste-reducing platforms. *Waste Management & Research*.

---

## 📝 Ліцензія

Цей проєкт ліцензований під ліцензією MIT.
