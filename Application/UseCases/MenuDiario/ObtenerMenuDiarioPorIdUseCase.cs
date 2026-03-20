using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using DomainRepo = MenuSoda.Application.Dto;

namespace MenuSoda.Application.UseCases.MenuDiario;

public class ObtenerMenuDiarioPorIdUseCase
{
    private readonly IMenuDiarioRepository _menuDiarioRepository;
    private readonly IMenuDiarioEntradaRepository _menuDiarioEntradaRepository;
    private readonly IMenuDiarioPlatoRepository _menuDiarioPlatoRepository;
    private readonly IMenuDiarioPlatoAdicionalRepository _menuDiarioPlatoAdicionalRepository;
    private readonly IMenuDiarioImagenRepository _menuDiarioImagenRepository;

    public ObtenerMenuDiarioPorIdUseCase(
        IMenuDiarioRepository menuDiarioRepository,
        IMenuDiarioEntradaRepository menuDiarioEntradaRepository,
        IMenuDiarioPlatoRepository menuDiarioPlatoRepository,
        IMenuDiarioPlatoAdicionalRepository menuDiarioPlatoAdicionalRepository,
        IMenuDiarioImagenRepository menuDiarioImagenRepository)
    {
        _menuDiarioRepository = menuDiarioRepository;
        _menuDiarioEntradaRepository = menuDiarioEntradaRepository;
        _menuDiarioPlatoRepository = menuDiarioPlatoRepository;
        _menuDiarioPlatoAdicionalRepository = menuDiarioPlatoAdicionalRepository;
        _menuDiarioImagenRepository = menuDiarioImagenRepository;
    }

    public async Task<MenuDiarioDetailResponse?> ExecuteAsync(int id, CancellationToken ct)
    {
        var header = await _menuDiarioRepository.GetByIdAsync(id, ct);
        if (header == null) return null;

        var requestDetail = new DomainRepo.MenuDiarioGetDetailByMenuRequest { Codmendia = id };

        var entradas = await _menuDiarioEntradaRepository.GetByMenuAsync(requestDetail, ct);
        var platos = await _menuDiarioPlatoRepository.GetByMenuAsync(requestDetail, ct);
        var adicionales = await _menuDiarioPlatoAdicionalRepository.GetByMenuAsync(requestDetail, ct);
        var imagen = await _menuDiarioImagenRepository.GetByMenuAsync(requestDetail, ct);

        // Map Platos + Adicionales
        var platosList = new List<MenuDiarioPlatoWithAdicionalResponse>();
        if (platos != null)
        {
            foreach (var p in platos)
            {
                var adicional = adicionales?.FirstOrDefault(a => a.Codmendiapla == p.Id);

                platosList.Add(new MenuDiarioPlatoWithAdicionalResponse
                {
                    Id = p.Id,
                    Codpla = p.Codpla,
                    Planom = p.Planom,
                    Plades = p.Plades,
                    Codtippla = p.Codtippla,
                    Tipplades = p.Tipplades,
                    Codest = p.Codest,
                    Adicional = adicional
                });
            }
        }

        return new MenuDiarioDetailResponse
        {
            Id = header.Id,
            Fecha = header.Mendiafec,
            EstadoId = header.Codest,
            FechaRegistro = header.Fecreg,
            UsuarioRegistro = header.Usureg,
            FechaModificacion = header.Fecmod,
            UsuarioModificacion = header.Usumod,
            Entradas = entradas.ToList(),
            Platos = platosList,
            Imagen = imagen
        };
    }
}
