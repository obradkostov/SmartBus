using SmartBus.DTOs;
using SmartBus.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SmartBus.Services.Internal;

public class ObavestenjeService : IObavestenjeService
{
    private readonly AppDbContext Context;

    public ObavestenjeService(AppDbContext context)
    {
        Context = context;
    }

    private ResponseObavestenjeDto MapToDto(Obavestenje obavestenje)
    {
        return new ResponseObavestenjeDto{
            Id = obavestenje.Id,
            PolazakId = obavestenje.PolazakId,
            Naslov = obavestenje.Naslov ?? string.Empty,
            Poruka = obavestenje.Poruka ?? string.Empty,
            VremeKreiranja = obavestenje.VremeKreiranja,
            Tip = obavestenje.Tip
        };
    }

    public async Task<ResponseObavestenjeDto> Create(CreateObavestenjeDto dto)
    {
        if(string.IsNullOrWhiteSpace(dto.Naslov)) throw new ArgumentException("[Obavestenje/Create] Nije unet naslov");
        if(string.IsNullOrEmpty(dto.Poruka)) throw new ArgumentException("[Obavestenje/Create] Nije uneta poruka");
        if(string.IsNullOrWhiteSpace(dto.Tip)) throw new ArgumentException("[Obavestenje/Create] Nije unet tip");
        var obavestenje = new Obavestenje
        {
            PolazakId = dto.PolazakId,
            Naslov = dto.Naslov,
            Poruka = dto.Poruka,
            Tip = dto.Tip,
            VremeKreiranja = DateTime.Now
        };
        var obavestenjeRet = await Context.Obavestenja.AddAsync(obavestenje);
        await Context.SaveChangesAsync();
        return MapToDto(obavestenjeRet.Entity);
    }

    public async Task<IList<ResponseObavestenjeDto>> Read()
    {
        var listaObavestenja = new List<ResponseObavestenjeDto>();
        var obavestenja = await Context.Obavestenja
            .OrderByDescending(o => o.VremeKreiranja)
            .ToListAsync();
        if(obavestenja == new List<Obavestenje>()) return listaObavestenja;
        foreach(var o in obavestenja)
        {
           listaObavestenja.Add(MapToDto(o)); 
        }
        return listaObavestenja;
    }

}

