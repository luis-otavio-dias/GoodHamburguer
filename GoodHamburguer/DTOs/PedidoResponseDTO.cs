namespace GoodHamburguer.DTOs;


public record PedidoResponseDTO(
    int Id,
    SanduicheResponseDTO Sanduiche,
    List<AcompanhamentoResponseDTO> Acompanhamentos,
    decimal Total
);