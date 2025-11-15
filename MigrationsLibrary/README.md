(Сделано при помощи chatGPT)









Это штука работает на http://localhost:5000

---



\# 1. Как оно вообще работает

В program.cs в строке var db = new Conection("Host=localhost;Username=postgres;Password=1234;Database=Happy"); Нужно прописать данные для подключениея к бд.



После ты создаёшь класс — например:



```csharp

\[Table("users")]

public class User

{

[PrimaryKey]

public int Id { get; set; }



[Column("name")]

 public string Name { get; set; }

}

```





# 2. Что нужно, чтобы оно работало



В базе PostgreSQL должна быть таблица:



```sql

CREATE TABLE migrations (

 id SERIAL PRIMARY KEY,

name TEXT,

applied\_at TIMESTAMP,

up\_sql TEXT,

down\_sql TEXT

);

```



Тут будут лежать миграции.



---



# 3. Как создать миграцию



Открываешь браузер и пишешь:



```

http://localhost:5000/migrate/create

```



Если есть изменения — создаст новую миграцию.



Пример ответа:



```json

{

"name": "Migration20251115123400",

"status": "created"

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

"name": "Migration20251115123400",

"status": "applied"

}

```



---





Пример:



```csharp

[Table("products")]

public class Product

{

[PrimaryKey]

 public int Id { get; set; }



[Column("title")]

 public string Title { get; set; }

}

```



Всё.



---



После этого:



\### Шаг 1 — создать миграцию



`/migrate/create`



\### Шаг 2 — применить миграцию



`/migrate/apply`



И таблица появится в базе.

---



