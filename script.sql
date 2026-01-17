-- Таблица Files (История загрузок файлов)
CREATE TABLE files (
    id SERIAL PRIMARY KEY,
    filename VARCHAR(255) UNIQUE NOT NULL,
    upload_time TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    processing_status BOOLEAN DEFAULT FALSE, -- true = успешно обработано, false = ошибка
);

CREATE TABLE values (
    date TIMESTAMP WITH TIME ZONE NOT NULL,
    execution_time NUMERIC NOT NULL CHECK(execution_time >= 0),
    value NUMERIC NOT NULL CHECK(value >= 0),
    file_id INTEGER REFERENCES files(id) ON DELETE CASCADE
);

-- Гипертаблица values для хранения временной шкалы измерений
SELECT create_hypertable('values', 'date');


-- Индексирование полей для ускорения выборок
CREATE INDEX idx_values_date ON values(date);

-- Таблица results для итоговых агрегатов по каждому файлу
CREATE TABLE results (
    id SERIAL PRIMARY KEY,
    file_name VARCHAR(255) UNIQUE NOT NULL,
    delta_seconds INTERVAL, -- Дельта времени (разница между максимальным и минимальным временем)
    start_time TIMESTAMP WITH TIME ZONE, -- Первое измеренное время
    average_execution_time NUMERIC, -- Среднее время выполнения операций
    average_value NUMERIC, -- Среднее значение показателей
    median_value NUMERIC, -- Медиана значений
    max_value NUMERIC, -- Максимальное значение
    min_value NUMERIC -- Минимальное значение
);

-- Индексация имени файла для быстрого поиска
CREATE INDEX idx_results_file_name ON results(file_name);

-- Добавляем ограничения целостности для соблюдения требований валидности данных
ALTER TABLE values ADD CONSTRAINT chk_valid_dates CHECK (date BETWEEN '2000-01-01' AND NOW());
ALTER TABLE values ADD CONSTRAINT chk_positive_times CHECK (execution_time >= 0);
ALTER TABLE values ADD CONSTRAINT chk_positive_values CHECK (value >= 0);