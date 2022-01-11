
# Семейный бюджет



## Необходимое ПО

Стараемся обновляться и использовать только последние версии ПО

 - [Visual Studio Community 2022](https://visualstudio.microsoft.com/ru/)
 - [PostgreSQL](https://www.enterprisedb.com/downloads/postgres-postgresql-downloads/)

### Установка Visual Studio Community 2022
При установке VS необходимо выбрать "ASP.NET и разработка веб-приложений"

### Установка PostgreSQL
1. На вкладке **Select Components** убираем галочку со **Stack Builder**.
2. На вкладке **Password** пишем пароль **123**.
3. На вкладке **Port** должен стоять порт **5432**.
4. Остальное по дефолту.
5. При первом запуске нужно будет тыкнуть на **Apply Migrations** и обновить страницу.

## Первый запуск

1. Откройте Visual Studio
2. Выберите "Клонировать репозиторий"
3. В поле "Расположение репозитория" укажите https://github.com/yudakov1814/FamilyBudget
4. Нажмите "Клонировать"
5. Дождитесь пока репозиторий склонируется
6. На панели справа нужно кликнуть "FamilyBudget.sln"
7. Profit

## Разработка

Задачи берутся с [доски в Jira](https://familybudget.atlassian.net/jira/software/projects/NPRP/boards/1).
Разработка происходит в [git репозитории](https://github.com/yudakov1814/FamilyBudget).
В Visual Studio работать с git можно или через меню сверху во вкладке "Git", или через status-bar в самом низу окна

Этапы добавления новых фичей в git репозиторий:
1. Обновляем ветвь master
2. Отводим свою ветвь с именем по шаблону `<ticket-key> - <ticket-name>` (например: `NPRB-11 - Обновить лого сайта`)
3. Делаем нужные изменения
4. Пушим на сервер
5. Создаем Pull Request
6. Пингуем ревьюверов
7. Ждем аппрува

## Запуск

На панеле инструментов жмем на зеленый треугольник. Рядом должна быть надпись `IIS Express`, если что-то другое - нужно выбрать `IIS Express` в выпадающем списке.

## Полезные ссылки
 - [Репозиторий](https://github.com/yudakov1814/FamilyBudget)
 - [Доска в jira](https://familybudget.atlassian.net/jira/software/projects/NPRP/boards/1)
 - [Дока по asp net core](https://metanit.com/sharp/aspnet5/)
 - [GIT в Visual Studio](https://docs.microsoft.com/ru-ru/visualstudio/version-control/git-with-visual-studio?view=vs-2019)
 
