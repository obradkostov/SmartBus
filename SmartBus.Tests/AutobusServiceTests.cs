using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SmartBus.Data;
using SmartBus.DTOs.AutobusDto;
using SmartBus.Models;
using SmartBus.Services.Internal;

namespace SmartBus.Tests;

[TestFixture]
public class AutobusServiceTests
{
    private AppDbContext _context = null!;
    private AutobusService _service = null!;

    [SetUp]
    public void SetUp()
    {
        // Nova prazna in-memory baza za SVAKI test (testovi su nezavisni)
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _service = new AutobusService(_context, new SedisteService(_context));
    }

    [TearDown]
    public void TearDown() => _context.Dispose();

    private static Autobus NoviAutobus(string model = "Mercedes Tourismo", int sedista = 10, string? prevoznik = "Niš-Ekspres")
        => new() { Model = model, BrojSedista = sedista, Prevoznik = prevoznik };

    // ───────────── CREATE ─────────────

    [Test]
    public async Task Create_ValidBus_ReturnsDtoAndSavesBus()
    {
        var dto = await _service.Create(NoviAutobus());

        Assert.That(dto.Id, Is.GreaterThan(0));
        Assert.That(dto.Model, Is.EqualTo("Mercedes Tourismo"));
        Assert.That(await _context.Autobusi.CountAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task Create_ValidBus_GeneratesSeatsNumbered1ToN()
    {
        var dto = await _service.Create(NoviAutobus(sedista: 8));

        var sedista = await _context.Sedista.Where(s => s.AutobusId == dto.Id).OrderBy(s => s.Broj).ToListAsync();
        Assert.That(sedista, Has.Count.EqualTo(8));
        Assert.That(sedista.First().Broj, Is.EqualTo(1));
        Assert.That(sedista.Last().Broj, Is.EqualTo(8));
        Assert.That(sedista.All(s => s.Status == "slobodno"), Is.True);
    }

    [TestCase(6)]    // donja granica
    [TestCase(7)]
    [TestCase(99)]
    [TestCase(100)]  // gornja granica
    public async Task Create_SeatCountInsideBoundaries_Succeeds(int brojSedista)
    {
        var dto = await _service.Create(NoviAutobus(sedista: brojSedista));
        Assert.That(dto.BrojSedista, Is.EqualTo(brojSedista));
    }

    [TestCase(5)]     // tik ispod granice
    [TestCase(1)]
    [TestCase(0)]     // nula
    [TestCase(-1)]    // negativna
    [TestCase(-100)]
    [TestCase(101)]   // tik iznad granice
    [TestCase(int.MaxValue)]
    public void Create_SeatCountOutsideBoundaries_ThrowsArgumentException(int brojSedista)
    {
        var ex = Assert.ThrowsAsync<ArgumentException>(() => _service.Create(NoviAutobus(sedista: brojSedista)));
        Assert.That(ex!.Message, Does.Contain("broj sedista"));
    }

    [TestCase("")]
    [TestCase("   ")]
    public void Create_EmptyModel_ThrowsArgumentException(string model)
    {
        var ex = Assert.ThrowsAsync<ArgumentException>(() => _service.Create(NoviAutobus(model: model)));
        Assert.That(ex!.Message, Does.Contain("model"));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("  ")]
    public void Create_EmptyPrevoznik_ThrowsArgumentException(string? prevoznik)
    {
        var ex = Assert.ThrowsAsync<ArgumentException>(() => _service.Create(NoviAutobus(prevoznik: prevoznik)));
        Assert.That(ex!.Message, Does.Contain("prevoznik"));
    }

    [Test]
    public async Task Create_InvalidBus_DoesNotSaveAnything()
    {
        Assert.ThrowsAsync<ArgumentException>(() => _service.Create(NoviAutobus(sedista: 0)));

        Assert.That(await _context.Autobusi.CountAsync(), Is.EqualTo(0));
        Assert.That(await _context.Sedista.CountAsync(), Is.EqualTo(0));
    }

    // ───────────── READ ─────────────

    [Test]
    public async Task Read_ExistingId_ReturnsBus()
    {
        var created = await _service.Create(NoviAutobus(model: "Setra"));

        var dto = await _service.Read(created.Id);

        Assert.That(dto.Id, Is.EqualTo(created.Id));
        Assert.That(dto.Model, Is.EqualTo("Setra"));
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(99999)]
    public void Read_NonExistingId_ThrowsException(int id)
    {
        var ex = Assert.ThrowsAsync<Exception>(() => _service.Read(id));
        Assert.That(ex!.Message, Does.Contain("Nema autobusa"));
    }

    [Test]
    public async Task ReadAll_EmptyDatabase_ReturnsEmptyList()
    {
        var lista = await _service.Read();
        Assert.That(lista, Is.Empty);
    }

    [Test]
    public async Task ReadAll_ThreeBuses_ReturnsThree()
    {
        await _service.Create(NoviAutobus(model: "A"));
        await _service.Create(NoviAutobus(model: "B"));
        await _service.Create(NoviAutobus(model: "C"));

        var lista = await _service.Read();

        Assert.That(lista, Has.Count.EqualTo(3));
    }

    // ───────────── UPDATE ─────────────

    [Test]
    public async Task Update_ValidData_ChangesFieldsAndRegeneratesSeats()
    {
        var created = await _service.Create(NoviAutobus(sedista: 10));

        var dto = await _service.Update(new ResponseAutobusDto
        {
            Id = created.Id, Model = "Novi model", BrojSedista = 20, Prevoznik = "Novi prevoznik"
        });

        Assert.That(dto.Model, Is.EqualTo("Novi model"));
        Assert.That(dto.BrojSedista, Is.EqualTo(20));
        Assert.That(dto.Prevoznik, Is.EqualTo("Novi prevoznik"));
        Assert.That(await _context.Sedista.CountAsync(s => s.AutobusId == created.Id), Is.EqualTo(20));
    }

    [TestCase(5)]
    [TestCase(0)]
    [TestCase(-3)]
    [TestCase(101)]
    public async Task Update_InvalidSeatCount_ThrowsArgumentExceptionAndKeepsOldData(int brojSedista)
    {
        var created = await _service.Create(NoviAutobus(sedista: 10));

        Assert.ThrowsAsync<ArgumentException>(() => _service.Update(new ResponseAutobusDto
        {
            Id = created.Id, Model = "X", BrojSedista = brojSedista, Prevoznik = "Y"
        }));

        var iz = await _service.Read(created.Id);
        Assert.That(iz.BrojSedista, Is.EqualTo(10));
    }

    [Test]
    public async Task Update_EmptyModel_ThrowsArgumentException()
    {
        var created = await _service.Create(NoviAutobus());
        Assert.ThrowsAsync<ArgumentException>(() => _service.Update(new ResponseAutobusDto
        {
            Id = created.Id, Model = "", BrojSedista = 10, Prevoznik = "Y"
        }));
    }

    [Test]
    public async Task Update_EmptyPrevoznik_ThrowsArgumentException()
    {
        var created = await _service.Create(NoviAutobus());
        Assert.ThrowsAsync<ArgumentException>(() => _service.Update(new ResponseAutobusDto
        {
            Id = created.Id, Model = "X", BrojSedista = 10, Prevoznik = " "
        }));
    }

    [Test]
    public void Update_NonExistingId_ThrowsException()
    {
        var ex = Assert.ThrowsAsync<Exception>(() => _service.Update(new ResponseAutobusDto
        {
            Id = 12345, Model = "X", BrojSedista = 10, Prevoznik = "Y"
        }));
        Assert.That(ex!.Message, Does.Contain("Nema autobusa"));
    }

    // ───────────── DELETE ─────────────

    [Test]
    public async Task Delete_ExistingBus_RemovesBusAndItsSeats()
    {
        var created = await _service.Create(NoviAutobus(sedista: 10));

        await _service.Delete(created.Id);

        Assert.That(await _context.Autobusi.CountAsync(), Is.EqualTo(0));
        Assert.That(await _context.Sedista.CountAsync(), Is.EqualTo(0));
    }

    [Test]
    public async Task Delete_ExistingBus_RemovesItsRatings()
    {
        var created = await _service.Create(NoviAutobus());
        _context.OceneAutobusa.Add(new OcenaAutobus { AutobusId = created.Id, KorisnikId = 1, Ocena = 5 });
        await _context.SaveChangesAsync();

        await _service.Delete(created.Id);

        Assert.That(await _context.OceneAutobusa.CountAsync(), Is.EqualTo(0));
    }

    [Test]
    public async Task Delete_OneOfTwo_LeavesTheOtherUntouched()
    {
        var a = await _service.Create(NoviAutobus(model: "A"));
        var b = await _service.Create(NoviAutobus(model: "B"));

        await _service.Delete(a.Id);

        var ostali = await _service.Read();
        Assert.That(ostali, Has.Count.EqualTo(1));
        Assert.That(ostali[0].Id, Is.EqualTo(b.Id));
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(99999)]
    public void Delete_NonExistingId_ThrowsException(int id)
    {
        var ex = Assert.ThrowsAsync<Exception>(() => _service.Delete(id));
        Assert.That(ex!.Message, Does.Contain("Nema autobusa"));
    }

    [Test]
    public async Task Delete_SameBusTwice_SecondCallThrows()
    {
        var created = await _service.Create(NoviAutobus());
        await _service.Delete(created.Id);
        Assert.ThrowsAsync<Exception>(() => _service.Delete(created.Id));
    }
}
