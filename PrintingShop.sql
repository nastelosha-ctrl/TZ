-- Создание базы данных
CREATE DATABASE PrintingShop;
USE PrintingShop;


-- Таблица Status
CREATE TABLE Status (
    id_status INT AUTO_INCREMENT PRIMARY KEY,
    status_name VARCHAR(50) NOT NULL
);

-- Таблица Service_Category
CREATE TABLE Service_Category (
    id_serviceCategory INT AUTO_INCREMENT PRIMARY KEY,
    category_name VARCHAR(100) NOT NULL
);

-- Таблица Client
CREATE TABLE Client (
    id_client INT AUTO_INCREMENT PRIMARY KEY,
    FIO VARCHAR(150) NOT NULL,
    phone_number VARCHAR(20) NOT NULL
);

-- Таблица Role (необходима для таблицы User)
CREATE TABLE Role (
    id_role INT AUTO_INCREMENT PRIMARY KEY,
    role_name VARCHAR(50) NOT NULL
);

-- Таблица User
CREATE TABLE User (
    id_user INT AUTO_INCREMENT PRIMARY KEY,
    FIO VARCHAR(150) NOT NULL,
    Login VARCHAR(50) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL,
    id_role INT NOT NULL,
    FOREIGN KEY (id_role) REFERENCES Role(id_role)
);

-- Таблица Service
CREATE TABLE Service (
    id_service INT AUTO_INCREMENT PRIMARY KEY,
    service_name VARCHAR(200) NOT NULL,
    Description TEXT,
    Price DECIMAL(10,2) NOT NULL,
    id_serviceCategory INT NOT NULL,
    service_image LONGBLOB NOT NULL,
    FOREIGN KEY (id_serviceCategory) REFERENCES Service_Category(id_serviceCategory)
);

-- Таблица Order
CREATE TABLE `Order` (
    id_order INT AUTO_INCREMENT PRIMARY KEY,
    id_service INT NOT NULL,
    Date_of_admission DATE NOT NULL,
    Due_date DATE,
    id_status INT NOT NULL,
    id_client INT NOT NULL,
    id_user INT NOT NULL,
    price DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (id_service) REFERENCES Service(id_service),
    FOREIGN KEY (id_status) REFERENCES Status(id_status),
    FOREIGN KEY (id_client) REFERENCES Client(id_client),
    FOREIGN KEY (id_user) REFERENCES User(id_user)
);

-- Таблица Basket
CREATE TABLE Basket (
    id_basket INT AUTO_INCREMENT PRIMARY KEY,
    id_order INT NOT NULL,
    FOREIGN KEY (id_order) REFERENCES `Order`(id_order)
);

-- Вставка данных в таблицу Role (3 записи)
INSERT INTO Role (role_name) VALUES 
('Администратор'),
('Менеджер'),
('Директор');

-- Вставка данных в таблицу Status (5 записей)
INSERT INTO Status (status_name) VALUES 
('Принят'),
('В работе'),
('Готов'),
('Выдан'),
('Отменен');

-- Вставка данных в таблицу Service_Category (7 записей)
INSERT INTO Service_Category (category_name) VALUES 
('Печать документов'),
('Фотопечать'),
('Визитки'),
('Баннеры'),
('Сувенирная продукция'),
('Альбомы и книги'),
('Мерч');

-- Вставка данных в таблицу Client (10 записей)
INSERT INTO Client (FIO, phone_number) VALUES 
('Иванов Алексей Петрович', '+79123456789'),
('Петрова Мария Сергеевна', '+79234567890'),
('Сидоров Дмитрий Иванович', '+79345678901'),
('Кузнецова Ольга Владимировна', '+79456789012'),
('Федорова Анна Михайловна', '+79567890123'),
('Смирнов Александр Игоревич', '+79678901234'),
('Ковалева Елена Викторовна', '+79789012345'),
('Попов Сергей Николаевич', '+79890123456'),
('Васильева Ирина Петровна', '+79901234567'),
('Николаев Денис Олегович', '+79012345678');

