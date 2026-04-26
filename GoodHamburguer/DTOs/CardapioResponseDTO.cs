namespace GoodHamburguer.DTOs;


public record CardapioResponseDTO(
    List<SanduicheResponseDTO> Sanduiches,
    List<AcompanhamentoResponseDTO> Acompanhamentos
);