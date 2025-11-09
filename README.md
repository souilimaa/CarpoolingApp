# 🚗 WeGo Carpooling (Xamarin • C# • SQL Server)

A modern **Android** application that connects **drivers** and **passengers** for shared rides — cutting costs, congestion, and emissions.



---

## 📚 Table of Contents
- [Overview](#overview)
- [Core Features](#core-features)
- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Prerequisites](#prerequisites)
- [Setup & Run](#setup--run)
- [Configuration](#configuration)
- [UML & Design](#uml--design)

---

## 🌟 Overview
**WeGo Carpooling** lets drivers **offer rides** and passengers **book seats** along the same route. Users create profiles, publish or search rides, message each other, and finalize trips with ratings.

**Problem space:** high travel cost, environmental impact, seat under-utilization.  
**Solution:** a lightweight mobile platform that matches people going the same way — securely and in real time.

---

## ✅ Core Features
- **Account & Auth** (register/login, persistent session)
- **Ride Offers** (origin, destination, date/time, price/seat, seats available)
- **Ride Search** (filters by city/route/date)
- **Booking Flow** (request/approve/confirm)
- **Ratings/Reviews** (post-ride)
- **Ride History**  (past & upcoming rides)
- **Profile & Vehicle**  (photo, phone, car model/plate)
- **Map Support**  (route preview & pickers — optional)
- **Notifications** (requests, approvals, cancellations)

---

## 🧱 Architecture
- **Mobile app (Android, Xamarin.Forms)** — UI, UX, client logic  
- **API layer (ASP.NET Web API)** — endpoints for auth/rides/bookings  
- **Database (SQL Server)** — normalized schema for users/rides/bookings/messages/ratings  


---

## 🛠️ Tech Stack
- **Mobile:** Xamarin.Forms (Android), C#, MVVM, Visual Studio  
- **Backend:** ASP.NET Web API (.NET 6/7), C#  
- **Database:** Microsoft **SQL Server** (Express/LocalDB) with EF Core  
- **Notifications:** Firebase Cloud Messaging (optional)  
- **Build/CI:** .NET CLI, Gradle (via VS), GitHub Actions (optional)


---

## 📦 Prerequisites
- **Visual Studio 2022** with **Mobile development with .NET** workload
- **Android SDK/Emulator** (API 30+ recommended)
- **.NET SDK** (6 or 7)
- **SQL Server** (Express/LocalDB) + **SSMS** (optional)
- **Git**

---

## 🚀 Setup & Run

### 1) Database
```bash
# Option A: run SQL scripts
sqlcmd -S .\SQLEXPRESS -i db\schema.sql
sqlcmd -S .\SQLEXPRESS -i db\seed.sql

# Option B: EF Core migrations
cd server/WeGo.Api
dotnet ef database update
```



### 2) Android app
- Open `WeGo.sln` in **Visual Studio**  
- Select **WeGo.Mobile.Android** → **Run** (emulator or device)

---

## ⚙️ Configuration


### Mobile (`mobile/WeGo.Mobile/Services/ApiClient.cs`)
```csharp
public const string BaseUrl = "http://10.0.2.2:5000"; // Android emulator -> host
```

> For Google Maps, add your **Android API key** to `AndroidManifest.xml` and enable Maps SDK.

---




## 🧩 UML & Design
Export from your slides and add to `/images`:
- `uml-usecase.png`:
    <img width="658" height="809" alt="image" src="https://github.com/user-attachments/assets/bb353ac7-8b5a-41d7-acad-285b1c638a5c" />
  
- `uml-class.png`:
  <img width="1035" height="793" alt="image" src="https://github.com/user-attachments/assets/4fbb311a-decb-40b7-88db-c0f62ae1c772" />
  
- `uml-activity-offer.png`:
  <img width="650" height="742" alt="image" src="https://github.com/user-attachments/assets/992003c4-4c63-4ec5-9876-d6b00679df6a" />
  
- `uml-activity-reserve.png`:
  <img width="583" height="775" alt="image" src="https://github.com/user-attachments/assets/4c1d401b-4584-408c-8e71-482cc2b05e09" />
  



---


