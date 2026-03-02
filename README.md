# Insurance API

This is a small API for managing **Claims** and **Covers** in an insurance system.  

It is designed to be **simple, clear, and easy to understand**, while demonstrating **event-driven architecture** with a **layered design**. Controllers don’t handle business logic directly, and all important operations are logged asynchronously.

 This architecture can be easily extended to **microservices**, using **Azure Service Bus** for communication between services.

---

## How It Works

Whenever you **create** or **delete** a claim or cover:

1. You call the API (like DELETE `/api/claims/{id}`).  
2. The **service layer** deletes the claim or cover.  
3. The service **publishes an event** (like `ClaimDeletedEvent`).  
4. A **background worker** (`QueuedHostedService`) picks up the event and writes an **audit record**.  

 This ensures that the API is **fast**, **scalable**, and **non-blocking**.

---

## Architecture Overview


[Controller] → [Service] → [Event Dispatcher] → [Background Queue] → [Audit Repository]


- **Controllers** handle HTTP requests.  
- **Services** contain business logic.  
- **Event Dispatcher** publishes domain events.  
- **Background Queue** picks up events asynchronously.  
- **Audit Repository** saves audit logs to the database.  

> In a **microservice setup**, the event dispatcher could push messages to **Azure Service Bus**, allowing multiple services to subscribe to the events without changing this API code.

---

## Endpoints

### Claims

- **GET /api/claims** – Returns all claims.  
- **DELETE /api/claims/{id}** – Deletes a claim by GUID and fires an event processed asynchronously.

### Covers

- **GET /api/covers** – Returns all covers.  
- **DELETE /api/covers/{id}** – Deletes a cover by GUID and logs the deletion asynchronously.

---

## Premium Calculation

Premiums are calculated based on **CoverType** and coverage duration.  

- Base rate: 1250 units/day  

**Type multiplier:**

| CoverType        | Multiplier |
|-----------------|------------|
| Yacht           | 1.10       |
| PassengerShip   | 1.20       |
| Tanker          | 1.50       |
| Other           | 1.30       |

**Coverage periods and discounts:**

| Period        | Days       | Discount | Notes                           |
|---------------|------------|----------|---------------------------------|
| First Period  | 0–30       | None     | Standard calculation             |
| Second Period | 31–180     | 5% (Yacht), 2% (Other) | Encourages longer coverage |
| Third Period  | 181+       | 3% (Yacht), 1% (Other) | Minor discount for long coverage |

> Calculated in the service layer and easily testable independently.

---

## Example Usage

### Deleting a Claim

1. Send DELETE `/claims/{id}` with a GUID.  
2. Service deletes the claim and publishes `ClaimDeletedEvent`.  
3. Background queue handles the event asynchronously and saves an audit record.  
4. API immediately returns `204 No Content`.

### Retrieving Claims

- GET `/claims` returns all claims.

---

## Testing

- Use **Postman** or any HTTP client.  
- DELETE requests **don’t require a body** — just the GUID.  
- Verify the audit record:

```sql
SELECT *
FROM ClaimAudit
WHERE ClaimId = 'your-guid-here';
