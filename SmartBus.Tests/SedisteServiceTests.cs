using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SmartBus.Data;
using SmartBus.DTOs.SedisteDto;
using SmartBus.Models;
using SmartBus.Services.Internal;

namespace SmartBus.Tests;

[TestFixture]
public class SedisteServiceTests
{
    private AppDbContext _context = null!;
    private SedisteService _service = null!;
    private int _autobusId;

    [SetUp]
    public async Task SetUp()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _service = new SedisteService(_context);

        var autobus = new Autobus { Model = "Test", BrojSedista = 10, Prevoznik = "P" };
        _context.Autobusi.Add(autobus);
        await _context.SaveChangesAsync();
        _autobusId = autobus.Id;
    }

    [TearDown]
    public void TearDown() => _context.Dispose();

    // ───────────── CREATE ─────────────

    [Test]
    public async Task Create_Valid_ReturnsFreeSeat()
    {
        var dto = await _service.Create(new CreateSedisteDto { AutobusId = _autobusId, Broj = 5 });

        Assert.That(dto.Broj, Is.EqualTo(5));
        Assert.That(dto.Status, Is.EqualTo("slobodno"));
        Assert.That(await _context.Sedista.CountAsync(), Is.EqualTo(1));
    }

    [Test]
    public void Create_ZeroBusId_ThrowsArgumentException()
        => Assert.ThrowsAsync<ArgumentException>(() => _service.Create(new CreateSedisteDto { AutobusId = 0, Broj = 1 }));

    [Test]
    public void Create_ZeroSeatNumber_ThrowsArgumentException()
        => Assert.ThrowsAsync<ArgumentException>(() => _service.Create(new CreateSedisteDto { AutobusId = _autobusId, Broj = 0 }));

    [Test]
    public void Create_NonExistingBus_ThrowsException()
    {
        var ex = Assert.ThrowsAsync<Exception>(() => _service.Create(new CreateSedisteDto { AutobusId = 9999, Broj = 1 }));
        Assert.That(ex!.Message, Does.Contain("Nije pronadjen autobus"));
    }

    // Ocekivano ponasanje: negativan broj sedista treba biti odbijen.
    // Trenutni kod proverava samo Broj == 0 -> ovaj test otkriva DEFEKT.
    [TestCase(-1)]
    [TestCase(-50)]
    public void Create_NegativeSeatNumber_ThrowsArgumentException(int broj)
        => Assert.ThrowsAsync<ArgumentException>(() => _service.Create(new CreateSedisteDto { AutobusId = _autobusId, Broj = broj }));

    // ───────────── READ ─────────────

    [Test]
    public async Task Read_Existing_ReturnsSeat()
    {
        var created = await _service.Create(new CreateSedisteDto { AutobusId = _autobusId, Broj = 3 });
        var dto = await _service.Read(created.Id);
        Assert.That(dto.Broj, Is.EqualTo(3));
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(9999)]
    public void Read_NonExisting_ThrowsException(int id)
    {
        var ex = Assert.ThrowsAsync<Exception>(() => _service.Read(id));
        Assert.That(ex!.Message, Does.Contain("Nema sedista"));
    }

    [Test]
    public async Task ReadForBus_ReturnsOnlySeatsOfThatBus()
    {
        var drugi = new Autobus { Model = "Drugi", BrojSedista = 10, Prevoznik = "P" };
        _context.Autobusi.Add(drugi);
        await _context.SaveChangesAsync();
        await _service.Create(new CreateSedisteDto { AutobusId = _autobusId, Broj = 1 });
        await _service.Create(new CreateSedisteDto { AutobusId = _autobusId, Broj = 2 });
        await _service.Create(new CreateSedisteDto { AutobusId = drugi.Id, Broj = 1 });

        var lista = await _service.ReadForBus(_autobusId);

        Assert.That(lista, Has.Count.EqualTo(2));
    }

    [Test]
    public void ReadForBus_NonExistingBus_ThrowsException()
    {
        var ex = Assert.ThrowsAsync<Exception>(() => _service.ReadForBus(9999));
        Assert.That(ex!.Message, Does.Contain("Nema autobusa"));
    }

    // ───────────── UPDATE ─────────────

    [Test]
    public async Task Update_NewNumber_ChangesNumber()
    {
        var c = await _service.Create(new CreateSedisteDto { AutobusId = _autobusId, Broj = 1 });
        var dto = await _service.Update(new UpdateSedisteDto { Id = c.Id, NoviBroj = 7, NoviStatus = "slobodno" });
        Assert.That(dto.Broj, Is.EqualTo(7));
    }

    // Ocekivano: status se menja u "zauzeto".
    // Trenutni kod ima obrnut uslov (Equals umesto !Equals) -> test otkriva DEFEKT.
    [Test]
    public async Task Update_NewStatus_ChangesStatus()
    {
        var c = await _service.Create(new CreateSedisteDto { AutobusId = _autobusId, Broj = 1 });
        var dto = await _service.Update(new UpdateSedisteDto { Id = c.Id, NoviBroj = 1, NoviStatus = "zauzeto" });
        Assert.That(dto.Status, Is.EqualTo("zauzeto"));
    }

    [Test]
    public void Update_ZeroId_ThrowsArgumentException()
        => Assert.ThrowsAsync<ArgumentException>(() => _service.Update(new UpdateSedisteDto { Id = 0, NoviBroj = 1, NoviStatus = "slobodno" }));

    [Test]
    public async Task Update_ZeroNumber_ThrowsArgumentException()
    {
        var c = await _service.Create(new CreateSedisteDto { AutobusId = _autobusId, Broj = 1 });
        Assert.ThrowsAsync<ArgumentException>(() => _service.Update(new UpdateSedisteDto { Id = c.Id, NoviBroj = 0, NoviStatus = "slobodno" }));
    }

    [TestCase("")]
    [TestCase("  ")]
    public async Task Update_EmptyStatus_ThrowsArgumentException(string status)
    {
        var c = await _service.Create(new CreateSedisteDto { AutobusId = _autobusId, Broj = 1 });
        Assert.ThrowsAsync<ArgumentException>(() => _service.Update(new UpdateSedisteDto { Id = c.Id, NoviBroj = 1, NoviStatus = status }));
    }

    [Test]
    public void Update_NonExisting_ThrowsException()
    {
        var ex = Assert.ThrowsAsync<Exception>(() => _service.Update(new UpdateSedisteDto { Id = 9999, NoviBroj = 1, NoviStatus = "slobodno" }));
        Assert.That(ex!.Message, Does.Contain("Nema sedista"));
    }

    // ───────────── DELETE ─────────────

    [Test]
    public async Task Delete_Existing_RemovesSeat()
    {
        var c = await _service.Create(new CreateSedisteDto { AutobusId = _autobusId, Broj = 1 });
        await _service.Delete(c.Id);
        Assert.That(await _context.Sedista.CountAsync(), Is.EqualTo(0));
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(9999)]
    public void Delete_NonExisting_ThrowsException(int id)
    {
        var ex = Assert.ThrowsAsync<Exception>(() => _service.Delete(id));
        Assert.That(ex!.Message, Does.Contain("Nije pronadjeno sediste"));
    }
}
