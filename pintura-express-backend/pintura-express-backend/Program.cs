using Microsoft.EntityFrameworkCore;
using pintura_express_backend.Data;
using pintura_express_backend.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Database connection 
builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

//Allow connection with Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular",
        policy => policy.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

var app = builder.Build();

app.UseCors("Angular");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Urls.Add("http://0.0.0.0:80");

app.MapGet("/", () => "API funcionando");

#region Productos

// GET all products
app.MapGet("/productos", async (Context db) =>
    await db.Productos.ToListAsync()
)
.WithName("Find products")
.WithTags("PRODUCT")
.WithOpenApi();

// GET products by ID
app.MapGet("/productos/{id}", async (int id, Context db) =>
{
    var producto = await db.Productos.FindAsync(id);
    return producto is not null ? Results.Ok(producto) : Results.NotFound();
})
.WithName("Find product by ID")
.WithTags("PRODUCT")
.WithOpenApi();

// POST product
app.MapPost("/productos", async (Producto producto, Context db) =>
{
    db.Productos.Add(producto);
    await db.SaveChangesAsync();
    return Results.Created($"/productos/{producto.ID}", producto);
})
.WithName("Add product")
.WithTags("PRODUCT")
.WithOpenApi();

// PUT product
app.MapPut("/productos/{id}", async (int id, Producto input, Context db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();

    producto.Nombre = input.Nombre;
    producto.Color = input.Color;
    producto.Tipo = input.Tipo;
    producto.Precio = input.Precio;
    producto.Stock = input.Stock;

    await db.SaveChangesAsync();
    return Results.Ok(producto);
})
.WithName("Edit product")
.WithTags("PRODUCT")
.WithOpenApi();

// DELETE product
app.MapDelete("/productos/{id}", async (int id, Context db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();

    db.Productos.Remove(producto);
    await db.SaveChangesAsync();
    return Results.Ok();
})
.WithName("Delete product")
.WithTags("PRODUCT")
.WithOpenApi();

#endregion

#region Ordenes

// POST orden
app.MapPost("/ordenes", async (OrdenCreateDTO dto, Context db) =>
{
    if (dto.Items == null || dto.Items.Count == 0)
        return Results.BadRequest("La orden debe tener productos");

    var orden = new Orden
    {
        ClientNom = dto.ClientNom,
        VendorNom = dto.VendorNom,
        CreatedAt = DateTime.Now,
        Estatus = "Pendiente"
    };

    decimal total = 0;

    db.Ordenes.Add(orden);
    await db.SaveChangesAsync(); // importante para obtener ID

    foreach (var item in dto.Items)
    {
        var producto = await db.Productos.FindAsync(item.ProductId);

        if (producto == null)
            return Results.NotFound("Producto no existe");

        if (producto.Stock < item.Quantity)
            return Results.BadRequest("Stock insuficiente");

        db.OrdenItems.Add(new OrdenItem
        {
            OrdenID = orden.ID,
            ProductoID = item.ProductId,
            Cantidad = item.Quantity
        });

        total += producto.Precio * item.Quantity;

        producto.Stock -= item.Quantity;
    }

    orden.Total = total;

    await db.SaveChangesAsync();

    return Results.Ok(orden);
})
.WithName("Add order")
.WithTags("ORDER")
.WithOpenApi();


// GET ordenes w details
app.MapGet("/ordenes", async (Context db) =>
{
    var data = await (
        from o in db.Ordenes
        join oi in db.OrdenItems on o.ID equals oi.OrdenID
        join p in db.Productos on oi.ProductoID equals p.ID
        select new
        {
            o.ID,
            o.ClientNom,
            o.VendorNom,
            o.Total,
            o.Estatus,
            o.CreatedAt,
            Producto = p.Nombre,
            oi.Cantidad
        }
    ).ToListAsync();

    return Results.Ok(data);
})
.WithName("Find orders w details")
.WithTags("ORDER")
.WithOpenApi();

#endregion

app.Run();
