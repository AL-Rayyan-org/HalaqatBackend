# Halaqat Backend - Project Instructions

## Project Overview

Halaqat Backend is a management system for tracking and evaluating students' progress in memorizing the Holy Quran. The system manages recitation sessions, evaluates performance using a point-based grading methodology, and generates analytics for students and groups (Halaqat).

## Technology Stack

- **.NET 10** - Target framework
- **PostgreSQL** - Database (Location: `DB\Halaqat.db`)
- **BCrypt.Net-Next** - Password hashing
- **ASP.NET Core** - Web API framework

## Project Architecture

### Folder Structure & Responsibilities

#### 1. **Models Folder**
- Contains classes that are **directly mapped to database tables**
- Each model represents a database entity with matching properties
- Example: `User`, `Teacher`, `Student`, `RecitationLog`, `Group`, etc.

#### 2. **DTOs Folder**
- Contains Data Transfer Objects for **communication with the frontend**
- Used for sending data to and receiving data from the client
- Separate DTOs for requests and responses to avoid exposing internal model structure
- Example: `LoginRequestDto`, `UserResponseDto`, `RecitationLogCreateDto`, etc.

#### 3. **Controllers Folder**
- Contains **API endpoints ONLY**
- **NO BUSINESS LOGIC should be placed here**
- Controllers should only:
  - Receive requests from frontend
  - Call appropriate service methods
  - Return responses
- **MUST use authorization attributes** to enforce role-based access control
- Example attributes: `[Authorize(Roles = "Owner,Admin")]`, `[Authorize(Roles = "Teacher")]`

