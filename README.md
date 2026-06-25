# HotelViewer

Десктопное приложение для **безопасного** управления отелями.

## Программный стек и инструменты

| Инструмент          |           Версия           | Предназначение                                                                  |
|:--------------------|:--------------------------:|:--------------------------------------------------------------------------------|
| C#                  |            14.0            | Язык программированя                                                            |
| .NET                |            10.0            | Платформа разработки и рантайма                                                 |
| LanguageExt         |       5.0.0-beta-77        | Библиотека с инструментами функционального программирования                     |
| xUnit.v3            |       4.0.0-pre.128        | Платформа модульного тестирования                                               |
| WPF                 |             -              | Платформа для декларативной разработки пользовательского интерфейса для Windows |
| Argon2Sharp         |           4.0.1            | Библиотека для хэширования паролей                                              |
| Micro$oft Access    |             -              | База данных                                                                     |
| System.Data.OleDb   | 11.0.0-preview.5.26302.115 | Библиотека для низкоуровнего общения с базой данных в среде ОС Windows          |
| Microsoft.ACE.OLEDB |             -              | Драйвер для OleDb для общения с базой данных Micro$oft Access                   |

### Архитектура

Для обеспечения минимальной связности компонентов, чтобы обеспечить лёгкую портативность кодовой базы меж
разными базами данных, фреймворков для написания пользовательского интерфейса и прочего, был использован
паттерн проектироввания DDD (Domain-Driven Design). Решение разделено на четыре проекта, которые отвечают
за разные задачи:
- `HotelViewer.Domain` - тут хранится основная логика и интерфейсы репозиториев
- `HotelViewer.Infrastructure` - общение с инфраструктурой (сторонние API, работа с файлами, БД и так далее).
  В нашем случае тут работа с базой данных MS Access через библиотеку `System.Data.OleDb` и драйвер
  `Microsoft.ACE.OLEDB.12.0`. Также тут реализуются интерфейсы репозиториев.
- `HotelViewer.ApplicationLayer` - прикладной уровень, отвечает за оркестрацию всем этим безумием.
- `HotelViewer.Presentation` - уровень презентации, отвечает за I/O с пользовательской стороны.

#### UML диаграммы
##### Архитектура
```plantuml
@startuml
package "Presentation Layer (WPF)" {
    [Views (Windows/Controls)] --> [ViewModels]
    [ViewModels] --> [Converters/Infrastructure]
}

package "Application Layer" {
    [EntityService<T, ID>]
    [AuthService]
    [ExportService]
    [SessionContext]
}

package "Domain Layer" {
    [Entities]
    [Value Objects]
    [Repository Interfaces]
}

package "Infrastructure Layer" {
    [Repositories (OleDb)]
    [Mappers]
    [QueryBuilder]
    [DataAccess (MS Access)]
}

[ViewModels] ..> [Application Layer] : Orchestration
[Application Layer] ..> [Repository Interfaces] : Use
[Repositories (OleDb)] --|> [Repository Interfaces] : Implements
[Repositories (OleDb)] --> [DataAccess (MS Access)] : Low-level I/O
@enduml
```
##### Use-case
```plantuml
@startuml
left to right direction
skinparam packageStyle rectangle

actor "Наблюдатель" as Reader
actor "Редактор" as Redactor
actor "Администратор" as Admin

Redactor --|> Reader
Admin --|> Redactor

rectangle "Система HotelViewer" {

  (Авторизация) as (Login)
  (Просмотр данных) as (View)
  (Поиск и фильтрация) as (Search)
  (Экспорт в CSV) as (Export)

  (Добавление данных) as (Add)
  (Редактирование данных) as (Edit)

  (Удаление данных) as (Delete)
  (Управление пользователями) as (Users)

  Reader --> (Login)
  Reader --> (View)
  Reader --> (Search)
  Reader --> (Export)

  Redactor --> (Add)
  Redactor --> (Edit)
  Redactor --> (Delete)

  Admin --> (Users)

  (Search) .> (View) : <<extend>>
  (Edit) ..> (View) : <<precede>>
}
@enduml
```

##### Диаграмма с потоком данных (DFD)
```plantuml
@startuml
skinparam backgroundColor white
skinparam BoxPadding 10
left to right direction

actor "Пользователь" as User
actor "Администратор" as Admin

rectangle "Система HotelViewer" {
    process "P1: Авторизация" as P1
    process "P2: Управление данными\n(CRUD)" as P2
    process "P3: Поиск и фильтрация" as P3
    process "P4: Формирование отчетов" as P4
    process "P5: Администрирование\nпользователей" as P5
}

database "БД MS Access" as DB

' Потоки данных
User --> P1 : Логин, Пароль
P1 --> DB : Запрос хэша
DB --> P1 : Хэш и Соль
P1 --> User : Статус сессии/Роль

User --> P2 : Данные сущности
P2 --> DB : SQL (Insert/Update/Delete)
DB --> P2 : Результат операции

User --> P3 : Критерии поиска
P3 --> DB : SELECT с фильтрами
DB --> P3 : Набор записей
P3 --> User : Отображение в таблице

User --> P4 : Запрос на экспорт
P4 --> DB : Чтение данных
P4 --> User : CSV файл

Admin --> P5 : Данные нового пользователя
P5 --> DB : Сохранение хэша
DB --> P5 : Список пользователей
P5 --> Admin : Список в UI

@enduml
```

Для большей надёжности, что было указано в ТЗ, были применены некоторые паттерны функционального программирования,
в частности - монады `Either` и `Option`, что позволяет нам избавиться от неожиданных исключений (точнее минимизировать их),
а также избавиться от типа `null`, заменив его на `Option`. Из этой же рубрики можно сказать и про лямбда-выражения.

Для безопасности и, опять же, надёжности, что всё ещё указано в ТЗ, вместо алгоритма общего хэширования семейства SHA
был использован алгоритм Argon2, который устойчив благодаря "соли" к переборам по радужным таблицам и сам алгоритм тоже
устойчив к перебору хэшей через графические процессоры и ASIC фермы.

**[Ознакомиться с текстом технического задания](./TECH_TASK.md)**

## Развёртка

### Требования
- Драйвер Microsoft Access Database Engine: [Скачать можно тут](https://www.microsoft.com/en-us/download/details.aspx?id=54920)
- ОС семейства Windows (10 и новее)

### Сборка из исходного кода
1. Склонируйте этот репозиторий с помощью (требуется git с расширением git-lfs)
  - `git clone git@github.com:Urtyom-Alyanov/HotelViewer.git` - SSH (рекомендуется, ибо безопасно), требует авторизации
    в GitHub через SSH ключ
  - `git clone https://github.com/Urtyom-Alyanov/HotelViewer.git` - HTTPS, ничего не требует, но медленнее.
2. Запустите сборку приложения (требуется .NET SDK) - `dotnet build -c Release` или/а потом `dotnet run -c Release --project .\source\HotelViewer.Presentation\`

### Скачать уже собранный проект
- [Последняя версия](https://github.com/Urtyom-Alyanov/HotelViewer/releases/tag/latest)
- [Последний коммит](https://github.com/Urtyom-Alyanov/HotelViewer/releases/tag/master)

## Использование

Для использования проект, требуется скачать .accdb файл, его можно [скачать отсюда](./assets/PureHotels.accdb), после
загрузки - выберите его. Изначально в ней нет данных - загрузите их с помощью соответствующей кнопки на окне входа.

## Отчёт

Отчёт по практике [выложен тут](./assets/Report.docx). Хранится он в Git LFS.
