
<div align="center">

<h1>⌛ ChronoShift — Activity & Time Tracking Portal</h1>
  
<p>An ASP.NET Core web-app that streamlines daily activity logging and holiday calculation for company interns and full time employees.</p>

<div>
  <img src="https://img.shields.io/badge/Team%20-%20Project%20-%20gray?logo=codecrafters&labelColor=orange" style="height: 30px; width: auto;">
  <img src="https://img.shields.io/badge/In%20Development%20-%20%230f5bf3?logo=googlecloudspanner&logoColor=white" style="height: 30px; width: auto;">
</div>

</div>

---

## 🏷️ Features

- **Active Directory Sign-In** – Authenticate with your university / company Active Directory credentials.
  
- **Role-Based UI** – Pick between *Intern*, *Mentor*, *Manager* on first login or get assigned as an *Administrator* and unlock just the dashboards you need.
  
- **In-Line Timesheet Editing** – Add, overwrite or delete activities for any day, complete with duration and description, in a single editable grid.
  
- **Smart Holiday Overview** – Automatic conversion of logged hours to working-days and real-time calculation of remaining leave.

---

## 🧰 Tech Stack
| Layer            | Technology                                  |
|------------------|---------------------------------------------|
| Back-End         | ASP.NET Core, C#, Entity Framework Core |
| Front-End        | Razor Pages, HTML, CSS, Vanilla JS |
| Data             | MySQL |

---

## 🚀 Getting Started

### Prerequisites
- **.NET 5 SDK**
- **MySQL 8**

### Clone & Run
```bash
git clone https://github.com/vasilev17/chrono-shift.git
cd chrono-shift/ChronoShift
dotnet restore
dotnet run            # opens https://localhost:5000
```

---

## 🎬 Showcase

### Login Screen
<img width="939" height="569" alt="Chronoshift Login" src="https://github.com/user-attachments/assets/e8b6e867-0fd5-4f0b-a290-932dfcd4ef1e" />

### Role-Select Screen
![Chronoshift Role Select](https://github.com/user-attachments/assets/af631187-8fa5-436d-a994-894c0e953c4c)

