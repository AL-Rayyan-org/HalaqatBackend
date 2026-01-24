-- 1. Users Table (The source of truth for all people)
CREATE TABLE users (
    id VARCHAR(50) PRIMARY KEY,
    first_name VARCHAR(100),
    last_name VARCHAR(100),
    email VARCHAR(255) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    phone VARCHAR(20),
    role VARCHAR(20) NOT NULL, -- e.g., 'admin', 'teacher', 'student'
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- 2. Halaqat Table
CREATE TABLE groups (
    id VARCHAR(50) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    recitation_days VARCHAR(255),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- 3. Teacher relations with Halaqat (Linked to users)
CREATE TABLE group_teachers (
    id VARCHAR(50) PRIMARY KEY,
    group_id VARCHAR(50) NOT NULL,
    teacher_id VARCHAR(50) NOT NULL, -- References users.id
    CONSTRAINT fk_group FOREIGN KEY (group_id) REFERENCES groups(id) ON DELETE CASCADE,
    CONSTRAINT fk_teacher_user FOREIGN KEY (teacher_id) REFERENCES users(id) ON DELETE CASCADE
);

-- 4. Students Metadata Table
-- (Keep this only if you need specific fields like 'info' that don't belong in 'users')
CREATE TABLE students (
    id VARCHAR(50) PRIMARY KEY, -- Usually same as user_id for 1:1 relationship
    user_id VARCHAR(50) NOT NULL UNIQUE,
    group_id VARCHAR(50),
    info TEXT,
    CONSTRAINT fk_student_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    CONSTRAINT fk_student_group FOREIGN KEY (group_id) REFERENCES groups(id) ON DELETE SET NULL
);

-- 5. Recitation Logs (Linked to users for teacher tracking)
CREATE TABLE recitation_logs (
    id VARCHAR(50) PRIMARY KEY,
    student_id VARCHAR(50) NOT NULL, -- References students.id
    teacher_id VARCHAR(50) NOT NULL, -- References users.id
    group_id VARCHAR(50) NOT NULL,
    date DATE NOT NULL DEFAULT CURRENT_DATE,
    completed BOOLEAN DEFAULT FALSE,
    hifz_content VARCHAR(255),
    hifz_page_count FLOAT DEFAULT 0,
    hifz_reminders INTEGER DEFAULT 0,
    hifz_mistakes INTEGER DEFAULT 0,
    hifz_minor_mistakes INTEGER DEFAULT 0,
    hifz_final_score INTEGER,
    rev_content VARCHAR(255),
    rev_page_count FLOAT DEFAULT 0,
    rev_reminders INTEGER DEFAULT 0,
    rev_mistakes INTEGER DEFAULT 0,
    rev_minor_mistakes INTEGER DEFAULT 0,
    rev_final_score INTEGER,
    extra_point INTEGER DEFAULT 0,
    notes VARCHAR(500),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_log_student FOREIGN KEY (student_id) REFERENCES students(id) ON DELETE CASCADE,
    CONSTRAINT fk_log_teacher_user FOREIGN KEY (teacher_id) REFERENCES users(id) ON DELETE CASCADE,
    CONSTRAINT fk_log_group FOREIGN KEY (group_id) REFERENCES groups(id) ON DELETE CASCADE
);

-- 6. Attendance Table
CREATE TABLE attendance (
    id VARCHAR(50) PRIMARY KEY,
    student_id VARCHAR(50) NOT NULL,
    group_id VARCHAR(50) NOT NULL,
    date DATE NOT NULL DEFAULT CURRENT_DATE,
    status VARCHAR(30),
    CONSTRAINT fk_attendance_student FOREIGN KEY (student_id) REFERENCES students(id) ON DELETE CASCADE,
    CONSTRAINT fk_attendance_group FOREIGN KEY (group_id) REFERENCES groups(id) ON DELETE CASCADE
);

-- 7. Audit Logs
CREATE TABLE audit_logs (
    id VARCHAR(50) PRIMARY KEY,
    user_id VARCHAR(50),
    date DATE DEFAULT CURRENT_DATE,
    action VARCHAR(255),
    entity_name VARCHAR(100),
    CONSTRAINT fk_audit_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE SET NULL
);

-- 8. Refresh Tokens Table
CREATE TABLE refresh_tokens (
    id VARCHAR(50) PRIMARY KEY,
    user_id VARCHAR(50) NOT NULL,
    token TEXT NOT NULL,
    expires_at TIMESTAMP WITH TIME ZONE NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    revoked_at TIMESTAMP WITH TIME ZONE,
    CONSTRAINT fk_token_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);