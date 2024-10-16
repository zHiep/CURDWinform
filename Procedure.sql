SELECT * FROM dbo.Customer

--Update 
CREATE PROCEDURE spKH_update
    @Id INT, -- Mã khách hàng
    @Name NVARCHAR(100), -- Tên khách hàng
    @Email VARCHAR(100), -- Email
    @PhoneNumber VARCHAR(15) -- Số điện thoại
AS
BEGIN
    UPDATE dbo.Customer
    SET Name = @Name,
        Email = @Email,
        PhoneNumber = @PhoneNumber
    WHERE Id = @Id;
END

--Insert
CREATE PROCEDURE spKH_insert
    @Name NVARCHAR(100), -- Tên khách hàng
    @Email VARCHAR(100), -- Email
    @PhoneNumber VARCHAR(15) -- Số điện thoại
AS
BEGIN
    INSERT INTO dbo.Customer (Name, Email, PhoneNumber)
    VALUES (@Name, @Email, @PhoneNumber);
END

--Delete
CREATE PROCEDURE spKH_delete
    @Id INT -- Mã khách hàng cần xóa
AS
BEGIN
    DELETE FROM dbo.Customer
    WHERE Id = @Id;
END

