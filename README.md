# PowerConnector
Conector multi datos con funcionales sencillas y útiles

Caracteristicas:
----------------
PowerConnector es una [libreria nuget] (https://www.nuget.org/packages/PowerConnector) que usted puede agregar a su proyecto, brindandole
poderosas herramientas de acceso a datos.

Ejemplos de uso:
----------------
Valores escalares
```csharp
using(IPowerConnector powerConnector = new PowerConnector(ProviderType.MySQL, "connectionString..."))
{
    var scalar = powerConnector.ExecuteScalar(CommandType.Text, "select count(*) from persons");
}
```

Valores escalares con filtros
```csharp
using(IPowerConnector powerConnector = new PowerConnector(ProviderType.MySQL, "connectionString..."))
{
    var scalar = powerConnector.ExecuteScalar(CommandType.Text, "select count(*) from persons where country = @country", "Argentina");
}
```

Procedimientos Almacenados.

Con los procedimientos almacenados solo basta con definir los valores, ya que los nombres de parametros se mapean automaticamente, la
única limitante es que debe definir los valores en el orden los parametros.
```sql
CREATE PROCEDURE GetPersonByDates 
	@Date1 DATE,
	@Date2 DATE
AS
BEGIN
	SET NOCOUNT ON;

    SELECT * FROM Persons WHERE RegisteredDate BETWEEN @Date1 AND @Date2
END
GO
```

```csharp
public class Persons
{
    public int? Age { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; }
    public float? Weight { get; set; }
    public DateTime RegisteredDate { get; set; }
}

using(IPowerConnector powerConnector = new PowerConnector(ProviderType.SqlServer, "connectionString..."))
{
    var personList = powerConnector.Query<Persons>(CommandType.StoredProcedure, "GetPersonByDates", "2016-01-01", "2016-02-01");
}
```
