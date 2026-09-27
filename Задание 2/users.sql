-- Задание 2. MySQL. Только пользователи, без ролей.
-- Повторный импорт не создаёт дубликаты и не меняет существующие записи.
SET NAMES utf8mb4;
CREATE DATABASE IF NOT EXISTS obuv_task2 CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE obuv_task2;
CREATE TABLE IF NOT EXISTS users (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    full_name VARCHAR(200) NOT NULL,
    login VARCHAR(254) NOT NULL UNIQUE,
    password VARCHAR(128) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
-- Пароли из учебного Excel. Для рабочего приложения нужны стойкие хеши.
START TRANSACTION;
INSERT INTO users (full_name, login, password) VALUES
('Никифорова Весения Николаевна', '94d5ous@gmail.com', 'uzWC67'),
('Сазонов Руслан Германович', 'uth4iz@mail.com', '2L6KZG'),
('Одинцов Серафим Артёмович', 'yzls62@outlook.com', 'JlFRCZ'),
('Степанов Михаил Артёмович', '1diph5e@tutanota.com', '8ntwUp'),
('Ворсин Петр Евгеньевич', 'tjde7c@yahoo.com', 'YOyhfR'),
('Старикова Елена Павловна', 'wpmrc3do@tutanota.com', 'RSbvHv'),
('Михайлюк Анна Вячеславовна', '5d4zbu@tutanota.com', 'rwVDh9'),
('Ситдикова Елена Анатольевна', 'ptec8ym@yahoo.com', 'LdNyos'),
('Ворсин Петр Евгеньевич', '1qz4kw@mail.com', 'gynQMT'),
('Старикова Елена Павловна', '4np6se@mail.com', 'AtnDjr')
ON DUPLICATE KEY UPDATE id = users.id;
COMMIT;
