import random
from datetime import datetime, timedelta


def generate_random_timestamp(start_date):
    """Генерация случайного времени"""
    days_to_add = random.randint(
        0, 365 * 2)  # Случайное количество дней в пределах двух лет
    hours_to_add = random.randint(0, 23)
    minutes_to_add = random.randint(0, 59)
    seconds_to_add = random.randint(0, 59)
    milliseconds_to_add = random.randint(0, 999)

    new_date = start_date + timedelta(days=days_to_add, hours=hours_to_add,
                                      minutes=minutes_to_add, seconds=seconds_to_add, milliseconds=milliseconds_to_add)
    return new_date.strftime('%Y-%m-%dT%H:%M:%S.%fZ').replace('.000000', '.000')


def generate_random_float():
    """Генерация случайного дробного числа"""
    return round(random.uniform(0.1, 10.0), 1)


def main():
    # Начальная дата
    start_date = datetime.strptime(
        '2023-10-06T17:00:00.000Z', '%Y-%m-%dT%H:%M:%S.%fZ')

    with open('t0.csv', 'w') as f:
        for i in range(10000):
            # Генерация случайной даты
            random_date = generate_random_timestamp(start_date)
            # Генерация случайных значений
            execution_time = generate_random_float()
            value = generate_random_float()

            # Запись строки в файл
            f.write(f"{random_date};{execution_time};{value}\n")


if __name__ == "__main__":
    main()
