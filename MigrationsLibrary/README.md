









Это штука работает на http://localhost:5000

---



\# 1. Как оно вообще работает



Ты создаёшь класс — например:



```csharp

\[Table("users")]

public class User

{

&nbsp;   \[PrimaryKey]

&nbsp;   public int Id { get; set; }



&nbsp;   \[Column("name")]

&nbsp;   public string Name { get; set; }

}

```





\# 2. Что нужно, чтобы оно работало



В базе PostgreSQL должна быть таблица:



```sql

CREATE TABLE migrations (

&nbsp;   id SERIAL PRIMARY KEY,

&nbsp;   name TEXT,

&nbsp;   applied\_at TIMESTAMP,

&nbsp;   up\_sql TEXT,

&nbsp;   down\_sql TEXT

);

```



Тут будут лежать миграции.



---



\# 3. Как создать миграцию



Открываешь браузер и пишешь:



```

http://localhost:5000/migrate/create

```



Если есть изменения — создаст новую миграцию.



Пример ответа:



```json

{

&nbsp; "name": "Migration20251115123400",

&nbsp; "status": "created"

}

```



\# 4. Как применить миграцию (создать таблицы)



Пишешь:



```

http://localhost:5000/migrate/apply

```



Если всё ок:



```json

{

&nbsp; "name": "Migration20251115123400",

&nbsp; "status": "applied"

}

```



---





Пример:



```csharp

\[Table("products")]

public class Product

{

&nbsp;   \[PrimaryKey]

&nbsp;   public int Id { get; set; }



&nbsp;   \[Column("title")]

&nbsp;   public string Title { get; set; }

}

```



Всё.



---



&nbsp;После этого:



\### Шаг 1 — создать миграцию



`/migrate/create`



\### Шаг 2 — применить миграцию



`/migrate/apply`



И таблица появится в базе.

---