-- Вставка данных в таблицу User (5 записей)
INSERT INTO User (FIO, Login, Password, id_role) VALUES 
('Иванов Иван Иванович', 'admin', 'admin123', 1),
('Петров Петр Петрович', 'manager1', 'mng123', 2),
('Сидорова Ольга Дмитриевна', 'manager2', 'mng456', 2),
('Кузнецов Михаил Сергеевич', 'director', 'dir123', 3),
('Федорова Светлана Алексеевна', 'manager3', 'mng789', 2);

-- Вставка данных в таблицу Service (20 записей)
INSERT INTO Service (service_name, Description, Price, id_serviceCategory) VALUES 
('Печать черно-белая А4', 'Печать черно-белых документов формата А4', 15.00, 1),
('Печать цветная А4', 'Печать цветных документов формата А4', 30.00, 1),
('Печать А3 цветная', 'Печать цветных документов формата А3', 60.00, 1),
('Ламинирование А4', 'Ламинирование документов формата А4', 50.00, 1),
('Фото 10x15', 'Печать фотографий 10x15 см', 25.00, 2),
('Фото 15x20', 'Печать фотографий 15x20 см', 40.00, 2),
('Фотоколлаж А4', 'Создание фотоколлажа формата А4', 200.00, 2),
('Визитки стандартные 100шт', 'Изготовление стандартных визиток 100 шт', 300.00, 3),
('Визитки премиум 100шт', 'Изготовление визиток премиум качества 100 шт', 500.00, 3),
('Визитки с ламинацией 100шт', 'Изготовление ламинированных визиток 100 шт', 700.00, 3),
('Баннер 1x1м', 'Изготовление баннера 1x1 метр', 800.00, 4),
('Баннер 2x3м', 'Изготовление баннера 2x3 метра', 2000.00, 4),
('Ролл-ап 85x200см', 'Изготовление ролл-апа 85x200 см', 2500.00, 4),
('Кружка с логотипом', 'Нанесение логотипа на кружку', 350.00, 5),
('Футболка с принтом', 'Нанесение принта на футболку', 600.00, 5),
('Брелок акриловый', 'Изготовление акрилового брелока', 150.00, 5),
('Фотоальбом 20стр', 'Создание фотоальбома 20 страниц', 1200.00, 6),
('Книга в твердом переплете', 'Печать книги в твердом переплете', 800.00, 6),
('Бейсболка с лого', 'Нанесение логотипа на бейсболку', 450.00, 7),
('Толстовка с принтом', 'Нанесение принта на толстовку', 900.00, 7);

-- Вставка данных в таблицу Order (10 записей)
INSERT INTO `Order` (id_service, Date_of_admission, Due_date, id_status, id_client, id_user, price) VALUES 
(1, '2024-01-15', '2024-01-18', 4, 1, 2, 300.00),
(8, '2024-01-16', '2024-01-20', 4, 2, 3, 300.00),
(5, '2024-01-17', '2024-01-22', 3, 3, 2, 250.00),
(11, '2024-01-18', '2024-01-25', 2, 4, 3, 800.00),
(17, '2024-01-19', '2024-01-26', 1, 5, 5, 1200.00),
(14, '2024-01-20', '2024-01-28', 2, 6, 2, 350.00),
(15, '2024-01-21', '2024-01-29', 3, 7, 5, 600.00),
(9, '2024-01-22', '2024-01-30', 4, 8, 3, 500.00),
(12, '2024-01-23', '2024-01-31', 1, 9, 2, 2000.00),
(16, '2024-01-24', '2024-02-01', 2, 10, 5, 150.00);

-- Вставка данных в таблицу Basket (5 записей)
INSERT INTO Basket (id_order) VALUES 
(1),
(2),
(3),
(4),
(5);



