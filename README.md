# EntityFix
When you call stored procedure with select and output parameters and you don't select on some conditions (for example, you're just returning an error code) — you get a mapping EntityCommandExecutionException. This package fixes this annoying behavior of EF by returning empty data set and letting you read output parameters.

Example:
Consider you have this sql procedure:
```sql
CREATE PROCEDURE [dbo].[Foo]
    @Condition int = NULL,
    @ResultCode Int = NULL OUTPUT
AS
BEGIN
    IF @Condition IS NULL BEGIN
        SET @ResultCode = 1;       -- return 1 if condition = null
        RETURN;
    END
    SELECT 1 as 'One', 2 as 'Two'; -- select otherwise
    SET @ResultCode = 0;
END
```
Calling this procedure in EF:
```c#
var resultCodeParameter = new ObjectParameter("ResultCode", typeof(int));
var result = db.Foo(null, resultCodeParameter).FirstOrDefault();
var resultCode = resultCodeParameter.Value;
```
will throw an exception:

> System.Data.Entity.Core.EntityCommandExecutionException: The data reader is incompatible with the specified 'xxx.Foo_Result'. A member of the type, 'One', does not have a corresponding column in the data reader with the same name.

Calling **EntityFix.Load()** once on your application start will fix this bug.

#Installing
Using nuget:
```Batchfile
Install-Package EntityFix
```
or from [https://www.nuget.org/packages/EntityFix](https://www.nuget.org/packages/EntityFix/)