**Example Controller Pattern:**
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Owner,Admin,Teacher")]
    public async Task<IActionResult> GetStudent(Guid id)
    {
        var result = await _studentService.GetStudentByIdAsync(id);
        return Ok(result);
    }
}
```

#### 4. **Services Folder**
- **THE BRAIN OF THE PROJECT**
- Contains **ALL business logic and calculations**
- Services handle:
  - Data validation
  - Business rule enforcement
  - Score calculations
  - Performance analytics
  - Complex operations involving multiple repositories
- Each service should have an interface (e.g., `IStudentService`)

#### 5. **Repositories Folder**
- **Database connection layer**
- Responsible for all database interactions
- Implements CRUD operations
- Uses Entity Framework Core or raw SQL queries
- Each repository should have an interface (e.g., `IStudentRepository`)
- **NO BUSINESS LOGIC** - only data access operations

## User Roles & Permissions

### Role Hierarchy

1. **Owner**
   - Full access to all APIs
   - Can perform any operation in the system
   
2. **Admin**
   - Same as Owner
   - **Cannot delete Owner accounts**
   
3. **Teacher**
   - Access limited to their assigned groups (Halaqat) only
   - Can manage students in their groups
   - Can create/update recitation logs for their students
   - Can view attendance and analytics for their groups
   
4. **Student**
   - **Read-only access** to their own data
   - Can view their recitation logs
   - Can view their attendance
   - Can view their performance analytics
   - Cannot modify any data

## Database Schema

The database is located at: `D:\ahmad\Work Projects\AL-Rayyan\HalaqatBackend\DB\Halaqat.db`

### Core Tables:
- **users** - User accounts (email, password_hash, role)
- **teachers** - Teacher profiles
- **students** - Student profiles
- **groups** - Halaqat (Quran study groups)
- **group_teachers** - Many-to-many relationship between teachers and groups
- **recitation_logs** - Daily recitation records with scores
- **attendance** - Student attendance tracking
- **audit_logs** - System activity logging
- **refresh_tokens** - JWT refresh token management

## Business Logic & Core Concepts

### 1. Dynamic Scheduling

The system supports **flexible scheduling** for different groups (Halaqat). While groups may currently meet on specific days (e.g., Sunday/Wednesday or Saturday/Tuesday), the architecture is dynamic and can accommodate:
- Increasing to 3+ days per week
- Decreasing to 1 day per week
- Different schedules for different groups

### 2. Scoring & Evaluation Logic

Every recitation session starts with a **maximum of 200 points**:
- **100 points for Memorization (Hifz)**
- **100 points for Revision (Mura'aja)**

#### Point Deduction System

| Error Type | Arabic Term | Penalty |
|------------|-------------|---------|
| Reminder | Tanbih | -1 point |
| Minor Mistake | Ghalat Basit | -2 points |
| Major Mistake | Ghalat | -4 points |

#### Score Calculation Formula

```
Score = 100 - (Reminders × 1) - (Minor Mistakes × 2) - (Major Mistakes × 4)
```

**Implementation Example:**
```csharp
public int CalculateRecitationScore(int reminders, int minorMistakes, int majorMistakes)
{
    int baseScore = 100;
    int deductions = (reminders * 1) + (minorMistakes * 2) + (majorMistakes * 4);
    return Math.Max(0, baseScore - deductions);
}
```

### 3. Extra Points & Attendance Penalties

The `extra_point` column in `recitation_logs` accounts for **punctuality and behavior**.

#### Delay Penalties:

| Excuse Status | Penalty |
|---------------|---------|
| Valid excuse | 0 points deduction |
| Unconvincing excuse | -5 from Hifz, -5 from Revision |
| No excuse | -10 from Hifz, -10 from Revision |

**Note:** These penalties are applied through the `extra_point` field (stored as negative values).

### 4. Analytics and Rankings

#### Monthly Student Rankings

Students are ranked by their **total monthly points**:
```
Total Points = Sum of (Hifz Scores + Revision Scores + Extra Points)
```

Top 3 students receive 1st, 2nd, and 3rd place honors.

#### Group Performance Percentage

Each group (Halaqa) receives a performance percentage calculated as:

```
Group Performance % = (Average Student Points / Maximum Possible Points) × 100
```

**Formula Breakdown:**
1. Sum all student points for the month
2. Divide by the number of students (Average)
3. Divide by total possible points for the period
4. Multiply by 100 for percentage

**Example Calculation:**
- 4-week period
- 2 sessions per week
- Total possible = 8 sessions × 200 points = 1,600 points
- Group average = 1,200 points per student
- Performance % = (1,200 / 1,600) × 100 = 75%

## Coding Guidelines

### 1. Separation of Concerns
- Keep Controllers thin - no logic
- Keep Services fat - all logic here
- Keep Repositories focused - only data access

### 2. Authorization
- **Always** use `[Authorize]` attribute on protected endpoints
- Specify roles explicitly: `[Authorize(Roles = "Owner,Admin")]`
- Validate user permissions in Services when needed (e.g., teachers accessing only their groups)

### 3. Error Handling
- Use proper HTTP status codes
- Return meaningful error messages
- Log errors appropriately

### 4. Async/Await
- Use async/await for all database operations
- All service and repository methods should be async

### 5. Dependency Injection
- Register all services and repositories in `Program.cs`
- Use constructor injection
- Follow interface-based programming

### 6. Security
- **Always hash passwords** using BCrypt.Net-Next
- Never expose password hashes in DTOs
- Validate user input
- Prevent SQL injection by using parameterized queries

### 7. Naming Conventions
- Use PascalCase for classes, methods, and properties
- Use camelCase for method parameters and local variables
- Prefix interfaces with "I" (e.g., `IStudentService`)
- Use descriptive names that reflect purpose

### 7. Http Response
always return the response opject that in: (`DTOs/ApiResponse`) for all endpoints.

## Example Workflow

### Adding a New Feature (e.g., Recitation Log)

1. **Create Model** (`Models/RecitationLog.cs`)
   - Map to database table structure

2. **Create DTOs** (`DTOs/RecitationLogCreateDto.cs`, `DTOs/RecitationLogResponseDto.cs`)
   - Define what frontend sends/receives

3. **Create Repository Interface & Implementation** (`Repositories/IRecitationLogRepository.cs`)
   - Implement database operations

4. **Create Service Interface & Implementation** (`Services/IRecitationLogService.cs`)
   - Implement business logic
   - Calculate scores using the formulas above
   - Validate data

5. **Create Controller** (`Controllers/RecitationLogsController.cs`)
   - Add endpoints with proper authorization
   - Call service methods
   - Return results

6. **Register in DI Container** (`Program.cs`)
   ```csharp
   builder.Services.AddScoped<IRecitationLogRepository, RecitationLogRepository>();
   builder.Services.AddScoped<IRecitationLogService, RecitationLogService>();
   ```
